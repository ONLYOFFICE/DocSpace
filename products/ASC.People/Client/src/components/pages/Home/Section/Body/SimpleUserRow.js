import React from "react";
import Row from "@appserver/components/row";
import Avatar from "@appserver/components/avatar";
import UserContent from "./userContent";
import history from "@appserver/common/history";
import { inject, observer } from "mobx-react";
import { Trans, useTranslation } from "react-i18next";
import toastr from "@appserver/common/components/Toast/toastr";
import { EmployeeStatus } from "@appserver/common/constants";
import { resendUserInvites } from "@appserver/common/api/people"; //TODO: Move to store action

const SimpleUserRow = ({
  person,
  sectionWidth,
  checked,
  isAdmin,
  isMobile,
  selectUser,
  selectGroup,
  deselectUser,
  homepage,
  setChangeEmailDialogVisible,
  setChangePasswordDialogVisible,
  setDeleteProfileDialogVisible,
  setDeleteSelfProfileDialogVisible,
  setDialogData,
  closeDialogs,
  updateUserStatus,
}) => {
  const { t } = useTranslation("Home");
  const isRefetchPeople = true;

  const {
    email,
    role,
    displayName,
    avatar,
    id,
    status,
    mobilePhone,
    options,
    userName,
  } = person;

  const onContentRowSelect = (checked, user) => {
    if (checked) {
      selectUser(user);
    } else {
      deselectUser(user);
    }
  };

  const onEmailSentClick = () => {
    window.open("mailto:" + email);
  };

  const onSendMessageClick = () => {
    window.open(`sms:${mobilePhone}`);
  };

  const onEditClick = () => {
    history.push(`${homepage}/edit/${userName}`);
  };

  const toggleChangeEmailDialog = () => {
    setDialogData({
      email,
      id,
    });

    setChangeEmailDialogVisible(true);
  };

  const toggleChangePasswordDialog = () => {
    setDialogData({
      email,
    });

    setChangePasswordDialogVisible(true);
  };

  const toggleDeleteSelfProfileDialog = (e) => {
    closeDialogs();

    setDialogData({
      email,
    });

    setDeleteSelfProfileDialogVisible(true);
  };

  const toggleDeleteProfileEverDialog = (e) => {
    closeDialogs();

    setDialogData({
      id,
      displayName,
      userName,
    });

    setDeleteProfileDialogVisible(true);
  };

  const onDisableClick = (e) => {
    //onLoading(true);
    updateUserStatus(EmployeeStatus.Disabled, [id], isRefetchPeople)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };

  const onEnableClick = (e) => {
    //onLoading(true);
    updateUserStatus(EmployeeStatus.Active, [id], isRefetchPeople)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };

  const onReassignDataClick = (e) => {
    history.push(`${homepage}/reassign/${userName}`);
  };

  const onDeletePersonalDataClick = (e) => {
    toastr.success("Context action: Delete personal data"); //TODO: Implement and add translation
  };

  const onInviteAgainClick = () => {
    //onLoading(true);
    resendUserInvites([id])
      .then(() =>
        toastr.success(
          <Trans
            i18nKey="MessageEmailActivationInstuctionsSentOnEmail"
            ns="Home"
          >
            The email activation instructions have been sent to the
            <strong>{{ email: email }}</strong> email address
          </Trans>
        )
      )
      .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };

  const getUserContextOptions = (options, id) => {
    return options.map((option) => {
      switch (option) {
        case "send-email":
          return {
            key: option,
            label: t("LblSendEmail"),
            "data-id": id,
            onClick: onEmailSentClick,
          };
        case "send-message":
          return {
            key: option,
            label: t("LblSendMessage"),
            "data-id": id,
            onClick: onSendMessageClick,
          };
        case "separator":
          return { key: option, isSeparator: true };
        case "edit":
          return {
            key: option,
            label: t("EditButton"),
            "data-id": id,
            onClick: onEditClick,
          };
        case "change-password":
          return {
            key: option,
            label: t("PasswordChangeButton"),
            "data-id": id,
            onClick: toggleChangePasswordDialog,
          };
        case "change-email":
          return {
            key: option,
            label: t("EmailChangeButton"),
            "data-id": id,
            onClick: toggleChangeEmailDialog,
          };
        case "delete-self-profile":
          return {
            key: option,
            label: t("DeleteSelfProfile"),
            "data-id": id,
            onClick: toggleDeleteSelfProfileDialog,
          };
        case "disable":
          return {
            key: option,
            label: t("DisableUserButton"),
            "data-id": id,
            onClick: onDisableClick,
          };
        case "enable":
          return {
            key: option,
            label: t("EnableUserButton"),
            "data-id": id,
            onClick: onEnableClick,
          };
        case "reassign-data":
          return {
            key: option,
            label: t("ReassignData"),
            "data-id": id,
            onClick: onReassignDataClick,
          };
        case "delete-personal-data":
          return {
            key: option,
            label: t("RemoveData"),
            "data-id": id,
            onClick: onDeletePersonalDataClick,
          };
        case "delete-profile":
          return {
            key: option,
            label: t("DeleteSelfProfile"),
            "data-id": id,
            onClick: toggleDeleteProfileEverDialog,
          };
        case "invite-again":
          return {
            key: option,
            label: t("LblInviteAgain"),
            "data-id": id,
            onClick: onInviteAgainClick,
          };
        default:
          break;
      }

      return undefined;
    });
  };

  const showContextMenu = options && options.length > 0;
  const checkedProps = isAdmin ? { checked } : {};

  const element = (
    <Avatar size="min" role={role} userName={displayName} source={avatar} />
  );

  const contextOptionsProps =
    (isAdmin && showContextMenu) || (showContextMenu && id === currentUserId)
      ? {
          contextOptions: getUserContextOptions(options, id),
        }
      : {};

  return (
    <Row
      key={id}
      status={status}
      data={person}
      element={element}
      onSelect={onContentRowSelect}
      {...checkedProps}
      {...contextOptionsProps}
      //needForUpdate={this.needForUpdate}
      sectionWidth={sectionWidth}
    >
      <UserContent
        isMobile={isMobile}
        //widthProp={widthProp}
        user={person}
        history={history}
        selectGroup={selectGroup}
        sectionWidth={sectionWidth}
      />
    </Row>
  );
};

export default inject(({ auth, peopleStore }, { person }) => {
  return {
    homepage: auth.settingsStore.homepage,
    isAdmin: auth.isAdmin,
    checked: peopleStore.selectionStore.selection.some(
      (el) => el.id === person.id
    ),
    selectUser: peopleStore.selectionStore.selectUser,
    deselectUser: peopleStore.selectionStore.deselectUser,
    selectGroup: peopleStore.selectedGroupStore.selectGroup,

    setChangeEmailDialogVisible:
      peopleStore.dialogStore.setChangeEmailDialogVisible,

    setChangePasswordDialogVisible:
      peopleStore.dialogStore.setChangePasswordDialogVisible,

    setDeleteSelfProfileDialogVisible:
      peopleStore.dialogStore.setDeleteSelfProfileDialogVisible,

    setDeleteProfileDialogVisible:
      peopleStore.dialogStore.setDeleteProfileDialogVisible,

    setDialogData: peopleStore.dialogStore.setDialogData,
    closeDialogs: peopleStore.dialogStore.closeDialogs,

    updateUserStatus: peopleStore.usersStore.updateUserStatus,
  };
})(observer(SimpleUserRow));
