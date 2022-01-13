import React from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import SimulatePassword from "../../SimulatePassword";
import StyledComponent from "./StyledConvertPasswordDialog";
const ConvertPasswordDialogComponent = (props) => {
  const { t } = props;

  const onConvert = () => {};
  const onClose = () => {};

  return (
    <ModalDialog visible={true}>
      <ModalDialog.Header>{t("ConvertAndOpenTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledComponent>
          <div className="convert-password-dialog_content">
            <div className="convert-password-dialog_caption">
              <Text>{t("ConversionPasswordCaption")}</Text>
            </div>
            <div className="password-input">
              <SimulatePassword
                inputMaxWidth={"512px"}
                inputBlockMaxWidth={"536px"}
              />
            </div>
          </div>
        </StyledComponent>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <StyledComponent>
          <div className="convert_dialog_footer">
            <Button
              className="convert-password-dialog_button-accept"
              key="ContinueButton"
              label={t("Convert")}
              size="medium"
              primary
              onClick={onConvert}
            />
            <Button
              className="convert-password-dialog_button"
              key="CloseButton"
              label={t("Common:Cancel")}
              size="medium"
              onClick={onClose}
            />
          </div>
        </StyledComponent>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

const ConvertPasswordDialog = withTranslation([
  "ConvertPasswordDialog",
  "ConvertDialog",
  "Home",
  "Common",
])(ConvertPasswordDialogComponent);

export default ConvertPasswordDialog;
