import React, { useEffect } from "react";
import styled from "styled-components";
import { ModalDialog, Text, Button } from "asc-web-components";
import { utils } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

const { changeLanguage } = utils;

//import { connect } from "react-redux";
//import { withRouter } from "react-router";
//import { getOperationsFolders } from "../../../store/files/selectors";
const i18n = createI18N({
  page: "ThirdPartyMoveDialog",
  localesPath: "dialogs/ThirdPartyMoveDialog",
});

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

const ThirdPartyMoveContainer = withTranslation()(PureThirdPartyMoveContainer);

const ThirdPartyMoveDialog = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <ThirdPartyMoveContainer {...props} />
    </I18nextProvider>
  );
};

export default ThirdPartyMoveDialog;
