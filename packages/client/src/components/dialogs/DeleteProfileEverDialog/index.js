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
import styled, { css } from "styled-components";
import { size } from "@docspace/components/utils/device";

const { deleteUser } = api.people;
const { Filter } = api;

const StyledModalDialogContainer = styled(ModalDialogContainer)`
  #modal-dialog {
    ${(props) =>
      props.needReassignData &&
      css`
        width: auto;

        @media (min-width: ${size.smallTablet}px) {
          .delete-button,
          .cancel-button {
            width: auto;
          }
        }
      `}

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

    ${(props) =>
      !props.needReassignData &&
      css`
        padding-bottom: 0;
      `}
  }

  .reassign-data {
    line-height: 15px;
  }
`;

const DeleteProfileEverDialogComponent = (props) => {
  const {
    user,
    t,
    onClose,
    tReady,
    visible,
    setDataReassignmentDialogVisible,
    setDeleteProfileDialogVisible,
    setDataReassignmentDeleteProfile,
    setIsDeletingUserWithReassignment,
    userPerformedDeletion,
    setDialogData,
    getUsersList,
  } = props;
  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const needReassignData = user.isRoomAdmin || user.isOwner || user.isAdmin;

  const deleteMessage = (
    <Trans i18nKey="DeleteUserMessage" ns="DeleteProfileEverDialog" t={t}>
      {{ userCaption: t("Common:User") }}
      <strong>{{ user: user.displayName }}</strong>
    </Trans>
  );
  const warningMessageMyDocuments = t("DeleteMyDocumentsUser");
  const warningMessageReassign = (
    <Trans
      i18nKey="DeleteReassignDescriptionUser"
      ns="DeleteProfileEverDialog"
      t={t}
    >
      {{ warningMessageMyDocuments }}
      <strong>
        {{ userPerformedDeletion: userPerformedDeletion.displayName }}
        {{ userYou: t("Common:You") }}
      </strong>
    </Trans>
  );
  const warningMessage = needReassignData
    ? warningMessageReassign
    : warningMessageMyDocuments;

  const onDeleteUser = (id) => {
    const filter = Filter.getDefault();
    setIsRequestRunning(true);

    deleteUser(id)
      .then(() => {
        toastr.success(t("SuccessfullyDeleteUserInfoMessage"));
        getUsersList(filter, true);

        return;
      })
      .catch((error) => toastr.error(error))
      .finally(() => {
        setIsRequestRunning(false);
        onClose();
      });
  };

  const onDeleteProfileEver = () => {
    if (!needReassignData) {
      onDeleteUser(user.id);
      return;
    }

    setDialogData(user);

    setIsDeletingUserWithReassignment(true);
    setDataReassignmentDialogVisible(true);
    setDataReassignmentDeleteProfile(true);
    setDeleteProfileDialogVisible(false);
  };

  const onClickReassignData = () => {
    setDialogData(user);

    setDataReassignmentDialogVisible(true);
    setDataReassignmentDeleteProfile(true);
    setDeleteProfileDialogVisible(false);
  };

  return (
    <StyledModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      needReassignData={needReassignData}
    >
      <ModalDialog.Header>{t("DeleteUser")}</ModalDialog.Header>
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
            {t("DeleteProfileEverDialog:ReassignDataToAnotherUser")}
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
  "Common",
  "PeopleTranslations",
])(DeleteProfileEverDialogComponent);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default inject(({ peopleStore }) => {
  const { dialogStore, selectionStore } = peopleStore;

  const { getUsersList } = peopleStore.usersStore;

  const {
    setDataReassignmentDialogVisible,
    setDeleteProfileDialogVisible,
    setDataReassignmentDeleteProfile,
    setIsDeletingUserWithReassignment,
    setDialogData,
  } = dialogStore;

  const { setSelected } = selectionStore;

  return {
    setDataReassignmentDialogVisible,
    setDeleteProfileDialogVisible,
    setDataReassignmentDeleteProfile,
    setIsDeletingUserWithReassignment,
    setDialogData,
    setSelected,
    removeUser: peopleStore.usersStore.removeUser,
    userPerformedDeletion: peopleStore.authStore.userStore.user,
    getUsersList,
  };
})(observer(DeleteProfileEverDialog));
