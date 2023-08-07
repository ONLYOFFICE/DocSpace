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

  setFilter,

  setDataReassignmentDeleteAdministrator,
}) => {
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

  const navigate = useNavigate();

  const updateAccountsAfterDeleteUser = () => {
    const filter = Filter.getDefault();
    const params = filter.toUrlParams();
    const url = `/accounts/filter?${params}`;

    navigate(url, params);
    setFilter(filter);
    return;
  };

  useEffect(() => {
    if (percent === 100) {
      clearInterval(timerId);

      isDeleteProfile && updateAccountsAfterDeleteUser();
    }
  }, [percent, isDeleteProfile]);

  useEffect(() => {
    //If click Delete user
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
    dataReassignmentProgress(ids)
      .then((res) => {
        setPercent(res.percentage);
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });
  };

  const onReassign = useCallback(() => {
    checkReassignCurrentUser();
    setIsLoading(true);
    setShowProgress(true);

    const fromUserIds = user.id;

    dataReassignment([fromUserIds], selectedUser.id, isDeleteProfile)
      .then(() => {
        toastr.success(t("Common:ChangesSavedSuccessfully"));

        timerId = setInterval(() => checkProgress(fromUserIds), 500);
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });

    setIsLoading(false);
  }, [user, selectedUser, isDeleteProfile]);

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
            excludeItems={[user.id]}
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
            fromUser={user.displayName}
            toUser={selectedUser.label}
            percent={percent}
          />
        )}

        {tReady && !showProgress && (
          <>
            <StyledOwnerInfo>
              <Avatar
                className="avatar"
                role="user"
                source={user.avatar}
                size={"big"}
                hideRoleIcon
              />
              <div className="info">
                <div className="avatar-name">
                  <Text
                    className="display-name"
                    noSelect
                    title={user.displayName}
                  >
                    {user.displayName}
                  </Text>
                  {user.statusType === "disabled" && (
                    <StyledCatalogSpamIcon size="small" />
                  )}
                </div>

                <Text className="status" noSelect>
                  {firstLetterToUppercase(user.statusType)}
                </Text>
              </div>
            </StyledOwnerInfo>

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
                Delete profile when reassignment is finished
              </Text>
            </div>
          )}

          <div className="button-wrapper">
            {!showProgress && (
              <Button
                label="Reassign"
                size="normal"
                primary
                scale
                isDisabled={!selectedUser}
                onClick={onReassign}
                isLoading={isLoading}
              />
            )}
            {!(showProgress && percent === 100) && (
              <Button
                label={t("Common:CancelButton")}
                size="normal"
                scale
                onClick={onClose}
                isDisabled={isLoading}
              />
            )}
            {percent === 100 && (
              <Button
                label="Ok"
                size="normal"
                scale
                onClick={onClose}
                isDisabled={isLoading}
              />
            )}
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
    dataReassignmentDeleteAdministrator,
    setDataReassignmentDeleteAdministrator,
  } = peopleStore.dialogStore;
  const { currentColorScheme } = auth.settingsStore;
  const { dataReassignment, dataReassignmentProgress } = setup;

  const { id: idCurrentUser } = peopleStore.authStore.userStore.user;

  const { setFilterParams: setFilter } = peopleStore.filterStore;

  return {
    dataReassignmentDialogVisible,
    setDataReassignmentDialogVisible,
    theme: auth.settingsStore.theme,
    currentColorScheme,
    dataReassignment,
    idCurrentUser,
    dataReassignmentProgress,
    dataReassignmentDeleteProfile,

    setFilter,

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
