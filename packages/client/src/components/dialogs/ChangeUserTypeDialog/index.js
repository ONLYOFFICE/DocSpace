import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";

const ChangeUserTypeDialog = ({
  t,
  visible,
  setVisible,
  peopleDialogData,
  peopleFilter,
  updateUserType,
  getUsersList,
}) => {
  const [isRequestRunning, setIsRequestRunning] = useState(false);

  console.log(peopleDialogData);
  console.log(peopleFilter);

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
    <ModalDialog
      visible={visible}
      onClose={onCloseAction}
      displayType="modal"
      autoMaxHeight
    >
      <ModalDialog.Header>{t("ChangeUserTypeHeader")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>
          {firstType ? (
            <Trans
              i18nKey="ChangeUserTypeMessage"
              ns="ChangeUserTypeDialog"
              t={t}
            >
              Users with the <b>'{{ firstType }}'</b> type will be moved to{" "}
              <b>'{{ secondType }}'</b> type.
            </Trans>
          ) : (
            <Trans
              i18nKey="ChangeUserTypeMessageMulti"
              ns="ChangeUserTypeDialog"
              t={t}
            >
              The selected users will be moved to <b>'{{ secondType }}'</b>{" "}
              type.
            </Trans>
          )}{" "}
          {t("ChangeUserTypeMessageWarning")}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="change-user-type-modal_submit"
          label={t("ChangeUserTypeButton")}
          size="normal"
          scale
          primary
          onClick={onChangeUserType}
          isLoading={isRequestRunning}
          isDisabled={!userIDs.length}
        />
        <Button
          id="change-user-type-modal_cancel"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onCloseAction}
          isDisabled={isRequestRunning}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ dialogsStore, peopleStore }) => {
  const {
    changeUserTypeDialogVisible: visible,
    setChangeUserTypeDialogVisible: setVisible,
  } = dialogsStore;

  const { data: peopleDialogData } = peopleStore.dialogStore;
  const { filter: peopleFilter } = peopleStore.filterStore;
  const { updateUserType, getUsersList } = peopleStore.usersStore;

  return {
    visible,
    setVisible,
    peopleDialogData,
    peopleFilter,
    updateUserType,
    getUsersList,
  };
})(
  withTranslation(["ChangeUserTypeDialog", "People", "Common"])(
    withRouter(observer(ChangeUserTypeDialog))
  )
);
