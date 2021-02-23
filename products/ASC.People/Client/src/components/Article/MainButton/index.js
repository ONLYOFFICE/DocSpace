import React from "react";
//import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { MainButton, DropDownItem } from "asc-web-components";
import { InviteDialog } from "./../../dialogs";
import { withTranslation } from "react-i18next";
import { toastr, Loaders } from "asc-web-common";
import { inject, observer } from "mobx-react";

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

const ArticleMainButtonContent = withTranslation("Article")(
  PureArticleMainButtonContent
);

export default inject(({ auth }) => ({
  isAdmin: auth.isAdmin,
  settings: auth.settingsStore,
  isLoaded: auth.isLoaded,
  language: auth.language,
}))(observer(withRouter(ArticleMainButtonContent)));
