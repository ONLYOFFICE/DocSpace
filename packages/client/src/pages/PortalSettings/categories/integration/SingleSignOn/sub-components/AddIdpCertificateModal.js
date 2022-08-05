import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import StyledModalDialog from "../styled-containers/StyledModalDialog";
import Text from "@docspace/components/text";
import TextArea from "@docspace/components/textarea";

const AddIdpCertificateModal = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    closeIdpModal,
    addIdpCertificate,
    idpIsModalVisible,
    setInput,
    idpCertificate,
    isCertificateLoading,
  } = props;

  return (
    <StyledModalDialog
      autoMaxHeight
      autoMaxWidth
      displayType="modal"
      onClose={closeIdpModal}
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
          onChange={setInput}
          value={idpCertificate}
          placeholder={t("PlaceholderCert")}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          label={t("Common:OKButton")}
          onClick={addIdpCertificate}
          primary
          scale
          isLoading={isCertificateLoading}
          isDisabled={!idpCertificate}
          size="normalTouchscreen"
        />
        <Button
          label={t("Common:CancelButton")}
          onClick={closeIdpModal}
          size="normalTouchscreen"
          scale
          isDisabled={isCertificateLoading}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ ssoStore }) => {
  const {
    closeIdpModal,
    addIdpCertificate,
    idpIsModalVisible,
    setInput,
    idpCertificate,
    isCertificateLoading,
  } = ssoStore;

  return {
    closeIdpModal,
    addIdpCertificate,
    idpIsModalVisible,
    setInput,
    idpCertificate,
    isCertificateLoading,
  };
})(observer(AddIdpCertificateModal));
