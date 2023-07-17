import { useState, useEffect, useCallback } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import PeopleSelector from "@docspace/client/src/components/PeopleSelector";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Avatar from "@docspace/components/avatar";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import CatalogSpamIcon from "PUBLIC_DIR/images/catalog.spam.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import toastr from "@docspace/components/toast/toastr";
import SelectorAddButton from "@docspace/components/selector-add-button";
import { Base } from "@docspace/components/themes";
import { useNavigate } from "react-router-dom";
import {
  StyledOwnerInfo,
  StyledPeopleSelectorInfo,
  StyledPeopleSelector,
  StyledAvailableList,
  StyledFooterWrapper,
  StyledSelectedOwnerContainer,
  StyledSelectedOwner,
} from "../ChangePortalOwnerDialog/StyledDialog";

import Progress from "./sub-components/Progress/Progress";
import Loader from "./sub-components/Loader/Loader";

import api from "@docspace/common/api";
const { deleteUser } = api.people; //TODO: Move to action
const { Filter } = api;

const StyledModalDialog = styled(ModalDialog)`
  .avatar-name,
  .delete-profile-container {
    display: flex;
    align-items: center;
  }

  .delete-profile-checkbox {
    margin-bottom: 16px;
  }

  .list-container {
    gap: 6px;
  }
`;

const StyledCatalogSpamIcon = styled(CatalogSpamIcon)`
  ${commonIconsStyles}
  path {
    fill: #f21c0e;
  }

  padding-left: 8px;
`;

let timerId = null;

