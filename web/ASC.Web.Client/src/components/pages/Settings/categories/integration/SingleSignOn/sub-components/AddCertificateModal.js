import React from "react";
import styled from "styled-components";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import TextArea from "@appserver/components/textarea";

const StyledModalDialog = styled(ModalDialog)`
  .text-area {
    margin-top: 12px;
  }

  .ok-button {
    margin-right: 10px;
  }

  .text-area-label {
    margin-top: 5px;
  }
`;

const AddCertificateModal = ({ FormStore, t }) => {
  const headerContent = t("NewCertificate");

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={FormStore.onCloseModal}
      visible={FormStore.isModalVisible}
    >
      <ModalDialog.Header>{headerContent}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text className="text-area-label">{t("OpenCertificate")}</Text>
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
            label={t("OKButton", { ns: "Common" })}
            onClick={FormStore.onAddCertificate}
            primary
            size="medium"
          />
          <Button
            label={t("CancelButton", { ns: "Common" })}
            onClick={FormStore.onCloseModal}
            size="medium"
          />
        </Box>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default observer(AddCertificateModal);
