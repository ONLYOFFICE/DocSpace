import React from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import { ModalDialog, Button, Text, Checkbox } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

class ConvertDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      saveOriginalFormat: true,
      hideMessage: false,
    };
  }

  onChangeFormat = () =>
    this.setState({ saveOriginalFormat: !this.state.saveOriginalFormat });
  onChangeMessageVisible = () =>
    this.setState({ hideMessage: !this.state.hideMessage });

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

const ConvertDialog = withTranslation("ConvertDialog")(ConvertDialogComponent);

export default inject(({ uploadDataStore, treeFoldersStore }) => {
  const { setTreeFolders } = treeFoldersStore;
  const { setDialogVisible, convertUploadedFiles } = uploadDataStore;

  return {
    setTreeFolders,
    setDialogVisible,
    convertUploadedFiles,
  };
})(withRouter(observer(ConvertDialog)));
