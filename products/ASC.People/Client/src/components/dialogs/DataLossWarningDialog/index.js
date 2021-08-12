import React from "react";
import PropTypes from "prop-types";

import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";

import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";

class DataLossWarningDialogComponent extends React.Component {
  onClose = () => {
    const { setIsVisibleDataLossDialog } = this.props;
    setIsVisibleDataLossDialog(false);
  };

  onSubmit = () => {
    const {
      onContinue,
      setIsVisibleDataLossDialog,
      setIsEditingForm,
      callback,
    } = this.props;

    setIsVisibleDataLossDialog(false);
    setIsEditingForm(false);

    if (callback) {
      callback();
    } else {
      onContinue && onContinue();
    }
  };
  render() {
    const { t, tReady, isVisibleDataLossDialog } = this.props;

    return (
      <ModalDialogContainer
        visible={isVisibleDataLossDialog}
        onClose={this.onClose}
        isLoading={!tReady}
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
            label={t("Common:CancelButton")}
            size="medium"
            onClick={this.onClose}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DataLossWarningDialog = withTranslation("DataLossWarningDialog")(
  DataLossWarningDialogComponent
);

DataLossWarningDialog.propTypes = {
  //editingForm: PropTypes.object.isRequired,
  onContinue: PropTypes.func.isRequired,
};

export default inject(({ peopleStore }) => {
  return {
    isVisibleDataLossDialog:
      peopleStore.editingFormStore.isVisibleDataLossDialog,
    setIsVisibleDataLossDialog:
      peopleStore.editingFormStore.setIsVisibleDataLossDialog,
    setIsEditingForm: peopleStore.editingFormStore.setIsEditingForm,
    callback: peopleStore.editingFormStore.callback,
  };
})(observer(DataLossWarningDialog));
