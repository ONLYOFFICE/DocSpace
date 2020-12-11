import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import ModalDialogContainer from "../ModalDialogContainer";
import { ModalDialog, Button, Text, Checkbox } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils } from "asc-web-common";
import {
  setTreeFolders,
  setDialogVisible,
  convertUploadedFiles,
} from "../../../store/files/actions";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "ConvertDialog",
  localesPath: "dialogs/ConvertDialog",
});

const { changeLanguage } = utils;

class ConvertDialogComponent extends React.Component {
  constructor(props) {
    super(props);
    changeLanguage(i18n);

    this.state = {
      saveOriginalFormat: true,
      hideMessage: false,
    };
  }

  onChangeFormat = () =>
    this.setState({ saveOriginalFormat: !this.state.saveOriginalFormat });
  onChangeMessageVisible = () =>
    this.setState({ hideMessage: !this.state.hideMessage });

  shouldComponentUpdate(nextProps, nextState) {
    if (this.props.visible !== nextProps.visible) {
      return true;
    }

    if (this.state.saveOriginalFormat !== nextState.saveOriginalFormat) {
      return true;
    }

    if (this.state.hideMessage !== nextState.hideMessage) {
      return true;
    }

    return false;
  }

  onConvert = () => this.props.convertUploadedFiles(this.props.t);
  onClose = () => this.props.setDialogVisible(this.props.t);

  render() {
    const { t, visible } = this.props;
    const { saveOriginalFormat, hideMessage } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={this.onClose}>
          <ModalDialog.Header>{t("ConversionTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <div className="convert_dialog_content">
              <img
                className="convert_dialog_image"
                src="images/convert_alert.png"
                alt="convert alert"
              />
              <div className="convert_dialog-content">
                <Text>{t("ConversionMessage")}</Text>
                <Checkbox
                  className="convert_dialog_checkbox"
                  label={t("SaveOriginalFormatMessage")}
                  isChecked={saveOriginalFormat}
                  onChange={this.onChangeFormat}
                />
                <Checkbox
                  className="convert_dialog_checkbox"
                  label={t("HideMessage")}
                  isChecked={hideMessage}
                  onChange={this.onChangeMessageVisible}
                />
              </div>
            </div>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <div className="convert_dialog_footer">
              <Button
                className="convert_dialog_button-accept"
                key="ContinueButton"
                label={t("ContinueButton")}
                size="medium"
                primary
                onClick={this.onConvert}
              />
              <Button
                className="convert_dialog_button"
                key="CloseButton"
                label={t("CloseButton")}
                size="medium"
                onClick={this.onClose}
              />
            </div>
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const ModalDialogContainerTranslated = withTranslation()(
  ConvertDialogComponent
);

const ConvertDialog = (props) => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

export default connect(null, {
  setTreeFolders,
  setDialogVisible,
  convertUploadedFiles,
})(withRouter(ConvertDialog));
