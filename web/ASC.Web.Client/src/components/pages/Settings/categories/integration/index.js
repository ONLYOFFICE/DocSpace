import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";

import { combineUrl } from "@appserver/common/utils";
import AppServerConfig from "@appserver/common/constants/AppServerConfig";
import AppLoader from "@appserver/common/components/AppLoader";

import Submenu from "@appserver/components/submenu";

import ThirdPartyServices from "./thirdPartyServicesSettings";
import PortalIntegration from "./portalIntegration";

import config from "../../../../../../package.json";

const Integration = (props) => {
  const { t, history } = props;
  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const data = [
    {
      id: "third-party-services",
      name: t("ThirdPartyAuthorization"),
      content: <ThirdPartyServices />,
    },
    {
      id: "portal-integration",
      name: "Portal Integration",
      content: <PortalIntegration />,
    },
  ];

  const load = async () => {
    const { loadBaseInfo } = props;

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

  if (!isLoading) return <AppLoader />;

  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default inject(({ setup }) => {
  const { initSettings } = setup;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
  };
})(withTranslation("Settings")(withRouter(observer(Integration))));
