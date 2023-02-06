import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { ChangeUserTypeDialog } from "../dialogs";
import toastr from "@docspace/components/toast/toastr";

const ChangeUserTypeEvent = ({
  setVisible,
  visible,
  peopleDialogData,
  peopleFilter,
  updateUserType,
  getUsersList,
}) => {
  const {
    toType,
    fromType,
    userIDs,
    successCallback,
    abortCallback,
  } = peopleDialogData;
  const { t } = useTranslation(["ChangeUserTypeDialog", "Common"]);

  useEffect(() => {
    if (!peopleDialogData.toType) return;

    setVisible(true);

    return () => {
      setVisible(false);
    };
  }, [peopleDialogData.toType]);

  const onChangeUserType = () => {
    onClose();
    updateUserType(toType, userIDs, peopleFilter, fromType)
      .then(() => {
        toastr.success(t("SuccessChangeUserType"));

        successCallback && successCallback();
      })
      .catch((err) => {
        abortCallback && abortCallback();
        toastr.error(err);
      });
  };

  const onClose = () => {
    setVisible(false);
  };

  const onCloseAction = async () => {
    await getUsersList(peopleFilter);
    abortCallback && abortCallback();
    onClose();
  };

  const getType = (type) => {
    switch (type) {
      case "admin":
        return t("Common:DocSpaceAdmin");
      case "manager":
        return t("Common:RoomAdmin");
      case "user":
      default:
        return t("Common:User");
    }
  };

  const firstType =
    fromType?.length === 1 && fromType[0] ? getType(fromType[0]) : null;
  const secondType = getType(toType);

  return (
    <ChangeUserTypeDialog
      visible={visible}
      firstType={firstType}
      secondType={secondType}
      onCloseAction={onCloseAction}
      onChangeUserType={onChangeUserType}
    />
  );
};

export default inject(({ dialogsStore, peopleStore }) => {
  const {
    changeUserTypeDialogVisible: visible,
    setChangeUserTypeDialogVisible: setVisible,
  } = dialogsStore;

  const { dialogStore, filterStore, usersStore } = peopleStore;

  const { data: peopleDialogData } = dialogStore;
  const { filter: peopleFilter } = filterStore;
  const { updateUserType, getUsersList } = usersStore;

  return {
    visible,
    setVisible,
    peopleDialogData,
    peopleFilter,
    updateUserType,
    getUsersList,
  };
})(observer(ChangeUserTypeEvent));
