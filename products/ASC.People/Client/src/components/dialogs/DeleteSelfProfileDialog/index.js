import React from "react";
import PropTypes from "prop-types";

import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { Trans, withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { sendInstructionsToDelete } from "@appserver/common/api/people";
import toastr from "studio/toastr";

class DeleteSelfProfileDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isRequestRunning: false,
    };
  }
  onDeleteSelfProfileInstructions = () => {
    const { t, email, onClose } = this.props;
    this.setState({ isRequestRunning: true }, () => {
      sendInstructionsToDelete()
        .then((res) => {
          toastr.success(
            <div
              dangerouslySetInnerHTML={{
                __html: res,
              }}
            />
          );
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => onClose());
        });
    });
  };

  render() {
    console.log("DeleteSelfProfileDialog render");
    const { t, tReady, visible, email, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={onClose}
      >
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
            label={t("Common:SendButton")}
            size="medium"
            primary={true}
            onClick={this.onDeleteSelfProfileInstructions}
            isLoading={isRequestRunning}
          />
          <Button
            className="button-dialog"
            key="CloseBtn"
            label={t("Common:CloseButton")}
            size="medium"
            onClick={onClose}
            isDisabled={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DeleteSelfProfileDialog = withTranslation([
  "DeleteSelfProfileDialog",
  "Common",
])(DeleteSelfProfileDialogComponent);

DeleteSelfProfileDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
};

export default DeleteSelfProfileDialog;
