import { makeAutoObservable } from "mobx";
import { Trans } from "react-i18next";

import config from "PACKAGE_FILE";

import toastr from "client/toastr";

import history from "@docspace/common/history";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig, EmployeeStatus } from "@docspace/common/constants";
import { resendUserInvites } from "@docspace/common/api/people";

const { proxyURL } = AppServerConfig;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, "/");

const PROFILE_SELF_URL = combineUrl(PROXY_HOMEPAGE_URL, "/accounts/view/@self");

class AccountsContextOptionsStore {
  authStore = null;
  dialogStore = null;
  targetUserStore = null;
  usersStore = null;
  selectionStore = null;
  infoPanelStore = null;

  constructor(
    authStore,
    dialogStore,
    targetUserStore,
    usersStore,
    selectionStore,
    infoPanelStore
  ) {
    makeAutoObservable(this);
    this.authStore = authStore;
    this.dialogStore = dialogStore;
    this.targetUserStore = targetUserStore;
    this.usersStore = usersStore;
    this.selectionStore = selectionStore;
    this.infoPanelStore = infoPanelStore;
  }

  getUserContextOptions = (t, options, item) => {
    const contextMenu = options.map((option) => {
      switch (option) {
        case "separator-1":
          return { key: option, isSeparator: true };
        case "separator-2":
          return { key: option, isSeparator: true };

        case "profile":
          return {
            key: option,
            icon: "/static/images/profile.react.svg",
            label: t("Common:Profile"),
            onClick: this.onProfileClick,
          };

        case "change-name":
          return {
            key: option,
            icon: "images/pencil.react.svg",
            label: t("PeopleTranslations:NameChangeButton"),
            onClick: () => this.toggleChangeNameDialog(item),
          };
        case "change-email":
          return {
            key: option,
            icon: "images/change.mail.react.svg",
            label: t("PeopleTranslations:EmailChangeButton"),
            onClick: () => this.toggleChangeEmailDialog(item),
          };
        case "change-password":
          return {
            key: option,
            icon: "images/change.security.react.svg",
            label: t("PeopleTranslations:PasswordChangeButton"),
            onClick: () => this.toggleChangePasswordDialog(item),
          };
        case "change-owner":
          return {
            key: option,
            icon: "/static/images/refresh.react.svg",
            label: t("Translations:OwnerChange"),
            onClick: () => this.toggleChangeOwnerDialog(item),
          };

        case "enable":
          return {
            key: option,
            icon: "images/enable.react.svg",
            label: t("PeopleTranslations:EnableUserButton"),
            onClick: () => this.onEnableClick(t, item),
          };
        case "disable":
          return {
            key: option,
            icon: "images/remove.react.svg",
            label: t("PeopleTranslations:DisableUserButton"),
            onClick: () => this.onDisableClick(t, item),
          };

        case "reassign-data":
          return {
            key: option,
            icon: "images/ressing_data.react.svg",
            label: t("PeopleTranslations:ReassignData"),
            onClick: () => this.onReassignDataClick(item),
          };
        case "delete-personal-data":
          return {
            key: option,
            icon: "images/del_data.react.svg",
            label: t("PeopleTranslations:RemoveData"),
            onClick: () => this.onDeletePersonalDataClick(t, item),
          };
        case "delete-user":
          return {
            key: option,
            icon: "images/trash.react.svg",
            label: t("DeleteProfileEverDialog:DeleteUser"),
            onClick: () => this.toggleDeleteProfileEverDialog(item),
          };

        case "details":
          return {
            key: option,
            icon: "images/info.react.svg",
            label: t("PeopleTranslations:Details"),
            onClick: this.onDetailsClick,
          };

        case "invite-again":
          return {
            key: option,
            icon: "/static/images/invite.again.react.svg",
            label: t("LblInviteAgain"),
            onClick: () => onInviteAgainClick(t, item),
          };
        case "reset-auth":
          return {
            key: option,
            icon: "images/restore.auth.react.svg",
            label: t("PeopleTranslations:ResetAuth"),
            onClick: () => onResetAuth(item),
          };
        default:
          break;
      }

      return undefined;
    });

    return contextMenu;
  };

  onProfileClick = () => {
    history.push(PROFILE_SELF_URL);
  };

  toggleChangeNameDialog = (item) => {
    const { setDialogData, setChangeNameDialogVisible } = this.dialogStore;
    const { id, firstName, lastName } = item;

    setDialogData({ id, firstName, lastName });

    setChangeNameDialogVisible(true);
  };

  toggleChangeEmailDialog = (item) => {
    const { setDialogData, setChangeEmailDialogVisible } = this.dialogStore;
    const { id, email } = item;

    setDialogData({
      email,
      id,
    });

    setChangeEmailDialogVisible(true);
  };

  toggleChangePasswordDialog = (item) => {
    const { setDialogData, setChangePasswordDialogVisible } = this.dialogStore;
    const { email } = item;
    setDialogData({
      email,
    });

    setChangePasswordDialogVisible(true);
  };

  toggleChangeOwnerDialog = () => {
    const { setChangeOwnerDialogVisible } = this.dialogStore;

    setChangeOwnerDialogVisible(true);
  };

  onEnableClick = (t, item) => {
    const { id } = item;
    const { updateUserStatus } = this.usersStore;

    updateUserStatus(EmployeeStatus.Active, [id], true)
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessChangeUserStatus"))
      )
      .catch((error) => toastr.error(error));
  };

  onDisableClick = (t, item) => {
    const { id } = item;
    const { updateUserStatus } = this.usersStore;

    updateUserStatus(EmployeeStatus.Disabled, [id], true)
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessChangeUserStatus"))
      )
      .catch((error) => toastr.error(error));
  };

  onReassignDataClick = (item) => {
    const { userName } = item;

    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/reassign/${userName}`
      )
    );
  };

  onDeletePersonalDataClick = (t, item) => {
    toastr.success(t("PeopleTranslations:SuccessDeletePersonalData"));
  };

  toggleDeleteProfileEverDialog = (item) => {
    const { setDialogData, setDeleteProfileDialogVisible } = this.dialogStore;
    const { id, displayName, userName } = item;

    closeDialogs();

    setDialogData({
      id,
      displayName,
      userName,
    });

    setDeleteProfileDialogVisible(true);
  };

  onDetailsClick = () => {
    const { setVisible } = this.infoPanelStore;

    setVisible();
  };

  onInviteAgainClick = (t, item) => {
    const { id, email } = item;
    resendUserInvites([id])
      .then(() =>
        toastr.success(
          <Trans
            i18nKey="MessageEmailActivationInstuctionsSentOnEmail"
            ns="People"
            t={t}
          >
            The email activation instructions have been sent to the
            <strong>{{ email: email }}</strong> email address
          </Trans>
        )
      )
      .catch((error) => toastr.error(error));
  };
}

const onResetAuth = (item) => {
  console.log(item);
};

export default AccountsContextOptionsStore;
