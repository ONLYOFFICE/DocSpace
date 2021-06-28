import React from "react";
import Row from "@appserver/components/row";
import Avatar from "@appserver/components/avatar";
import UserContent from "./userContent";
import { inject, observer } from "mobx-react";
import { Trans, useTranslation } from "react-i18next";
import toastr from "studio/toastr";
import { AppServerConfig, EmployeeStatus } from "@appserver/common/constants";
import { resendUserInvites } from "@appserver/common/api/people"; //TODO: Move to store action
import { withRouter } from "react-router";
import config from "../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";

const SimpleUserRow = ({
  person,
  sectionWidth,
  checked,
  isAdmin,
  isMobile,
  selectUser,
  selectGroup,
  deselectUser,
  setChangeEmailDialogVisible,
  setChangePasswordDialogVisible,
  setDeleteProfileDialogVisible,
  setDeleteSelfProfileDialogVisible,
  setDialogData,
  closeDialogs,
  updateUserStatus,
  history,
  fetchProfile,
}) => {
  const { t } = useTranslation(["Home", "Translations"]);
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
    currentUserId,
  } = person;

  const onContentRowSelect = (checked, user) =>
    checked ? selectUser(user) : deselectUser(user);

  const onEmailSentClick = () => {
    window.open("mailto:" + email);
  };

  const onSendMessageClick = () => {
    window.open(`sms:${mobilePhone}`);
  };
  const redirectToEdit = () => {
    history.push(
      combineUrl(AppServerConfig.proxyURL, config.homepage, `/edit/${userName}`)
    );
  };
  const onEditClick = () => {
    const timer = setTimeout(() => redirectToEdit(), 500);
    fetchProfile(userName).finally(() => {
      clearTimeout(timer);
      if (
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/edit/${userName}`
        ) !== window.location.pathname
      )
        redirectToEdit();
    });
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
      .then(() => toastr.success(t("Translations:SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };

  const onEnableClick = (e) => {
    //onLoading(true);
    updateUserStatus(EmployeeStatus.Active, [id], isRefetchPeople)
      .then(() => toastr.success(t("Translations:SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };

  const onReassignDataClick = (e) => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/reassign/${userName}`
      )
    );
  };

  const onDeletePersonalDataClick = (e) => {
    toastr.success(t("Translations:SuccessDeletePersonalData"));
  };

  const onInviteAgainClick = () => {
    //onLoading(true);
    resendUserInvites([id])
      .then(() =>
        toastr.success(
          <Trans
            i18nKey="MessageEmailActivationInstuctionsSentOnEmail"
            ns="Home"
            t={t}
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
    const contextMenu = options.map((option) => {
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
            label: t("Common:EditButton"),
            "data-id": id,
            onClick: onEditClick,
          };
        case "change-password":
          return {
            key: option,
            label: t("Translations:PasswordChangeButton"),
            "data-id": id,
            onClick: toggleChangePasswordDialog,
          };
        case "change-email":
          return {
            key: option,
            label: t("Translations:EmailChangeButton"),
            "data-id": id,
            onClick: toggleChangeEmailDialog,
          };
        case "delete-self-profile":
          return {
            key: option,
            label: t("Translations:DeleteSelfProfile"),
            "data-id": id,
            onClick: toggleDeleteSelfProfileDialog,
          };
        case "disable":
          return {
            key: option,
            label: t("Translations:DisableUserButton"),
            "data-id": id,
            onClick: onDisableClick,
          };
        case "enable":
          return {
            key: option,
            label: t("Translations:EnableUserButton"),
            "data-id": id,
            onClick: onEnableClick,
          };
        case "reassign-data":
          return {
            key: option,
            label: t("Translations:ReassignData"),
            "data-id": id,
            onClick: onReassignDataClick,
          };
        case "delete-personal-data":
          return {
            key: option,
            label: t("Translations:RemoveData"),
            "data-id": id,
            onClick: onDeletePersonalDataClick,
          };
        case "delete-profile":
          return {
            key: option,
            label: t("Translations:DeleteSelfProfile"),
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

    return contextMenu;
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
        fetchProfile={fetchProfile}
      />
    </Row>
  );
};

export default withRouter(
  inject(({ auth, peopleStore }, { person }) => {
    return {
      isAdmin: auth.isAdmin,
      currentUserId: auth.userStore.user.id,
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
      fetchProfile: peopleStore.targetUserStore.getTargetUser,
    };
  })(observer(SimpleUserRow))
);
