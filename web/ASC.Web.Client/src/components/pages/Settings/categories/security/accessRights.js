import React, { Component, useEffect } from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
//import i18n from "../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import { TabContainer } from "asc-web-components";
import { utils } from "asc-web-common";

import OwnerSettings from "./sub-components/owner";
import AdminsSettings from "./sub-components/admins";
// import ModulesSettings from "./sub-components/modules";

import { createI18N } from "../../../../../helpers/i18n";
import { setDocumentTitle } from "../../../../../helpers/utils";
const i18n = createI18N({
  page: "Settings",
  localesPath: "pages/Settings",
});

const { changeLanguage } = utils;

const MainContainer = styled.div`
  padding-bottom: 16px;
  width: 100%;

  .page_loader {
    position: fixed;
    left: 50%;
  }
`;

class PureAccessRights extends Component {
  constructor(props) {
    super(props);

    const { t, organizationName } = props;

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
        history.push("/settings/security/accessrights/owner");
        break;
      case "1":
        history.push("/settings/security/accessrights/admins");
        break;
      // case "2":
      //   history.push("/settings/security/accessrights/modules");
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
        <TabContainer
          selectedItem={selectedTab}
          isDisabled={isLoading}
          onSelect={this.onSelectPage}
        >
          {[
            {
              key: "0",
              title: t("OwnerSettings"),
              content: <OwnerSettings />,
            },
            {
              key: "1",
              title: t("AdminsSettings"),
              content: <AdminsSettings />,
            },
            // {
            //   key: "2",
            //   title: "Portals settings",
            //   content: <ModulesSettings />
            // }
          ]}
        </TabContainer>
      </MainContainer>
    );
  }
}

function mapStateToProps(state) {
  const { organizationName } = state.auth.settings;
  return {
    organizationName,
  };
}

const AccessRightsContainer = connect(mapStateToProps)(
  withTranslation()(PureAccessRights)
);

const AccessRights = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <I18nextProvider i18n={i18n}>
      <AccessRightsContainer {...props} />
    </I18nextProvider>
  );
};

export default withRouter(AccessRights);
