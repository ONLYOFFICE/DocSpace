import React from "react";
import { useNavigate } from "react-router-dom";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";

import { withTranslation, Trans } from "react-i18next";
import api from "@docspace/common/api";
import toastr from "@docspace/components/toast/toastr";
import ModalDialogContainer from "../ModalDialogContainer";
import Link from "@docspace/components/link";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import styled from "styled-components";
import { size } from "@docspace/components/utils/device";

const { deleteUser } = api.people; //TODO: Move to action
const { Filter } = api;

const StyledModalDialogContainer = styled(ModalDialogContainer)`
  #modal-dialog {
    width: auto;
    max-width: 520px;
    max-height: none;
  }

  .user-delete {
    line-height: 20px;
    padding-bottom: 16px;
  }

  .text-warning {
    color: #f24724;
    font-size: 16px;
    font-weight: 700;
    line-height: 22px;
  }

  .text-delete-description {
    line-height: 20px;
    padding: 8px 0;
  }

  .reassign-data {
    line-height: 15px;
  }

  @media (min-width: ${size.smallTablet}px) {
    .delete-button,
    .cancel-button {
      width: auto;
    }
  }
`;

const DeleteProfileEverDialogComponent = (props) => {
  const {
    user,
    t,
    homepage,
    setFilter,
    onClose,
    tReady,
    visible,
    setDataReassignmentDialogVisible,
    setDeleteProfileDialogVisible,
    setDataReassignmentDeleteProfile,
    setDataReassignmentDeleteAdministrator,
    userPerformedDeletion,

    removeUser,
    setDialogData,
    userIds,
    filter,
    setSelected,
    setReassignDataIds,
  } = props;
  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const navigate = useNavigate();

  console.log("user", user);

  const onlyOneUser = user.length === 1;
  const needReassignData = onlyOneUser
    ? user[0].isRoomAdmin
    : user.some((item) => item.isRoomAdmin);

  console.log("onlyOneUser needReassignData", onlyOneUser, needReassignData);
  console.log("user", user);

  const header = onlyOneUser ? t("DeleteUser") : t("DeleteUsers");
  const deleteMessage = onlyOneUser ? (
    <Trans i18nKey="DeleteUserMessage" ns="DeleteProfileEverDialog" t={t}>
      {{ userCaption: t("Common:User") }}
      <strong>{{ user: user[0].displayName }}</strong>
      will be deleted. This action cannot be undone.
    </Trans>
  ) : (
    t("DeleteUsersMessage")
  );
  const i18nKeyWarningMessage = onlyOneUser
    ? "DeleteReassignDescriptionUser"
    : "DeleteReassignDescriptionUsers";

  const warningMessageMyDocuments = onlyOneUser
    ? t("DeleteMyDocumentsUser")
    : t("DeleteMyDocumentsUsers");

  const warningMessageReassign = (
    <Trans i18nKey={i18nKeyWarningMessage} ns="DeleteProfileEverDialog" t={t}>
      {{ warningMessageMyDocuments }}
      <strong>
        {{ userPerformedDeletion: userPerformedDeletion.displayName }}
        {{ userYou: t("Common:You") }}
      </strong>
    </Trans>
  );

  const warningMessageOnlyDelete = warningMessageMyDocuments;

  const warningMessage = needReassignData
    ? warningMessageReassign
    : warningMessageOnlyDelete;

  const onDeleteUser = (id) => {
    const filter = Filter.getDefault();
    const params = filter.toUrlParams();
    const url = `/accounts/filter?${params}`;
    setIsRequestRunning(true);
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
        setIsRequestRunning(false);
        onClose();
      });
  };

  const onDeleteUsers = (ids) => {
    console.log("onDeleteUsers", ids);
    setIsRequestRunning(true);
    removeUser(ids, filter)
      .then(() => {
        toastr.success(t("DeleteGroupUsersSuccessMessage"));
      })
      .catch((error) => toastr.error(error))
      .finally(() => {
        setSelected("close");
        onClose();
        setIsRequestRunning(false);
      });
  };
  const onDeleteProfileEver = () => {
    // If room for 1 and more
    if (needReassignData) {
      const ids = user.map((item) => item.id);
      console.log("onDeleteProfileEver ids", ids);

      onlyOneUser ? setDialogData(user) : setReassignDataIds(ids);

      setDataReassignmentDeleteAdministrator(userPerformedDeletion);
      setDataReassignmentDialogVisible(true);
      setDataReassignmentDeleteProfile(true);
      setDeleteProfileDialogVisible(false);
    }

    // If not room
    if (!needReassignData) {
      // 1
      if (onlyOneUser) {
        onDeleteUser(user[0].id);
      }
      // more  1
      if (!onlyOneUser) {
        onDeleteUsers(userIds);
      }
    }
  };

  const onClickReassignData = () => {
    const ids = user.map((item) => item.id);
    console.log("onClickReassignData ids", ids);

    onlyOneUser ? setDialogData(user) : setReassignDataIds(ids);

    setDataReassignmentDialogVisible(true);
    setDataReassignmentDeleteProfile(true);
    setDeleteProfileDialogVisible(false);
  };

  return (
    <StyledModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text className="user-delete">{deleteMessage}</Text>
        <Text className="text-warning">{t("Common:Warning")}!</Text>
        <Text className="text-delete-description">{warningMessage}</Text>

        {needReassignData && (
          <Link
            className="reassign-data"
            type="action"
            fontSize="13px"
            fontWeight={600}
            isHovered={true}
            onClick={onClickReassignData}
          >
            {t("DataReassignmentDialog:ReassignData")}
          </Link>
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="delete-button"
          key="OKBtn"
          label={t("Common:Delete")}
          size="normal"
          primary={true}
          scale
          onClick={onDeleteProfileEver}
          isLoading={isRequestRunning}
        />
        <Button
          className="cancel-button"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialogContainer>
  );
};

const DeleteProfileEverDialog = withTranslation([
  "DeleteProfileEverDialog",
  "DataReassignmentDialog",
  "Common",
  "PeopleTranslations",
])(DeleteProfileEverDialogComponent);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  // user: PropTypes.object.isRequired,
};

export default inject(({ peopleStore }) => ({
  homepage: config.homepage,

  setFilter: peopleStore.filterStore.setFilterParams,

  setDataReassignmentDialogVisible:
    peopleStore.dialogStore.setDataReassignmentDialogVisible,

  setDeleteProfileDialogVisible:
    peopleStore.dialogStore.setDeleteProfileDialogVisible,

  setDataReassignmentDeleteProfile:
    peopleStore.dialogStore.setDataReassignmentDeleteProfile,
  setDataReassignmentDeleteAdministrator:
    peopleStore.dialogStore.setDataReassignmentDeleteAdministrator,

  userPerformedDeletion: peopleStore.authStore.userStore.user,

  userIds: peopleStore.selectionStore.getUsersToRemoveIds,

  setDialogData: peopleStore.dialogStore.setDialogData,
  setReassignDataIds: peopleStore.dialogStore.setReassignDataIds,

  removeUser: peopleStore.usersStore.removeUser,
  filter: peopleStore.filterStore.filter,
  setSelected: peopleStore.selectionStore.setSelected,
}))(observer(DeleteProfileEverDialog));
