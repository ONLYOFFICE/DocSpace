import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation, Trans } from "react-i18next";

import { ChangeUserTypeDialog } from "../dialogs";
import toastr from "@docspace/components/toast/toastr";
import Link from "@docspace/components/link";
import { combineUrl } from "@docspace/common/utils";
import { useNavigate } from "react-router-dom";

const ChangeUserTypeEvent = ({
  setVisible,
  visible,
  peopleDialogData,
  peopleFilter,
  updateUserType,
  getUsersList,
  onClose,
}) => {
  const { toType, fromType, userIDs, successCallback, abortCallback } =
    peopleDialogData;
  const { t } = useTranslation(["ChangeUserTypeDialog", "Common", "Payments"]);

  const onKeyUpHandler = (e) => {
    if (e.keyCode === 27) onCloseAction();
    if (e.keyCode === 13) onChangeUserType();
  };
  useEffect(() => {
    document.addEventListener("keyup", onKeyUpHandler, false);

    return () => {
      document.removeEventListener("keyup", onKeyUpHandler, false);
    };
  }, []);

  useEffect(() => {
    if (!peopleDialogData.toType) return;

    setVisible(true);

    return () => {
      setVisible(false);
    };
  }, [peopleDialogData]);

  const onClickPayments = () => {
    const paymentPageUrl = combineUrl(
      "/portal-settings",
      "/payments/portal-payments"
    );

    toastr.clear();
    navigate(paymentPageUrl);
  };

  const onChangeUserType = () => {
    onClosePanel();
    updateUserType(toType, userIDs, peopleFilter, fromType)
      .then((users) => {
        toastr.success(t("SuccessChangeUserType"));

        successCallback && successCallback(users);
      })
      .catch((err) => {
        toastr.error(
          <Trans t={t} i18nKey="QuotaPaidUserLimitError" ns="Payments">
            The paid user limit has been reached.
            <Link color="#5387AD" isHovered={true} onClick={onClickPayments}>
              {t("Common:PaymentsTitle")}
            </Link>
          </Trans>,
          false,
          0,
          true,
          true
        );

        abortCallback && abortCallback();
      });
  };

  const onClosePanel = () => {
    setVisible(false);
    onClose();
  };

  const onCloseAction = async () => {
    await getUsersList(peopleFilter);
    abortCallback && abortCallback();
    onClosePanel();
  };

  const getType = (type) => {
    switch (type) {
      case "admin":
        return t("Common:DocSpaceAdmin");
      case "manager":
        return t("Common:RoomAdmin");
      case "collaborator":
        return t("Common:PowerUser");
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
