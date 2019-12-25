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
import { withTranslation, Trans } from "react-i18next";
import i18n from "./i18n";
import { api } from "asc-web-common";
import { typeUser } from "../../../helpers/customNames";
import { fetchPeople } from '../../../store/people/actions';
import ModalDialogContainer from '../ModalDialogContainer';
const { deleteUser } = api.people;
const { Filter } = api;

class DeleteProfileEverDialogComponent extends React.Component {
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
    this.setState({ isRequestRunning: true }, () => {
      deleteUser(user.id)
        .then((res) => {
          toastr.success(t('SuccessfullyDeleteUserInfoMessage'));
          return fetchPeople(filter);
        })
        .catch((error) => toastr.error(error))
        .finally(() => {
          this.setState({ isRequestRunning: false }, () => onClose());
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
                <Trans i18nKey='DeleteUserConfirmation' i18n={i18n}>
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
                className='button-dialog'
                key="ReassignBtn"
                label={t('ReassignData')}
                size="medium"
                onClick={this.onReassignDataClick}
                isDisabled={isRequestRunning}
              />
            </>
          }
        />
      </ModalDialogContainer>
    );
  }
}

const DeleteProfileEverDialogTranslated = withTranslation()(DeleteProfileEverDialogComponent);

const DeleteProfileEverDialog = props => (
  <DeleteProfileEverDialogTranslated i18n={i18n} {...props} />
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
