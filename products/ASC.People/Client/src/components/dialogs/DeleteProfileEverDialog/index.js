import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  toastr,
  ModalDialog,
  Button,
  Text
} from "asc-web-components";
import { withTranslation, I18nextProvider, Trans } from "react-i18next";
import i18n from "./i18n";
import styled from "styled-components";
import { api } from "asc-web-common";
import { typeUser } from "../../../helpers/customNames";
import { fetchPeople } from '../../../store/people/actions';
const { deleteUser } = api.people;
const { Filter } = api;

const ModalDialogContainer = styled.div`

  .margin-top {
    margin-top: 16px;
  }

  .margin-left {
    margin-left: 8px;
  }

  .warning-text {
    margin: 20px 0; 
  }
`;

class PureDeleteProfileEverDialog extends React.Component {
  constructor(props) {
    super(props);

    const { language } = props;

    this.state = {
      isRequestRunning: false
    };

    i18n.changeLanguage(language);
  }
  onDeleteProfileEver = () => {
    const { onClose, filter, fetchPeople, user, t } = this.props;
    this.setState({ isRequestRunning: true }, function () {
      deleteUser(user.id)
        .then((res) => {
          toastr.success(t('SuccessfullyDeleteUserInfoMessage'));
          return fetchPeople(filter);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          onClose();
          this.setState({ isRequestRunning: false });
        });
    })
  };

  onReassignDataClick = () => {
    const { history, settings, user } = this.props;
    history.push(`${settings.homepage}/reassign/${user.userName}`)
  };

  render() {
    console.log("DeleteProfileEverDialog render");
    const { t, visible, user, onClose } = this.props;
    const { isRequestRunning } = this.state;

    return (
      <ModalDialogContainer>
        <ModalDialog
          visible={visible}
          onClose={onClose}
          headerContent={t('Confirmation')}
          bodyContent={
            <>
              <Text>
                <Trans i18nKey='DeleteUserConfirmation'>
                  {{ typeUser }} <strong>{{ user: user.displayName }}</strong> will be deleted.
              </Trans>
              </Text>
              <Text>{t('NotBeUndone')}</Text>
              <Text color="#c30" fontSize="18px" className='warning-text'>
                {t('Warning')}
              </Text>
              <Text>
                {t('DeleteUserDataConfirmation')}
              </Text>
            </>
          }
          footerContent={
            <>
              <Button
                key="OKBtn"
                label={t('OKButton')}
                size="medium"
                primary={true}
                onClick={this.onDeleteProfileEver}
                isLoading={isRequestRunning}
              />
              <Button
                className='margin-left'
                key="ReassignBtn"
                label={t('ReassignData')}
                size="medium"
                onClick={this.onReassignDataClick}
                isDisabled={isRequestRunning}
              />
              <Button
                className='margin-left'
                key="CancelBtn"
                label={t('CancelButton')}
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


const DeleteProfileEverDialogContainer = withTranslation()(PureDeleteProfileEverDialog);

const DeleteProfileEverDialog = props => (
  <I18nextProvider i18n={i18n}>
    <DeleteProfileEverDialogContainer {...props} />
  </I18nextProvider>
);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
  filter: PropTypes.instanceOf(Filter).isRequired,
  fetchPeople: PropTypes.func.isRequired,
  settings: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName,
  };
}

export default connect(mapStateToProps, { fetchPeople })(withRouter(DeleteProfileEverDialog));
