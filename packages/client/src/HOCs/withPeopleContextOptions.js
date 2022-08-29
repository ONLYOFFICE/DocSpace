import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

export default function withContextOptions(WrappedComponent) {
  const WithContextOptions = (props) => {
    const {
      isAdmin,
      item,

      getUserContextOptions,
    } = props;
    const { id, options, currentUserId } = item;

    const { t } = useTranslation([
      "People",
      "Common",
      "PeopleTranslations",
      "DeleteProfileEverDialog",
      "Translations",
    ]);

    const showContextMenu = options && options.length > 0;

    const contextOptionsProps =
      (isAdmin && showContextMenu) || (showContextMenu && id === currentUserId)
        ? {
            contextOptions: getUserContextOptions(t, options, item),
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

    const {
      dialogStore,
      targetUserStore,
      usersStore,
      contextOptionsStore,
    } = peopleStore;
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

    const { getUserContextOptions } = contextOptionsStore;

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
      getUserContextOptions,
    };
  })(observer(WithContextOptions));
}
