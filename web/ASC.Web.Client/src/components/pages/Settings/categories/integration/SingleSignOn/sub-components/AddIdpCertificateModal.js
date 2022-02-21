import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import StyledModalDialog from "../styled-containers/StyledModalDialog";
import Text from "@appserver/components/text";
import TextArea from "@appserver/components/textarea";
import { addArguments } from "../../../../utils";

const AddIdpCertificateModal = ({ FormStore, t }) => {
  const onClose = addArguments(FormStore.onCloseModal, "isIdpModalVisible");
  const onSubmit = addArguments(FormStore.addCertificateToForm, "idp");

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={onClose}
      visible={FormStore.isIdpModalVisible}
    >
      <ModalDialog.Header>{t("NewCertificate")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text isBold className="text-area-label">
          {t("OpenCertificate")}
        </Text>

        <TextArea
          className="text-area"
          name="idp_certificate"
          onChange={FormStore.onTextInputChange}
          value={FormStore.idp_certificate}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex" marginProp="12px 0 4px 0">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={onSubmit}
            primary
            size="big"
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={onClose}
            size="big"
          />
        </Box>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default observer(AddIdpCertificateModal);
