import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import TextArea from "@docspace/components/textarea";

import ModalComboBox from "./ModalComboBox";
import StyledModalDialog from "../styled-containers/StyledModalDialog";

const AddSpCertificateModal = (props) => {
  const { t, ready } = useTranslation(["SingleSignOn", "Common"]);
  const {
    closeSpModal,
    addSpCertificate,
    spIsModalVisible,
    generateCertificate,
    setInput,
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
      onClose={closeSpModal}
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
          onChange={setInput}
          value={spCertificate}
          isDisabled={isGeneratedCertificate}
        />

        <Text isBold className="text-area-label" noSelect>
          {t("PrivateKey")}
        </Text>

        <TextArea
          className="text-area"
          name="spPrivateKey"
          onChange={setInput}
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
            isDisabled={
              isGeneratedCertificate || !spCertificate || !spPrivateKey
            }
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={closeSpModal}
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
    closeSpModal,
    addSpCertificate,
    spIsModalVisible,
    generateCertificate,
    setInput,
    spCertificate,
    spPrivateKey,
    isGeneratedCertificate,
  } = ssoStore;

  return {
    closeSpModal,
    addSpCertificate,
    spIsModalVisible,
    generateCertificate,
    setInput,
    spCertificate,
    spPrivateKey,
    isGeneratedCertificate,
  };
})(observer(AddSpCertificateModal));
