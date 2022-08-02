import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import SSO from "./SingleSignOn";
import ThirdParty from "./ThirdPartyServicesSettings";

import AppLoader from "@docspace/common/components/AppLoader";
import SSOLoader from "./sub-components/ssoLoader";

const IntegrationWrapper = (props) => {
  const { t, history, loadBaseInfo } = props;
  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

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

  if (!isLoading) return currentTab === 0 ? <SSOLoader /> : <AppLoader />;

  return <Submenu data={data} startSelect={currentTab} onSelect={onSelect} />;
};

export default inject(({ setup }) => {
  const { initSettings } = setup;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
  };
})(withTranslation("Settings")(withRouter(observer(IntegrationWrapper))));
