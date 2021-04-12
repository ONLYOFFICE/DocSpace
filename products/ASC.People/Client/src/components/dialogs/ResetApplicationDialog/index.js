import React from "react";
import PropTypes from "prop-types";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";

class ResetApplicationDialogComponent extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    console.log("Render ResetApplicationDialog");
    const { t, visible, onClose } = this.props;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("ResetApplicationTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text className="text-dialog">
              {t("ResetApplicationDescription")}
            </Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="SendBtn"
              label={t("ResetButton")}
              size="medium"
              primary={true}
            />
            <Button
              key="CloseBtn"
              className="button-dialog"
              label={t("CloseButton")}
              size="medium"
              primary={false}
              onClick={onClose}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const ResetApplicationDialog = withTranslation("ResetApplicationDialog")(
  ResetApplicationDialogComponent
);

ResetApplicationDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default ResetApplicationDialog;
