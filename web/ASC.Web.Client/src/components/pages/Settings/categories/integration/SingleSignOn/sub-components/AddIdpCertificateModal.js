import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import StyledModalDialog from "../styled-containers/StyledModalDialog";
import Text from "@appserver/components/text";
import TextArea from "@appserver/components/textarea";
import { addArgument } from "../../../../utils";

const AddIdpCertificateModal = ({ FormStore, t }) => {
  const headerContent = t("NewCertificate");

  const onClose = addArgument(FormStore.onCloseModal, "isIdpModalVisible");

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={onClose}
      visible={FormStore.isIdpModalVisible}
    >
      <ModalDialog.Header>{headerContent}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text isBold className="text-area-label">
          {t("OpenCertificate")}
        </Text>

        <TextArea
          className="text-area"
          name="newIdpCertificate"
          onChange={FormStore.onTextInputChange}
          value={FormStore.newIdpCertificate}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex" marginProp="12px 0 4px 0">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={FormStore.onAddCertificate}
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
