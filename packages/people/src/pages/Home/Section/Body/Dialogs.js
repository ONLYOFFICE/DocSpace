import { inject, observer } from "mobx-react";
import React from "react";
import {
  ChangeEmailDialog,
  ChangePasswordDialog,
  DeleteSelfProfileDialog,
  DeleteProfileEverDialog,
  ChangeUserTypeDialog,
  ChangeUserStatusDialog,
  SendInviteDialog,
  DeleteUsersDialog,
  InviteDialog,
} from "../../../../components/dialogs";
import { EmployeeType, EmployeeStatus } from "@docspace/common/constants";

const Dialogs = ({
  changeEmail,
  changePassword,
  deleteSelfProfile,
  deleteProfileEver,
  data,
  closeDialogs,
  employeeDialogVisible,
  guestDialogVisible,
  activeDialogVisible,
  disableDialogVisible,
  sendInviteDialogVisible,
  deleteDialogVisible,
  invitationDialogVisible,
}) => {
  return (
    <>
      {changeEmail && (
        <ChangeEmailDialog
          visible={changeEmail}
          onClose={closeDialogs}
          user={data}
        />
      )}
      {changePassword && (
        <ChangePasswordDialog
          visible={changePassword}
          onClose={closeDialogs}
          email={data.email}
        />
      )}
      {deleteSelfProfile && (
        <DeleteSelfProfileDialog
          visible={deleteSelfProfile}
          onClose={closeDialogs}
          email={data.email}
        />
      )}
      {deleteProfileEver && (
        <DeleteProfileEverDialog
          visible={deleteProfileEver}
          onClose={closeDialogs}
          user={data}
        />
      )}
      {employeeDialogVisible && (
        <ChangeUserTypeDialog
          visible={employeeDialogVisible}
          onClose={closeDialogs}
          userType={EmployeeType.User}
        />
      )}
      {guestDialogVisible && (
        <ChangeUserTypeDialog
          visible={guestDialogVisible}
          onClose={closeDialogs}
          userType={EmployeeType.Guest}
        />
      )}
      {activeDialogVisible && (
        <ChangeUserStatusDialog
          visible={activeDialogVisible}
          onClose={closeDialogs}
          userStatus={EmployeeStatus.Active}
        />
      )}
      {disableDialogVisible && (
        <ChangeUserStatusDialog
          visible={disableDialogVisible}
          onClose={closeDialogs}
          userStatus={EmployeeStatus.Disabled}
        />
      )}
      {sendInviteDialogVisible && (
        <SendInviteDialog
          visible={sendInviteDialogVisible}
          onClose={closeDialogs}
        />
      )}

      {deleteDialogVisible && (
        <DeleteUsersDialog
          visible={deleteDialogVisible}
          onClose={closeDialogs}
        />
      )}
      {invitationDialogVisible && (
        <InviteDialog
          visible={invitationDialogVisible}
          onClose={closeDialogs}
          onCloseButton={closeDialogs}
        />
      )}
    </>
  );
};

export default inject(({ peopleStore }) => {
  const {
    changeEmail,
    changePassword,
    deleteSelfProfile,
    deleteProfileEver,
    data,
    closeDialogs,

    employeeDialogVisible,
    guestDialogVisible,
    activeDialogVisible,
    disableDialogVisible,
    sendInviteDialogVisible,
    deleteDialogVisible,
    invitationDialogVisible,
  } = peopleStore.dialogStore;

  return {
    changeEmail,
    changePassword,
    deleteSelfProfile,
    deleteProfileEver,
    data,
    closeDialogs,

    employeeDialogVisible,
    guestDialogVisible,
    activeDialogVisible,
    disableDialogVisible,
    sendInviteDialogVisible,
    deleteDialogVisible,
    invitationDialogVisible,
  };
})(observer(Dialogs));
