import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "LeaveFormDialog",
  localesPath: "dialogs/LeaveFormDialog",
});

const { changeLanguage } = utils;

class LeaveFormDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: props.visible,
    };
    changeLanguage(i18n);
  }

  onClose = () => {
    this.setState({ visible: false });
  };

  onSubbmit = () => {
    const { onContinue } = this.props;
    onContinue && onContinue();
  };
  render() {
    const { t } = this.props;
    const { visible } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={this.onClose}>
          <ModalDialog.Header>{t("LeaveDialogHeader")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text fontSize="13px">{t("LeaveDialogBody")}</Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="LeaveForm"
              label={t("LeaveDialogLeaveBtn")}
              size="medium"
              primary={true}
              onClick={this.onSubbmit}
            />
            <Button
              className="button-dialog"
              key="StayOnPage"
              label={t("LeaveDialogCancelBtn")}
              size="medium"
              onClick={this.onClose}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const LeaveFormDialogTranslated = withTranslation()(LeaveFormDialogComponent);

const LeaveFormDialog = (props) => (
  <LeaveFormDialogTranslated i18n={i18n} {...props} />
);

LeaveFormDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onContinue: PropTypes.func.isRequired,
};

export default LeaveFormDialog;
