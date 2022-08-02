import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
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
  } = props;

  return (
    <StyledModalDialog
      contentHeight="100%"
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
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex" marginProp="12px 0 4px 0">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={addIdpCertificate}
            primary
            size="small"
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={closeIdpModal}
            size="small"
          />
        </Box>
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
  } = ssoStore;

  return {
    closeIdpModal,
    addIdpCertificate,
    idpIsModalVisible,
    setInput,
    idpCertificate,
  };
})(observer(AddIdpCertificateModal));
