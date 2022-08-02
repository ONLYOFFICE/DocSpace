import React from "react";
//import PropTypes from "prop-types";
import { withRouter } from "react-router";
import MainButton from "@docspace/components/main-button";
import InviteDialog from "../../dialogs/InviteDialog/index";
import { withTranslation } from "react-i18next";
import toastr from "client/toastr";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import { isMobile } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";
import MobileView from "./MobileView";

import withLoader from "../../../HOCs/withLoader";

class ArticleMainButtonContent extends React.Component {
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

  // goToGuestCreate = () => {
  //   const { history, homepage } = this.props;
  //   history.push(
  //     combineUrl(AppServerConfig.proxyURL, homepage, "/create/guest")
  //   );
  //   if (isMobile || isMobileUtils()) this.props.toggleShowText();
  // };

  // goToGroupCreate = () => {
  //   const { history, homepage } = this.props;
  //   history.push(
  //     combineUrl(AppServerConfig.proxyURL, homepage, "/group/create")
  //   );
  //   if (isMobile || isMobileUtils()) this.props.toggleShowText();
  // };

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
      // guestCaption,
      // groupCaption,
      sectionWidth,

      isMobileArticle,
    } = this.props;

    const { dialogVisible } = this.state;

    const separator = {
      key: "separator",
      isSeparator: true,
    };

    const menuModel = [
      {
        key: "create-user",
        icon: combineUrl(
          AppServerConfig.proxyURL,
          homepage,
          "/images/add.employee.react.svg"
        ),
        label: userCaption,
        onClick: this.goToEmployeeCreate,
      },
      // {
      //   key: "create-guest",
      //   icon: combineUrl(
      //     AppServerConfig.proxyURL,
      //     homepage,
      //     "/images/add.guest.react.svg"
      //   ),
      //   label: guestCaption,
      //   onClick: this.goToGuestCreate,
      // },
      // {
      //   key: "create-group",
      //   icon: combineUrl(
      //     AppServerConfig.proxyURL,
      //     homepage,
      //     "/images/add.department.react.svg"
      //   ),
      //   label: groupCaption,
      //   onClick: this.goToGroupCreate,
      // },
    ];

    const links = [
      {
        key: "invite-link",
        icon: combineUrl(
          AppServerConfig.proxyURL,
          "/static/images/invitation.link.react.svg"
        ),
        label: t("PeopleTranslations:InviteLinkTitle"),
        onClick: this.onInvitationDialogClick,
      },
      /* {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          "/static/images/plane.react.svg"
        ),
        label: t("SendInvitesAgain"),
        onClick: this.onNotImplementedClick.bind(this, t("SendInvitesAgain")),
      },
      separator,
      {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          "/static/images/import.react.svg"
        ),
        label: t("ImportPeople"),
        onClick: this.onImportClick,
      }, */
    ];

    return isAdmin ? (
      <>
        {isMobileArticle ? (
          <MobileView
            labelProps={t("Common:OtherOperations")}
            actionOptions={menuModel}
            buttonOptions={links}
            sectionWidth={sectionWidth}
          />
        ) : (
          <MainButton
            isDisabled={false}
            isDropdown={true}
            text={t("Common:Actions")}
            model={[...menuModel, separator, ...links]}
            className="main-button_invitation-link"
          />
        )}

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
      isMobileArticle: auth.settingsStore.isMobileArticle,
      showText: auth.settingsStore.showText,
    };
  })(
    withTranslation(["Article", "Common", "PeopleTranslations"])(
      withLoader(observer(ArticleMainButtonContent))(<Loaders.ArticleButton />)
    )
  )
);
