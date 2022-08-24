import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { isMobile } from "react-device-detect";
import PortalIntegration from "./portalIntegration";

import SSO from "./SingleSignOn";
import ThirdParty from "./ThirdPartyServicesSettings";
import PortalPlugins from "./PortalPlugins";

import AppLoader from "@docspace/common/components/AppLoader";
import SSOLoader from "./sub-components/ssoLoader";

const IntegrationWrapper = (props) => {
  const { t, tReady, history, loadBaseInfo, enablePlugins } = props;
  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const integrationData = {
    id: "portal-integration",
    name: "Portal Integration",
    content: <PortalIntegration />,
  };

  const pluginData = {
    id: "plugins",
    name: "Plugins",
    content: <PortalPlugins />,
  };

  const data = [
    {
      id: "single-sign-on",
      name: t("SingleSignOn"),
      content: <SSO />,
    },
    {
      id: "third-party-services",
      name: t("ThirdPartyTitle"),
      content: <ThirdParty />,
    },
  ];

  if (!isMobile) {
    data.push(integrationData);
    enablePlugins && data.push(pluginData);
  }

  const load = async () => {
    await loadBaseInfo();
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) setCurrentTab(currentTab);
    setIsLoading(true);
  };

  useEffect(() => {
    load();
  }, []);

  const onSelect = (e) => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/portal-settings/integration/${e.id}`
      )
    );
  };

  if (!isLoading && !tReady)
    return currentTab === 0 ? <SSOLoader /> : <AppLoader />;

  return <Submenu data={data} startSelect={currentTab} onSelect={onSelect} />;
};

export default inject(({ setup, auth }) => {
  const { initSettings } = setup;

  const { enablePlugins } = auth.settingsStore;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
    enablePlugins,
  };
})(
  withTranslation(["Settings", "SingleSignOn"])(
    withRouter(observer(IntegrationWrapper))
  )
);
