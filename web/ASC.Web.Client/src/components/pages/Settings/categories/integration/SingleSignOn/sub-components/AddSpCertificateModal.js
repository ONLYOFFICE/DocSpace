import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import TextArea from "@appserver/components/textarea";

import ModalComboBox from "./ModalComboBox";
import StyledModalDialog from "../styled-containers/StyledModalDialog";
import { addArguments } from "../../../../utils";

const AddSpCertificateModal = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    onCloseModal,
    addCertificateToForm,
    sp_isModalVisible,
    generateCertificate,
    onTextInputChange,
    sp_certificate,
    sp_privateKey,
  } = props;

  const onClose = addArguments(onCloseModal, "sp_isModalVisible");
  const onSubmit = addArguments(addCertificateToForm, "sp");

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={onClose}
      visible={sp_isModalVisible}
    >
      <ModalDialog.Header>{t("NewCertificate")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Box marginProp="4px 0 15px 0">
          <Link
            className="generate"
            isHovered
            onClick={generateCertificate}
            type="action"
          >
            {t("GenerateCertificate")}
          </Link>
        </Box>

        <Text isBold className="text-area-label" noSelect>
          {t("OpenCertificate")}
        </Text>

        <TextArea
          className="text-area"
          name="sp_certificate"
          onChange={onTextInputChange}
          value={sp_certificate}
        />

        <Text isBold className="text-area-label" noSelect>
          {t("PrivateKey")}
        </Text>

        <TextArea
          className="text-area"
          name="sp_privateKey"
          onChange={onTextInputChange}
          value={sp_privateKey}
        />

        <ModalComboBox />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={onSubmit}
            primary
            size="small"
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={onClose}
            size="small"
          />
        </Box>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ ssoStore }) => {
  const {
    onCloseModal,
    addCertificateToForm,
    sp_isModalVisible,
    generateCertificate,
    onTextInputChange,
    sp_certificate,
    sp_privateKey,
  } = ssoStore;

  return {
    onCloseModal,
    addCertificateToForm,
    sp_isModalVisible,
    generateCertificate,
    onTextInputChange,
    sp_certificate,
    sp_privateKey,
  };
})(observer(AddSpCertificateModal));
