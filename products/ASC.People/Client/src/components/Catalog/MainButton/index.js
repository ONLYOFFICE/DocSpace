import React from "react";
//import PropTypes from "prop-types";
import { withRouter } from "react-router";
import MainButton from "@appserver/components/main-button";
import DropDownItem from "@appserver/components/drop-down-item";
import InviteDialog from "./../../dialogs/InviteDialog/index";
import { withTranslation } from "react-i18next";
import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import withLoader from "../../../HOCs/withLoader";
import { isMobile } from "react-device-detect";
import { isMobile as isMobileUtils } from "@appserver/components/utils/device";

class CatalogMainButtonContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      dialogVisible: false,
    };
  }

  onImportClick = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "${homepage}/import")
    );
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  goToEmployeeCreate = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/create/user")
    );
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  goToGuestCreate = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/create/guest")
    );
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  goToGroupCreate = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/group/create")
    );
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  onNotImplementedClick = (text) => {
    toastr.success(text);
  };

  onInvitationDialogClick = () =>
    this.setState({ dialogVisible: !this.state.dialogVisible });

  render() {
    //console.log("People ArticleMainButtonContent render");
    const {
      t,
      isAdmin,
      homepage,
      userCaption,
      guestCaption,
      groupCaption,
    } = this.props;

    const { dialogVisible } = this.state;

    return isAdmin ? (
      <>
        <MainButton
          isDisabled={false}
          isDropdown={true}
          text={t("Common:Actions")}
        >
          <DropDownItem
            icon={combineUrl(
              AppServerConfig.proxyURL,
              homepage,
              "/images/add.employee.react.svg"
            )}
            label={userCaption}
            onClick={this.goToEmployeeCreate}
          />

          <DropDownItem
            icon={combineUrl(
              AppServerConfig.proxyURL,
              homepage,
              "/images/add.guest.react.svg"
            )}
            label={guestCaption}
            onClick={this.goToGuestCreate}
          />
          <DropDownItem
            icon={combineUrl(
              AppServerConfig.proxyURL,
              homepage,
              "/images/add.department.react.svg"
            )}
            label={groupCaption}
            onClick={this.goToGroupCreate}
          />
          <DropDownItem isSeparator />
          <DropDownItem
            icon={combineUrl(
              AppServerConfig.proxyURL,
              "/static/images/invitation.link.react.svg"
            )}
            label={t("Translations:InviteLinkTitle")}
            onClick={this.onInvitationDialogClick}
          />
          {/* <DropDownItem
              icon="images/plane.react.svg"
              label={t("SendInvitesAgain")}
              onClick={this.onNotImplementedClick.bind(this, t("SendInvitesAgain"))}
            /> */}
          {false && (
            <DropDownItem
              icon={combineUrl(
                AppServerConfig.proxyURL,
                homepage,
                "/images/import.react.svg"
              )}
              label={t("ImportPeople")}
              onClick={this.onImportClick}
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
    ) : (
      <></>
    );
  }
}

export default withRouter(
  inject(({ auth }) => {
    const {
      userCaption,
      guestCaption,
      groupCaption,
    } = auth.settingsStore.customNames;
    return {
      isAdmin: auth.isAdmin,
      homepage: config.homepage,
      userCaption,
      guestCaption,
      groupCaption,
      toggleShowText: auth.settingsStore.toggleShowText,
    };
  })(
    withTranslation(["Article", "Common", "Translations"])(
      withLoader(observer(CatalogMainButtonContent))(<Loaders.MainButton />)
    )
  )
);
