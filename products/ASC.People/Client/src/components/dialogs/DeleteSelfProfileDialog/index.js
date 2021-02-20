import React from "react";
import PropTypes from "prop-types";
import { ModalDialog, Button, Link, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { api, toastr } from "asc-web-common";
const { sendInstructionsToDelete } = api.people;

class DeleteSelfProfileDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isRequestRunning: false,
    };
  }
  onDeleteSelfProfileInstructions = () => {
    const { onClose } = this.props;
    this.setState({ isRequestRunning: true }, () => {
      sendInstructionsToDelete()
        .then((res) => {
          toastr.success(res);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => onClose());
        });
    });
  };

  render() {
    console.log("DeleteSelfProfileDialog render");
    const { t, visible, email, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("DeleteProfileTitle")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text fontSize="13px">
              {t("DeleteProfileInfo")}{" "}
              <Link
                type="page"
                href={`mailto:${email}`}
                noHover
                color="#316DAA"
                title={email}
              >
                {email}
              </Link>
            </Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              key="SendBtn"
              label={t("SendButton")}
              size="medium"
              primary={true}
              onClick={this.onDeleteSelfProfileInstructions}
              isLoading={isRequestRunning}
            />
            <Button
              className="button-dialog"
              key="CloseBtn"
              label={t("CloseButton")}
              size="medium"
              onClick={onClose}
              isDisabled={isRequestRunning}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const DeleteSelfProfileDialog = withTranslation("DeleteSelfProfileDialog")(
  DeleteSelfProfileDialogComponent
);

DeleteSelfProfileDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
};

export default DeleteSelfProfileDialog;
