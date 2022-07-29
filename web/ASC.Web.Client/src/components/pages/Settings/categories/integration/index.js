import React, { useEffect, useState } from "react";
import Submenu from "@appserver/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import config from "../../../../../../package.json";

import SSO from "./SingleSignOn";
import ThirdParty from "./ThirdPartyServicesSettings";

import AppLoader from "@appserver/common/components/AppLoader";
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
        `/settings/integration/${e.id}`
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
