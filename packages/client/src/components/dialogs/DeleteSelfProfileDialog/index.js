import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import { Trans, withTranslation } from "react-i18next";
import ModalDialogContainer from "../ModalDialogContainer";
import { sendInstructionsToDelete } from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";

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
    const { t, tReady, visible, email, onClose, theme } = this.props;
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
              color={theme.peopleDialogs.deleteSelf.linkColor}
              title={email}
            >
              {email}
            </Link>
          </Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            key="DeleteSelfSendBtn"
            label={t("Common:SendButton")}
            size="normal"
            scale
            primary={true}
            onClick={this.onDeleteSelfProfileInstructions}
            isLoading={isRequestRunning}
          />
          <Button
            key="CloseBtn"
            label={t("Common:CloseButton")}
            size="normal"
            scale
            onClick={onClose}
            isDisabled={isRequestRunning}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DeleteSelfProfileDialog = inject(({ auth }) => ({
  theme: auth.settingsStore.theme,
}))(
  observer(
    withTranslation(["DeleteSelfProfileDialog", "Common"])(
      DeleteSelfProfileDialogComponent
    )
  )
);

DeleteSelfProfileDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
};

export default DeleteSelfProfileDialog;
