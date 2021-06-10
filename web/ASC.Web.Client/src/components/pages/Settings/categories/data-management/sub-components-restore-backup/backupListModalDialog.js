import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";

class BackupListModalDialog extends React.Component {
  render() {
    const { onModalClose, isVisibleDialog, t } = this.props;

    return (
      <ModalDialog visible={isVisibleDialog} onClose={onModalClose}>
        <ModalDialog.Header>{t("BackupList")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("BackupListDeleteWarning")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            className="modal-dialog-button"
            primary
            size="big"
            label={t("Common:CloseButton")}
            tabIndex={1}
            onClick={onModalClose}
          />
        </ModalDialog.Footer>
      </ModalDialog>
    );
  }
}

BackupListModalDialog.propTypes = {
  t: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired,
  isVisibleDialog: PropTypes.bool.isRequired,
};

export default BackupListModalDialog;
