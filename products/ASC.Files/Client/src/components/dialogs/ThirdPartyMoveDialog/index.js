import React from "react";
import styled from "styled-components";
import { ModalDialog, Text, Button } from "asc-web-components";
import { withTranslation } from "react-i18next";

const StyledOperationDialog = styled.div`
  .operation-button {
    margin-right: 8px;
  }
`;

const PureThirdPartyMoveContainer = ({
  t,
  onClose,
  visible,
  startMoveOperation,
  startCopyOperation,
  provider,
}) => {
  const zIndex = 310;

  return (
    <StyledOperationDialog>
      <ModalDialog visible={visible} zIndex={zIndex} onClose={onClose}>
        <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("MoveConfirmationMessage", { provider })}</Text>
          <br />
          <Text>{t("MoveConfirmationAlert")}</Text>
        </ModalDialog.Body>

        <ModalDialog.Footer>
          <Button
            className="operation-button"
            label={t("Move")}
            size="big"
            primary
            onClick={startMoveOperation}
          />
          <Button
            className="operation-button"
            label={t("Copy")}
            size="big"
            onClick={startCopyOperation}
          />
          <Button
            className="operation-button"
            label={t("CancelButton")}
            size="big"
            onClick={onClose}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    </StyledOperationDialog>
  );
};

export default withTranslation("ThirdPartyMoveDialog")(
  PureThirdPartyMoveContainer
);
