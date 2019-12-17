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
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";
import ModalDialogContainer from '../ModalDialogContainer';
import { api } from "asc-web-common";
const { sendInstructionsToDelete } = api.people;

class PureDeleteSelfProfileDialog extends React.Component {
  constructor(props) {
    super(props);

    const { language } = props;

    this.state = {
      isRequestRunning: false
    };

    i18n.changeLanguage(language);
  }
  onDeleteSelfProfileInstructions = () => {
    const { onClose } = this.props;
    this.setState({ isRequestRunning: true }, function () {
      sendInstructionsToDelete()
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
    console.log("DeleteSelfProfileDialog render");
    const { t, visible, email, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={visible}
          onClose={onClose}
          headerContent={t('DeleteProfileTitle')}
          bodyContent={
            <Text fontSize='13px'>
              {t('DeleteProfileInfo')} <Link type="page" href={`mailto:${email}`} isHovered title={email}>
                {email}
              </Link>
            </Text>
          }
          footerContent={
            <>
              <Button
                key="SendBtn"
                label={t('SendButton')}
                size="medium"
                primary={true}
                onClick={this.onDeleteSelfProfileInstructions}
                isLoading={isRequestRunning}
              />
              <Button
                className='button-dialog'
                key="CloseBtn"
                label={t('CloseButton')}
                size="medium"
                onClick={onClose}
                isDisabled={isRequestRunning}
              />
            </>
          }
        />
      </ModalDialogContainer>
    );
  }
}


const DeleteSelfProfileDialogContainer = withTranslation()(PureDeleteSelfProfileDialog);

const DeleteSelfProfileDialog = props => (
  <I18nextProvider i18n={i18n}>
    <DeleteSelfProfileDialogContainer {...props} />
  </I18nextProvider>
);

DeleteSelfProfileDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  email: PropTypes.string.isRequired,
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName,
  };
}

export default connect(mapStateToProps, {})(withRouter(DeleteSelfProfileDialog));
