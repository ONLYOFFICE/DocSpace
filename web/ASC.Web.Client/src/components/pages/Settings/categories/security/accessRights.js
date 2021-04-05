import React, { Component } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import TabContainer from "@appserver/components/tabs-container";

import OwnerSettings from "./sub-components/owner";
import AdminsSettings from "./sub-components/admins";
// import ModulesSettings from "./sub-components/modules";

import { setDocumentTitle } from "../../../../../helpers/utils";
import { inject } from "mobx-react";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const MainContainer = styled.div`
  width: 100%;

  .settings_tabs {
    padding-bottom: 16px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
  }
`;

class PureAccessRights extends Component {
  constructor(props) {
    super(props);

    const { t } = props;

    setDocumentTitle(t("ManagementCategorySecurity"));

    const url = props.history.location.pathname;
    const newUrl = url.split("/");
    const activeStatus = newUrl[newUrl.length - 1];

    let selectedTab = 0;
    if (activeStatus === "admins") {
      selectedTab = 1;
    }
    // else if (activeStatus === "modules") {
    //   selectedTab = 2;
    // }

    this.state = {
      selectedTab,
    };
  }

  onSelectPage = (page) => {
    const { history } = this.props;

    switch (page.key) {
      case "0":
        history.push(
          combineUrl(
            AppServerConfig.proxyURL,
            "/settings/security/accessrights/owner"
          )
        );
        break;
      case "1":
        history.push(
          combineUrl(
            AppServerConfig.proxyURL,
            "/settings/security/accessrights/admins"
          )
        );
        break;
      // case "2":
      //   history.push(combineUrl(AppServerConfig.proxyURL, "/settings/security/accessrights/modules"));
      //   break;
      default:
        break;
    }
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { isLoading, selectedTab } = this.state;
    if (
      isLoading === nextState.isLoading &&
      selectedTab === nextState.selectedTab
    ) {
      return false;
    }
    return true;
  }

  render() {
    const { isLoading, selectedTab } = this.state;
    const { t } = this.props;

    console.log("accessRight render_");

    return (
      <MainContainer>
        <OwnerSettings />
        {/* <TabContainer
          classNem="settings_tabs"
          selectedItem={selectedTab}
          isDisabled={isLoading}
          onSelect={this.onSelectPage}
          elements={[
            {
              key: "0",
              title: t("OwnerSettings"),
              content: <OwnerSettings />,
            },
            {
              key: "1",
              title: t("AdminsSettings"),
              content: <AdminsSettings />,
              dis
            },
            {
              key: "2",
              title: "Portals settings",
              content: <ModulesSettings />
            }
          ]}
        /> */}
      </MainContainer>
    );
  }
}

export default inject(({ auth }) => ({
  organizationName: auth.settingsStore.organizationName,
}))(withTranslation("Settings")(withRouter(PureAccessRights)));
