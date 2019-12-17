import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  toastr,
  ModalDialog,
  Button,
  Link,
  Text
} from "asc-web-components";
import { withTranslation, I18nextProvider, Trans } from "react-i18next";
import i18n from "./i18n";
import ModalDialogContainer from '../ModalDialogContainer';
import { api } from "asc-web-common";
const { sendInstructionsToChangePassword } = api.people;

class PureChangePasswordDialog extends React.Component {
  constructor(props) {
    super(props);

    const { language } = props;

    this.state = {
      isRequestRunning: false
    };

    i18n.changeLanguage(language);
  }
  onSendPasswordChangeInstructions = () => {
    const { email, onClose } = this.props;
    this.setState({ isRequestRunning: true }, function () {
      sendInstructionsToChangePassword(email)
        .then((res) => {
          toastr.success(res);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          onClose();
          this.setState({ isRequestRunning: false });
        });
    })

  }


  render() {
    console.log("ChangePasswordDialog render");
    const { t, visible, email, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t('PasswordChangeTitle')}
        bodyContent={
          <Text fontSize='13px'>
            <Trans i18nKey="MessageSendPasswordChangeInstructionsOnEmail">
              Send the password change instructions to the
              <Link type="page" href={`mailto:${email}`} isHovered title={email}>
                {{ email }}
              </Link>
              email address
          </Trans>
          </Text>

        }
        footerContent={
          <Button
            key="SendBtn"
            label={t('SendButton')}
            size="medium"
            primary={true}
            onClick={this.onSendPasswordChangeInstructions}
            isLoading={isRequestRunning}
          />
        }
      />
    );
  }
}

const ChangePasswordDialogContainer = withTranslation()(PureChangePasswordDialog);

const ChangePasswordDialog = props => (
  <I18nextProvider i18n={i18n}>
    <ChangePasswordDialogContainer {...props} />
  </I18nextProvider>
);

ChangePasswordDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName,
  };
}

export default connect(mapStateToProps, {})(withRouter(ChangePasswordDialog));
