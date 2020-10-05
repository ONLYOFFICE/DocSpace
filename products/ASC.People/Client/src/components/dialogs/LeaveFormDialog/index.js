import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { createI18N } from "../../../helpers/i18n";
import {
  setIsVisibleModalLeave,
  setIsEditingForm,
} from "../../../store/people/actions";

const i18n = createI18N({
  page: "LeaveFormDialog",
  localesPath: "dialogs/LeaveFormDialog",
});

const { changeLanguage } = utils;

class LeaveFormDialogComponent extends React.Component {
  constructor(props) {
    super(props);
    changeLanguage(i18n);
  }

  onClose = () => {
    const { setIsVisibleModalLeave } = this.props;
    setIsVisibleModalLeave(false);
  };

  onSubmit = () => {
    const { onContinue, setIsVisibleModalLeave, setIsEditingForm } = this.props;

    setIsVisibleModalLeave(false);
    setIsEditingForm(false);

    onContinue && onContinue();
  };
  render() {
    const { t, isVisibleModalLeave } = this.props;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={isVisibleModalLeave} onClose={this.onClose}>
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
              onClick={this.onSubmit}
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
  isVisibleModalLeave: PropTypes.bool.isRequired,
  onContinue: PropTypes.func.isRequired,
};

function mapStateToProps(state) {
  return {
    isVisibleModalLeave: state.people.editingForm.isVisibleModalLeave,
  };
}

export default connect(mapStateToProps, {
  setIsVisibleModalLeave,
  setIsEditingForm,
})(LeaveFormDialog);
