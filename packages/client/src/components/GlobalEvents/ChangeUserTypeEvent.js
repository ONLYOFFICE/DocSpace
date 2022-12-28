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
  const { t } = useTranslation(["ChangeUserTypeDialog", "Common"]);

  const [isRequestRunning, setIsRequestRunning] = useState(false);

  useEffect(() => {
    setVisible(true);

    return () => {
      setVisible(false);
    };
  }, [peopleDialogData]);

  const onChangeUserType = async () => {
    setIsRequestRunning(true);
    updateUserType(toType, userIDs, peopleFilter, fromType)
      .then(() => toastr.success(t("SuccessChangeUserType")))
      .catch((err) => toastr.error(err))
      .finally(() => {
        setIsRequestRunning(false);
        onClose();
      });
  };

  const onClose = () => {
    setVisible(false);
  };

  const onCloseAction = async () => {
    if (!isRequestRunning) {
      await getUsersList(peopleFilter);
      onClose();
    }
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

  const { toType, fromType, userIDs } = peopleDialogData;

  const firstType =
    fromType.length === 1 && fromType[0] ? getType(fromType[0]) : null;
  const secondType = getType(toType);

  return (
    <ChangeUserTypeDialog
      visible={visible}
      firstType={firstType}
      secondType={secondType}
      onCloseAction={onCloseAction}
      onChangeUserType={onChangeUserType}
      isRequestRunning={isRequestRunning}
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
