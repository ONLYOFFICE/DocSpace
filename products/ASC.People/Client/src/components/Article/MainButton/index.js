import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { store } from "asc-web-common";
import { MainButton, DropDownItem } from "asc-web-components";
import { InviteDialog } from "./../../dialogs";
import { withTranslation, I18nextProvider } from "react-i18next";
import { utils, toastr, Loaders } from "asc-web-common";
import { createI18N } from "../../../helpers/i18n";
const { getLanguage, getSettings } = store.auth.selectors;
const i18n = createI18N({
  page: "Article",
  localesPath: "Article",
});

const { changeLanguage } = utils;

class PureArticleMainButtonContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      dialogVisible: false,
    };
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
  };

  goToGroupCreate = () => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/group/create`);
  };

  onNotImplementedClick = (text) => {
    toastr.success(text);
  };

  onInvitationDialogClick = () =>
    this.setState({ dialogVisible: !this.state.dialogVisible });

  render() {
    //console.log("People ArticleMainButtonContent render");
    const { settings, t, isLoaded } = this.props;
    const { userCaption, guestCaption, groupCaption } = settings.customNames;
    const { dialogVisible } = this.state;
    return !isLoaded ? (
      <Loaders.Rectangle />
    ) : (
      <>
        <MainButton isDisabled={false} isDropdown={true} text={t("Actions")}>
          <DropDownItem
            icon="AddEmployeeIcon"
            label={userCaption}
            onClick={this.goToEmployeeCreate}
          />
          <DropDownItem
            icon="AddGuestIcon"
            label={guestCaption}
            onClick={this.goToGuestCreate}
          />
          <DropDownItem
            icon="AddDepartmentIcon"
            label={groupCaption}
            onClick={this.goToGroupCreate}
          />
          <DropDownItem isSeparator />
          <DropDownItem
            icon="InvitationLinkIcon"
            label={t("InviteLinkTitle")}
            onClick={this.onInvitationDialogClick}
          />
          {/* <DropDownItem
              icon="PlaneIcon"
              label={t('LblInviteAgain')}
              onClick={this.onNotImplementedClick.bind(this, "Invite again action")}
            /> */}
          {false && (
            <DropDownItem
              icon="ImportIcon"
              label={t("ImportPeople")}
              onClick={this.onDropDownItemClick.bind(
                this,
                `${settings.homepage}/import`
              )}
            />
          )}
        </MainButton>
        {dialogVisible && (
          <InviteDialog
            visible={dialogVisible}
            onClose={this.onInvitationDialogClick}
            onCloseButton={this.onInvitationDialogClick}
          />
        )}
      </>
    );
  }
}

const ArticleMainButtonContentContainer = withTranslation()(
  PureArticleMainButtonContent
);

const ArticleMainButtonContent = ({ language, ...rest }) => {
  useEffect(() => {
    changeLanguage(i18n, language);
  }, [language]);

  return (
    <I18nextProvider i18n={i18n}>
      <ArticleMainButtonContentContainer {...rest} />
    </I18nextProvider>
  );
};

ArticleMainButtonContent.propTypes = {
  history: PropTypes.object.isRequired,
  language: PropTypes.string,
};

const mapStateToProps = (state) => {
  const { isLoaded } = state.auth;
  return {
    isLoaded,
    settings: getSettings(state),
    language: getLanguage(state),
  };
};

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));