const DataReassignmentDialog = ({
  visible,
  user,
  setDataReassignmentDialogVisible,
  dataReassignment,
  dataReassignmentProgress,
  currentColorScheme,
  idCurrentUser,
  dataReassignmentDeleteProfile,
  dataReassignmentDeleteAdministrator,
  t,
  tReady,
  reassignDataIds,

  setFilter,

  removeUser,
  filter,

  setSelected,
  setDataReassignmentDeleteAdministrator,
}) => {
  // const { id, avatar, displayName, statusType, deleteProfile } = user;
  const [selectorVisible, setSelectorVisible] = useState(false);

  if (dataReassignmentDeleteAdministrator) {
    dataReassignmentDeleteAdministrator.label =
      dataReassignmentDeleteAdministrator.displayName;
  }

  const defaultSelectedUser = dataReassignmentDeleteAdministrator;

  const [selectedUser, setSelectedUser] = useState(defaultSelectedUser);
  const [isLoading, setIsLoading] = useState(false);
  const [isDeleteProfile, setIsDeleteProfile] = useState(
    dataReassignmentDeleteProfile
  );
  const [showProgress, setShowProgress] = useState(false);
  const [isReassignCurrentUser, setIsReassignCurrentUser] = useState(false);

  const [percent, setPercent] = useState(0);

  const onlyOneUser = user.length === 1;
  const navigate = useNavigate();

  console.log(
    "DataReassignmentDialog user reassignDataIds onlyOneUser ",
    user,
    reassignDataIds,
    onlyOneUser
  );

  const onDeleteUser = (id) => {
    const filter = Filter.getDefault();
    const params = filter.toUrlParams();
    const url = `/accounts/filter?${params}`;

    console.log("onDeleteUser", id);
    deleteUser(id)
      .then((res) => {
        toastr.success(t("SuccessfullyDeleteUserInfoMessage"));
        navigate(url, params);
        setFilter(filter);
        return;
      })
      .catch((error) => toastr.error(error))
      .finally(() => {
        // onClose();
      });
  };

  const onDeleteUsers = (ids) => {
    console.log("onDeleteUsers", ids);

    removeUser(ids, filter)
      .then(() => {
        toastr.success(t("DeleteGroupUsersSuccessMessage"));
      })
      .catch((error) => toastr.error(error))
      .finally(() => {
        setSelected("close");
        // onClose();
      });
  };

  useEffect(() => {
    if (percent === 100) {
      clearInterval(timerId);
    }
  }, [percent]);

  useEffect(() => {
    //If click Delete
    if (dataReassignmentDeleteAdministrator) {
      onReassign();
    }
    return () => {
      setDataReassignmentDeleteAdministrator(null);
    };
  }, [dataReassignmentDeleteAdministrator]);

  const onToggleDeleteProfile = () => {
    setIsDeleteProfile((remove) => !remove);
  };

  const onTogglePeopleSelector = () => {
    setSelectorVisible((show) => !show);
  };

  const onClose = () => {
    setDataReassignmentDialogVisible(false);
  };

  const onAccept = (item) => {
    setSelectorVisible(false);
    setSelectedUser({ ...item[0] });
  };

  const firstLetterToUppercase = (str) => {
    return str[0].toUpperCase() + str.slice(1);
  };

  const checkReassignCurrentUser = () => {
    setIsReassignCurrentUser(idCurrentUser === selectedUser.id);
  };

  const checkProgress = (ids) => {
    console.log("checkProgress", ids);
    dataReassignmentProgress(ids)
      .then((res) => {
        setPercent(res.percentage);
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });
  };

  const onReassign = useCallback(() => {
    // checkReassignCurrentUser();
    setIsLoading(true);
    setShowProgress(true);

    let reassignIds = [];
    let deleteIds = [];

    if (!onlyOneUser) {
      user.forEach((item) => {
        item.isRoomAdmin ? reassignIds.push(item.id) : deleteIds.push(item.id);
      });
    }

    const fromUserIds = onlyOneUser ? [user[0].id] : reassignIds;

    console.log(
      "dataReassignment",
      fromUserIds,
      selectedUser.id,
      isDeleteProfile
    );

    console.log("deleteIds reassignIds", deleteIds, reassignIds);

    // with simple delete
    if (deleteIds.length && isDeleteProfile) {
      if (deleteIds.length === 1) {
        onDeleteUser(deleteIds);
      } else onDeleteUsers(deleteIds);
    }

    dataReassignment(fromUserIds, selectedUser.id, isDeleteProfile)
      .then(() => {
        toastr.success(t("Common:ChangesSavedSuccessfully"));

        timerId = setInterval(() => checkProgress(fromUserIds), 500);
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });

    setIsLoading(false);
  }, [onlyOneUser, user, selectedUser, isDeleteProfile]);

  return (
    <StyledModalDialog
      displayType="aside"
      visible={visible}
      onClose={onClose}
      containerVisible={selectorVisible}
      withFooterBorder
      withBodyScroll
    >
      {selectorVisible && (
        <ModalDialog.Container>
          <PeopleSelector
            acceptButtonLabel={t("Common:SelectAction")}
            // excludeItems={[id]}
            onAccept={onAccept}
            onCancel={onClose}
            onBackClick={onTogglePeopleSelector}
            withCancelButton
            withAbilityCreateRoomUsers
          />
        </ModalDialog.Container>
      )}

      <ModalDialog.Header>Data reassignment</ModalDialog.Header>

      <ModalDialog.Body>
        {!tReady && <Loader />}

        {tReady && showProgress && (
          <Progress
            isReassignCurrentUser={isReassignCurrentUser}
            fromUser={user[0].displayName}
            toUser={selectedUser.label}
            percent={percent}
          />
        )}

        {tReady && !showProgress && (
          <>
            {onlyOneUser && !reassignDataIds && (
              <StyledOwnerInfo>
                <Avatar
                  className="avatar"
                  role="user"
                  source={user[0].avatar}
                  size={"big"}
                  hideRoleIcon
                />
                <div className="info">
                  <div className="avatar-name">
                    <Text
                      className="display-name"
                      noSelect
                      title={user[0].displayName}
                    >
                      {user[0].displayName}
                    </Text>
                    {user[0].statusType === "disabled" && (
                      <StyledCatalogSpamIcon size="small" />
                    )}
                  </div>

                  <Text className="status" noSelect>
                    {firstLetterToUppercase(user[0].statusType)}
                  </Text>
                </div>
              </StyledOwnerInfo>
            )}
            <StyledPeopleSelectorInfo>
              <Text className="new-owner" noSelect>
                New data owner
              </Text>
              <Text className="description" noSelect>
                User to whom the data will be transferred
              </Text>
            </StyledPeopleSelectorInfo>
            {selectedUser ? (
              <StyledSelectedOwnerContainer>
                <StyledSelectedOwner currentColorScheme={currentColorScheme}>
                  <Text className="text">{selectedUser.label}</Text>
                </StyledSelectedOwner>

                <Link
                  type={"action"}
                  isHovered
                  fontWeight={600}
                  onClick={onTogglePeopleSelector}
                >
                  {t("ChangePortalOwner:ChangeUser")}
                </Link>
              </StyledSelectedOwnerContainer>
            ) : (
              <StyledPeopleSelector>
                <SelectorAddButton
                  className="selector-add-button"
                  onClick={onTogglePeopleSelector}
                />
                <Text
                  className="label"
                  noSelect
                  title={t("Translations:ChooseFromList")}
                >
                  {t("Translations:ChooseFromList")}
                </Text>
              </StyledPeopleSelector>
            )}
            <StyledAvailableList className="list-container">
              <Text className="list-item" noSelect>
                We will transfer rooms created by user and documents stored in
                user’s rooms.
              </Text>
              <Text className="list-item" noSelect>
                Note: this action cannot be undone.
              </Text>

              <Link
                type={"action"}
                isHovered
                fontWeight={600}
                style={{ textDecoration: "underline" }}
              >
                More about data transfer
              </Link>
            </StyledAvailableList>
          </>
        )}
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <StyledFooterWrapper>
          {!showProgress && (
            <div className="delete-profile-container">
              <Checkbox
                className="delete-profile-checkbox"
                isChecked={isDeleteProfile}
                onClick={onToggleDeleteProfile}
              />
              <Text className="info" noSelect>
                {onlyOneUser
                  ? "Delete profile when reassignment is finished"
                  : "Delete profiles when reassignment is finished"}
              </Text>
            </div>
          )}

          <div className="button-wrapper">
            {!showProgress && (
              <Button
                tabIndex={5}
                label="Reassign"
                size="normal"
                primary
                scale
                isDisabled={!selectedUser}
                onClick={onReassign}
                isLoading={isLoading}
              />
            )}
            <Button
              tabIndex={5}
              label={t("Common:CancelButton")}
              size="normal"
              scale
              onClick={onClose}
              isDisabled={isLoading}
            />
          </div>
        </StyledFooterWrapper>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, peopleStore, setup }) => {
  const {
    dataReassignmentDialogVisible,
    setDataReassignmentDialogVisible,
    dataReassignmentDeleteProfile,
    reassignDataIds,
    dataReassignmentDeleteAdministrator,
    setDataReassignmentDeleteAdministrator,
  } = peopleStore.dialogStore;
  const { currentColorScheme } = auth.settingsStore;
  const { dataReassignment, dataReassignmentProgress } = setup;

  const { id: idCurrentUser } = peopleStore.authStore.userStore.user;

  const { filter, setFilter } = peopleStore.filterStore;
  const { removeUser } = peopleStore.usersStore;
  const { setSelected } = peopleStore.selectionStore;

  return {
    dataReassignmentDialogVisible,
    setDataReassignmentDialogVisible,
    theme: auth.settingsStore.theme,
    currentColorScheme,
    dataReassignment,
    idCurrentUser,
    dataReassignmentProgress,
    dataReassignmentDeleteProfile,
    reassignDataIds,
    filter,
    setFilter,
    removeUser,
    setSelected,

    dataReassignmentDeleteAdministrator,
    setDataReassignmentDeleteAdministrator,
  };
})(
  observer(
    withTranslation(["Common,Translations,ChangePortalOwner"])(
      DataReassignmentDialog
    )
  )
);
