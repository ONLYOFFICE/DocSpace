import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import TextArea from "@appserver/components/textarea";

import ModalComboBox from "./ModalComboBox";
import StyledModalDialog from "../styled-containers/StyledModalDialog";
import { addArguments } from "../../../../utils";

const AddSpCertificateModal = ({ FormStore, t }) => {
  const onClose = addArguments(FormStore.onCloseModal, "isSpModalVisible");
  const onSubmit = addArguments(FormStore.addCertificateToForm, "sp");

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={onClose}
      visible={FormStore.isSpModalVisible}
    >
      <ModalDialog.Header>{t("NewCertificate")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Box marginProp="4px 0 15px 0">
          <Link
            className="generate"
            isHovered
            onClick={FormStore.generateCertificate}
            type="action"
          >
            {t("GenerateCertificate")}
          </Link>
        </Box>

        <Text isBold className="text-area-label">
          {t("OpenCertificate")}
        </Text>

        <TextArea
          className="text-area"
          name="sp_certificate"
          onChange={FormStore.onTextInputChange}
          value={FormStore.sp_certificate}
        />

        <Text isBold className="text-area-label">
          {t("PrivateKey")}
        </Text>

        <TextArea
          className="text-area"
          name="sp_privateKey"
          onChange={FormStore.onTextInputChange}
          value={FormStore.sp_privateKey}
        />

        <ModalComboBox FormStore={FormStore} t={t} />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex">
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

export default observer(AddSpCertificateModal);
