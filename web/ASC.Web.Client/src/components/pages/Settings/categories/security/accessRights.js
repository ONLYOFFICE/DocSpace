import React, { Component } from "react";
import { Route } from "react-router-dom";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import { TabContainer } from "asc-web-components";

import OwnerSettings from "./sub-components/owner";
import AdminsSettings from "./sub-components/admins";
import ModulesSettings from "./sub-components/modules";

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

    //console.log("props", props);

    const url = props.history.location.pathname;
    const newUrl = url.split("/");
    const activeStatus = newUrl[newUrl.length - 1];

    let selectedTab = 0;
    if (activeStatus === "admins") {
      selectedTab = 1;
    } else if (activeStatus === "modules") {
      selectedTab = 2;
    }

    this.state = {
      selectedTab
    };
  }

  componentDidMount() {}

  onSelectPage = page => {
    const { history } = this.props;

    switch (page.key) {
      case "0":
        history.push("/settings/security/accessrights/owner");
        break;
      case "1":
        history.push("/settings/security/accessrights/admins");
        break;
      case "2":
        history.push("/settings/security/accessrights/modules");
        break;
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
              title: "Owner settings",
              content: <Route component={OwnerSettings} />
            },
            {
              key: "1",
              title: "Admins settings",
              content: <Route component={AdminsSettings} />
            },
            {
              key: "2",
              title: "Portals settings",
              content: <Route component={ModulesSettings} />
            }
          ]}
        </TabContainer>
      </MainContainer>
    );
  }
}

const AccessRightsContainer = withTranslation()(PureAccessRights);

const AccessRights = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <AccessRightsContainer {...props} />
    </I18nextProvider>
  );
};

export default connect(null, {})(withRouter(AccessRights));
