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

const AddSpCertificateModal = (props) => {
  const { t, ready } = useTranslation(["SingleSignOn", "Common"]);
  const {
    onCloseSpModal,
    addSpCertificate,
    spIsModalVisible,
    generateCertificate,
    onTextInputChange,
    spCertificate,
    spPrivateKey,
    isGeneratedCertificate,
  } = props;

  const onGenerate = () => {
    if (isGeneratedCertificate) return;
    generateCertificate();
  };

  return (
    <StyledModalDialog
      zIndex={310}
      isLoading={!ready}
      autoMaxHeight
      onClose={onCloseSpModal}
      visible={spIsModalVisible}
    >
      <ModalDialog.Header>{t("NewCertificate")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Box marginProp="4px 0 15px 0">
          <Link
            className="generate"
            isHovered
            onClick={onGenerate}
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
          name="spCertificate"
          onChange={onTextInputChange}
          value={spCertificate}
          isDisabled={isGeneratedCertificate}
        />

        <Text isBold className="text-area-label" noSelect>
          {t("PrivateKey")}
        </Text>

        <TextArea
          className="text-area"
          name="spPrivateKey"
          onChange={onTextInputChange}
          value={spPrivateKey}
          isDisabled={isGeneratedCertificate}
        />

        <ModalComboBox isDisabled={isGeneratedCertificate} />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={addSpCertificate}
            primary
            size="small"
            isDisabled={isGeneratedCertificate}
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={onCloseSpModal}
            size="small"
            isDisabled={isGeneratedCertificate}
          />
        </Box>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ ssoStore }) => {
  const {
    onCloseSpModal,
    addSpCertificate,
    spIsModalVisible,
    generateCertificate,
    onTextInputChange,
    spCertificate,
    spPrivateKey,
    isGeneratedCertificate,
  } = ssoStore;

  return {
    onCloseSpModal,
    addSpCertificate,
    spIsModalVisible,
    generateCertificate,
    onTextInputChange,
    spCertificate,
    spPrivateKey,
    isGeneratedCertificate,
  };
})(observer(AddSpCertificateModal));
