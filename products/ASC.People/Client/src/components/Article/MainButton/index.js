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
  };

  goToEmployeeCreate = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/create/user")
    );
  };

  goToGuestCreate = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/create/guest")
    );
  };

  goToGroupCreate = () => {
    const { history, homepage } = this.props;
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, "/group/create")
    );
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

    const menuModel = [
      {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          homepage,
          "/images/add.employee.react.svg"
        ),
        label: userCaption,
        onClick: this.goToEmployeeCreate,
        className: "main-button_create-user",
      },
      {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          homepage,
          "/images/add.guest.react.svg"
        ),
        label: guestCaption,
        onClick: this.goToGuestCreate,
        className: "main-button_create-guest",
      },
      {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          homepage,
          "/images/add.department.react.svg"
        ),
        label: groupCaption,
        onClick: this.goToGroupCreate,
        className: "main-button_create-group",
      },
      {
        isSeparator: true,
      },
      {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          "/static/images/invitation.link.react.svg"
        ),
        label: t("Translations:InviteLinkTitle"),
        onClick: this.onInvitationDialogClick,
        className: "main-button_invitation-link",
      },
      /* {
        icon: combineUrl(
          AppServerConfig.proxyURL,
          "/static/images/plane.react.svg"
        ),
        label: t("SendInvitesAgain"),
        onClick: this.onNotImplementedClick.bind(this, t("SendInvitesAgain")),
      },
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
        <MainButton
          isDisabled={false}
          isDropdown={true}
          text={t("Common:Actions")}
          model={menuModel}
          className="main-button_invitation-link people_main-button"
        />
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
    };
  })(
    withTranslation(["Article", "Common", "Translations"])(
      withLoader(observer(ArticleMainButtonContent))(<Loaders.MainButton />)
    )
  )
);
