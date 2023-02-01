import React from "react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation, Trans } from "react-i18next";

const ChangeUserTypeDialog = ({
  t,
  visible,
  firstType,
  secondType,
  onCloseAction,
  onChangeUserType,
  isRequestRunning,
}) => {
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
          //isDisabled={!userIDs.length}
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

export default withTranslation(["ChangeUserTypeDialog", "People", "Common"])(
  ChangeUserTypeDialog
);
