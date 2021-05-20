import { inject, observer } from "mobx-react";
import React from "react";
import {
  ChangeEmailDialog,
  ChangePasswordDialog,
  DeleteSelfProfileDialog,
  DeleteProfileEverDialog,
} from "../../../../components/dialogs";

const Dialogs = ({
  changeEmail,
  changePassword,
  deleteSelfProfile,
  deleteProfileEver,
  data,
  closeDialogs,
  filter,
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
    </>
  );
};

export default inject(({ peopleStore }) => ({
  changeEmail: peopleStore.dialogStore.changeEmail,
  changePassword: peopleStore.dialogStore.changePassword,
  deleteSelfProfile: peopleStore.dialogStore.deleteSelfProfile,
  deleteProfileEver: peopleStore.dialogStore.deleteProfileEver,
  data: peopleStore.dialogStore.data,
  closeDialogs: peopleStore.dialogStore.closeDialogs,
}))(observer(Dialogs));
