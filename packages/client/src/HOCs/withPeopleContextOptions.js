import React from "react";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig, EmployeeStatus } from "@docspace/common/constants";
import { resendUserInvites } from "@docspace/common/api/people"; //TODO: Move to store action
import config from "PACKAGE_FILE";
import { Trans, useTranslation } from "react-i18next";
import toastr from "client/toastr";

export default function withContextOptions(WrappedComponent) {
  const WithContextOptions = (props) => {
    const {
      isAdmin,
      item,
      history,
      setDialogData,
      closeDialogs,
      setDeleteSelfProfileDialogVisible,
      setChangePasswordDialogVisible,
      setChangeEmailDialogVisible,
      updateUserStatus,
      setDeleteProfileDialogVisible,
      fetchProfile,
    } = props;
    const {
      id,
      displayName,
      options,
      userName,
      email,
      mobilePhone,
      currentUserId,
    } = item;

    const isRefetchPeople = true; //TODO: why always true?

    const { t } = useTranslation(["People", "Common", "PeopleTranslations"]);

    const onEmailSentClick = () => {
      window.open("mailto:" + email);
    };

    const onSendMessageClick = () => {
      window.open(`sms:${mobilePhone}`);
    };

    const redirectToEdit = () => {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/accounts/edit/${userName}`
        )
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
            `/accounts/edit/${userName}`
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

    const toggleDeleteSelfProfileDialog = () => {
      closeDialogs();

      setDialogData({
        email,
      });

      setDeleteSelfProfileDialogVisible(true);
    };

    const toggleDeleteProfileEverDialog = () => {
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
        .then(() =>
          toastr.success(t("PeopleTranslations:SuccessChangeUserStatus"))
        )
        .catch((error) => toastr.error(error));
      //.finally(() => onLoading(false));
    };

    const onEnableClick = () => {
      //onLoading(true);
      updateUserStatus(EmployeeStatus.Active, [id], isRefetchPeople)
        .then(() =>
          toastr.success(t("PeopleTranslations:SuccessChangeUserStatus"))
        )
        .catch((error) => toastr.error(error));
      //.finally(() => onLoading(false));
    };

    const onReassignDataClick = () => {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/reassign/${userName}`
        )
      );
    };

    const onDeletePersonalDataClick = () => {
      toastr.success(t("PeopleTranslations:SuccessDeletePersonalData"));
    };

    const onInviteAgainClick = () => {
      //onLoading(true);
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
      //.finally(() => onLoading(false));
    };

    const getUserContextOptions = () => {
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
              label: t("PeopleTranslations:PasswordChangeButton"),
              "data-id": id,
              onClick: toggleChangePasswordDialog,
            };
          case "change-email":
            return {
              key: option,
              label: t("PeopleTranslations:EmailChangeButton"),
              "data-id": id,
              onClick: toggleChangeEmailDialog,
            };
          case "delete-self-profile":
            return {
              key: option,
              label: t("PeopleTranslations:DeleteSelfProfile"),
              "data-id": id,
              onClick: toggleDeleteSelfProfileDialog,
            };
          case "disable":
            return {
              key: option,
              label: t("PeopleTranslations:DisableUserButton"),
              "data-id": id,
              onClick: onDisableClick,
            };
          case "enable":
            return {
              key: option,
              label: t("PeopleTranslations:EnableUserButton"),
              "data-id": id,
              onClick: onEnableClick,
            };
          case "reassign-data":
            return {
              key: option,
              label: t("PeopleTranslations:ReassignData"),
              "data-id": id,
              onClick: onReassignDataClick,
            };
          case "delete-personal-data":
            return {
              key: option,
              label: t("PeopleTranslations:RemoveData"),
              "data-id": id,
              onClick: onDeletePersonalDataClick,
            };
          case "delete-profile":
            return {
              key: option,
              label: t("PeopleTranslations:DeleteSelfProfile"),
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

    const contextOptionsProps =
      (isAdmin && showContextMenu) || (showContextMenu && id === currentUserId)
        ? {
            contextOptions: getUserContextOptions(),
          }
        : {};

    return (
      <WrappedComponent
        t={t}
        contextOptionsProps={contextOptionsProps}
        {...props}
      />
    );
  };

  return inject(({ auth, peopleStore }) => {
    const { isAdmin } = auth;

    const { dialogStore, targetUserStore, usersStore } = peopleStore;
    const { getTargetUser } = targetUserStore;
    const { updateUserStatus } = usersStore;
    const {
      setDialogData,
      closeDialogs,
      setDeleteSelfProfileDialogVisible,
      setChangePasswordDialogVisible,
      setChangeEmailDialogVisible,
      setDeleteProfileDialogVisible,
    } = dialogStore;

    return {
      isAdmin,
      fetchProfile: getTargetUser,
      setDialogData,
      closeDialogs,
      setDeleteSelfProfileDialogVisible,
      setChangePasswordDialogVisible,
      setChangeEmailDialogVisible,
      updateUserStatus,
      setDeleteProfileDialogVisible,
    };
  })(observer(WithContextOptions));
}
