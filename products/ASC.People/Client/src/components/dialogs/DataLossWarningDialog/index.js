import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils } from "asc-web-common";
import ModalDialogContainer from "../ModalDialogContainer";
import { createI18N } from "../../../helpers/i18n";
import {
  setIsVisibleDataLossDialog,
  setIsEditingForm,
} from "../../../store/people/actions";

const i18n = createI18N({
  page: "DataLossWarningDialog",
  localesPath: "dialogs/DataLossWarningDialog",
});

const { changeLanguage } = utils;

class DataLossWarningDialogComponent extends React.Component {
  constructor(props) {
    super(props);
    changeLanguage(i18n);
  }

  onClose = () => {
    const { setIsVisibleDataLossDialog } = this.props;
    setIsVisibleDataLossDialog(false);
  };

  onSubmit = () => {
    const {
      onContinue,
      setIsVisibleDataLossDialog,
      setIsEditingForm,
      editingForm,
    } = this.props;

    setIsVisibleDataLossDialog(false, null);
    setIsEditingForm(false);

    if (editingForm.callback) {
      editingForm.callback();
    } else {
      onContinue && onContinue();
    }
  };
  render() {
    const { t, editingForm } = this.props;

    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={editingForm.isVisibleDataLossDialog}
          onClose={this.onClose}
        >
          <ModalDialog.Header>{t("DataLossWarningHeader")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text fontSize="13px">{t("DataLossWarningBody")}</Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="LeaveForm"
              label={t("DataLossWarningLeaveBtn")}
              size="medium"
              primary={true}
              onClick={this.onSubmit}
            />
            <Button
              className="button-dialog"
              key="StayOnPage"
              label={t("DataLossWarningCancelBtn")}
              size="medium"
              onClick={this.onClose}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const DataLossWarningDialogTranslated = withTranslation()(
  DataLossWarningDialogComponent
);

const DataLossWarningDialog = (props) => (
  <DataLossWarningDialogTranslated i18n={i18n} {...props} />
);

DataLossWarningDialog.propTypes = {
  editingForm: PropTypes.object.isRequired,
  onContinue: PropTypes.func.isRequired,
};

function mapStateToProps(state) {
  return {
    editingForm: state.people.editingForm,
  };
}

export default connect(mapStateToProps, {
  setIsVisibleDataLossDialog,
  setIsEditingForm,
})(DataLossWarningDialog);
