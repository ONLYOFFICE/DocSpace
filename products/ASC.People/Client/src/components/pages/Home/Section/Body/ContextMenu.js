import React from "react";
import history from "@appserver/common/history";
import { Trans } from "react-i18next";

import { resendUserInvites } from "@appserver/common/api/people";
import { EmployeeStatus } from "@appserver/common/constants";
import config from "../../../../../../package.json";
import toastr from "@appserver/common/components/Toast/toastr";
import PeopleStore from "../../../../../store/PeopleStore";
import { observe } from "mobx";

const peopleStore = new PeopleStore();

const onEmailSentClick = (email) => () => window.open("mailto:" + email);

const onSendMessageClick = (mobilePhone) => () =>
  window.open(`sms:${mobilePhone}`);

const onEditClick = (userName) => () =>
  history.push(`${config.homepage}/edit/${userName}`);

const toggleChangePasswordDialog = (email) => {
  return () => {
    peopleStore.dialogStore.setDialogData({
      email,
    });
    peopleStore.dialogStore.setChangePasswordDialogVisible(true);
    console.log(
      "peopleStore.dialogStore.changePassword: ",
      peopleStore.dialogStore.changePassword
    );
  };
};

const toggleChangeEmailDialog = (email, id) => {
  return () => {
    peopleStore.dialogStore.setDialogData({
      email,
      id,
    });
  };
};

const toggleDeleteSelfProfileDialog = (email) => {
  return () => {
    peopleStore.dialogStore.closeDialogs();
    peopleStore.dialogStore.setDialogData({
      email,
    });
    peopleStore.dialogStore.setDeleteSelfProfileDialogVisible(true);
  };
};

const toggleDeleteProfileEverDialog = (id, displayName, userName) => {
  return () => {
    peopleStore.dialogStore.closeDialogs();
    peopleStore.dialogStore.setDialogData({
      id,
      displayName,
      userName,
    });
    peopleStore.dialogStore.setDeleteProfileDialogVisible(true);
  };
};

const onDisableClick = (id) => {
  return () => {
    //onLoading(true);

    peopleStore.usersStore.updateUserStatus(
      EmployeeStatus.Disabled,
      [id],
      true
    );
    toastr.success("SuccessChangeUserStatus");
    // toastr.success(t("SuccessChangeUserStatus"));
    // .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };
};

const onEnableClick = (id) => {
  return () => {
    //onLoading(true);
    peopleStore.usersStore
      .updateUserStatus(EmployeeStatus.Active, [id], true)
      .then(() => toastr.success(t("SuccessChangeUserStatus")))
      .catch((error) => toastr.error(error));
    //.finally(() => onLoading(false));
  };
};

const onReassignDataClick = (userName) => () =>
  history.push(`${homepage}/reassign/${userName}`);

const onDeletePersonalDataClick = () =>
  toastr.success("Context action: Delete personal data"); //TODO: Implement and add translation

const onInviteAgainClick = (id, email) => {
  return () => {
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
};

export const getUserContextOptions = (user, t) => {
  const { options, email, id, mobilePhone, userName, displayName } = user;
  return options.map((option) => {
    switch (option) {
      case "send-email":
        return {
          key: option,
          label: t("LblSendEmail"),
          "data-id": id,
          onClick: onEmailSentClick(email),
        };
      case "send-message":
        return {
          key: option,
          label: t("LblSendMessage"),
          "data-id": id,
          onClick: onSendMessageClick(mobilePhone),
        };
      case "separator":
        return { key: option, isSeparator: true };
      case "edit":
        return {
          key: option,
          label: t("EditButton"),
          "data-id": id,
          onClick: onEditClick(userName),
        };
      case "change-password":
        return {
          key: option,
          label: t("PasswordChangeButton"),
          "data-id": id,
          onClick: toggleChangePasswordDialog(email),
        };
      case "change-email":
        return {
          key: option,
          label: t("EmailChangeButton"),
          "data-id": id,
          onClick: toggleChangeEmailDialog(email),
        };
      case "delete-self-profile":
        return {
          key: option,
          label: t("DeleteSelfProfile"),
          "data-id": id,
          onClick: toggleDeleteSelfProfileDialog(email),
        };
      case "disable":
        return {
          key: option,
          label: t("DisableUserButton"),
          "data-id": id,
          onClick: onDisableClick(id),
        };
      case "enable":
        return {
          key: option,
          label: t("EnableUserButton"),
          "data-id": id,
          onClick: onEnableClick(id),
        };
      case "reassign-data":
        return {
          key: option,
          label: t("ReassignData"),
          "data-id": id,
          onClick: onReassignDataClick(userName),
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
          onClick: toggleDeleteProfileEverDialog(id, displayName, userName),
        };
      case "invite-again":
        return {
          key: option,
          label: t("LblInviteAgain"),
          "data-id": id,
          onClick: onInviteAgainClick(id, email),
        };
      default:
        break;
    }

    return undefined;
  });
};
