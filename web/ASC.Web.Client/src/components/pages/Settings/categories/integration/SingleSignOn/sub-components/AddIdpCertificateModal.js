import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import StyledModalDialog from "../styled-containers/StyledModalDialog";
import Text from "@appserver/components/text";
import TextArea from "@appserver/components/textarea";

import { addArguments } from "../../../../utils";

const AddIdpCertificateModal = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    onCloseIdpModal,
    addCertificateToForm,
    idpIsModalVisible,
    onTextInputChange,
    idpCertificate,
  } = props;

  const onSubmit = addArguments(addCertificateToForm, "idp");

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={onCloseIdpModal}
      visible={idpIsModalVisible}
    >
      <ModalDialog.Header>{t("NewCertificate")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text isBold className="text-area-label" noSelect>
          {t("OpenCertificate")}
        </Text>

        <TextArea
          className="text-area"
          name="idpCertificate"
          onChange={onTextInputChange}
          value={idpCertificate}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex" marginProp="12px 0 4px 0">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={onSubmit}
            primary
            size="small"
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={onCloseIdpModal}
            size="small"
          />
        </Box>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ ssoStore }) => {
  const {
    onCloseIdpModal,
    addCertificateToForm,
    idpIsModalVisible,
    onTextInputChange,
    idpCertificate,
  } = ssoStore;

  return {
    onCloseIdpModal,
    addCertificateToForm,
    idpIsModalVisible,
    onTextInputChange,
    idpCertificate,
  };
})(observer(AddIdpCertificateModal));
