import React from "react";
import { inject, observer } from "mobx-react";

import {
  ChangeEmailDialog,
  ChangePasswordDialog,
  ChangePortalOwnerDialog,
  DeleteSelfProfileDialog,
  DeleteProfileEverDialog,
  ChangeUserTypeDialog,
  ChangeUserStatusDialog,
  SendInviteDialog,
  DeleteUsersDialog,
  ChangeNameDialog,
  ResetApplicationDialog,
} from "SRC_DIR/components/dialogs";

const Dialogs = ({
  changeOwner,
  deleteSelfProfile,
  deleteProfileEver,
  data,
  closeDialogs,
  changeUserTypeDialogVisible,
  changeUserStatusDialogVisible,

  sendInviteDialogVisible,
  deleteDialogVisible,
  resetAuthDialogVisible,

  changeEmailVisible,
  setChangeEmailVisible,

  changePasswordVisible,
  setChangePasswordVisible,

  changeNameVisible,
  setChangeNameVisible,

  profile,
  resetTfaApp,
}) => {
  return (
    <>
      {changeEmailVisible && (
        <ChangeEmailDialog
          visible={changeEmailVisible}
          onClose={() => setChangeEmailVisible(false)}
          user={profile}
          fromList
        />
      )}
      {changePasswordVisible && (
        <ChangePasswordDialog
          visible={changePasswordVisible}
          onClose={() => setChangePasswordVisible(false)}
          email={data.email}
        />
      )}
      {changeOwner && (
        <ChangePortalOwnerDialog visible={changeOwner} onClose={closeDialogs} />
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
      {changeUserTypeDialogVisible && (
        <ChangeUserTypeDialog
          visible={changeUserTypeDialogVisible}
          onClose={closeDialogs}
          {...data}
        />
      )}
      {changeUserStatusDialogVisible && (
        <ChangeUserStatusDialog
          visible={changeUserStatusDialogVisible}
          onClose={closeDialogs}
          {...data}
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

      {changeNameVisible && (
        <ChangeNameDialog
          visible={changeNameVisible}
          onClose={() => setChangeNameVisible(false)}
          profile={profile}
          fromList
        />
      )}

      {resetAuthDialogVisible && (
        <ResetApplicationDialog
          visible={resetAuthDialogVisible}
          onClose={closeDialogs}
          resetTfaApp={resetTfaApp}
          id={data}
        />
      )}
    </>
  );
};

export default inject(({ auth, peopleStore }) => {
  const {
    changeOwner,
    deleteSelfProfile,
    deleteProfileEver,
    data,
    closeDialogs,

    changeUserTypeDialogVisible,
    guestDialogVisible,
    changeUserStatusDialogVisible,
    disableDialogVisible,
    sendInviteDialogVisible,
    deleteDialogVisible,
    resetAuthDialogVisible,
  } = peopleStore.dialogStore;

  const { user: profile } = auth.userStore;

  const {
    changeEmailVisible,
    setChangeEmailVisible,
    changeNameVisible,
    changePasswordVisible,
    setChangePasswordVisible,
    setChangeNameVisible,
  } = peopleStore.targetUserStore;

  const { tfaStore } = auth;

  const { unlinkApp: resetTfaApp } = tfaStore;

  return {
    changeOwner,
    deleteSelfProfile,
    deleteProfileEver,
    data,
    closeDialogs,

    changeUserTypeDialogVisible,
    guestDialogVisible,
    changeUserStatusDialogVisible,
    disableDialogVisible,
    sendInviteDialogVisible,
    deleteDialogVisible,
    resetAuthDialogVisible,

    changeEmailVisible,
    setChangeEmailVisible,

    changePasswordVisible,
    setChangePasswordVisible,

    changeNameVisible,
    setChangeNameVisible,

    profile,

    resetTfaApp,
  };
})(observer(Dialogs));
