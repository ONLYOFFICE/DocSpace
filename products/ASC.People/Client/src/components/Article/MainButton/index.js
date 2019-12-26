import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import {
  MainButton,
  DropDownItem,
  toastr
} from "asc-web-components";
import { InviteDialog } from './../../dialogs';
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from '../i18n';
import { typeUser, typeGuest, department } from './../../../helpers/customNames';
import { store } from 'asc-web-common';
const { isAdmin } = store.auth.selectors;

class PureArticleMainButtonContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      dialogVisible: false
    }
  }

  onDropDownItemClick = (link) => {
    this.props.history.push(link);
  };

  goToEmployeeCreate = () => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/create/user`);
  };

  goToGuestCreate = () => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/create/guest`);
  }

  goToGroupCreate = () => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/group/create`);
  }

  onNotImplementedClick = (text) => {
    toastr.success(text);
  };

  onInvitationDialogClick = () => this.setState({ dialogVisible: !this.state.dialogVisible });

  render() {
    console.log("People ArticleMainButtonContent render");
    const { isAdmin, settings, t } = this.props;
    const { dialogVisible } = this.state;
    return (
      isAdmin ?
        <>
          <MainButton
            isDisabled={false}
            isDropdown={true}
            text={t('Actions')}
          >
            <DropDownItem
              icon="CatalogEmployeeIcon"
              label={t('CustomNewEmployee', { typeUser })}
              onClick={this.goToEmployeeCreate}
            />
            <DropDownItem
              icon="CatalogGuestIcon"
              label={t('CustomNewGuest', { typeGuest })}
              onClick={this.goToGuestCreate}
            />
            <DropDownItem
              icon="CatalogDepartmentsIcon"
              label={t('CustomNewDepartment', { department })}
              onClick={this.goToGroupCreate}
            />
            <DropDownItem isSeparator />
            <DropDownItem
              icon="InvitationLinkIcon"
              label={t('InviteLinkTitle')}
              onClick={this.onInvitationDialogClick}
            />
            {/* <DropDownItem
              icon="PlaneIcon"
              label={t('LblInviteAgain')}
              onClick={this.onNotImplementedClick.bind(this, "Invite again action")}
            /> */}
            {false &&
              <DropDownItem
                icon="ImportIcon"
                label={t('ImportPeople')}
                onClick={this.onDropDownItemClick.bind(this, `${settings.homepage}/import`)}
              />
            }
          </MainButton>
          {dialogVisible &&
            <InviteDialog
              visible={dialogVisible}
              onClose={this.onInvitationDialogClick}
              onCloseButton={this.onInvitationDialogClick}
            />
          }
        </>
        :
        <></>
    );
  };
};

const ArticleMainButtonContentContainer = withTranslation()(PureArticleMainButtonContent);

const ArticleMainButtonContent = (props) => {
  const { language } = props;
  i18n.changeLanguage(language);
  return (<I18nextProvider i18n={i18n}><ArticleMainButtonContentContainer {...props} /></I18nextProvider>);
};

ArticleMainButtonContent.propTypes = {
  isAdmin: PropTypes.bool.isRequired,
  history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
  return {
    isAdmin: isAdmin(state.auth.user),
    language: state.auth.user.cultureName || state.auth.settings.culture,
    settings: state.auth.settings
  }
}

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));