import React from "react";
import { inject, observer } from "mobx-react";

import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import withLoading from "SRC_DIR/HOCs/withLoading";

import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import Submenu from "@docspace/components/submenu";

import ThirdPartyServices from "./thirdPartyServicesSettings";
import PortalPlugins from "./PortalPlugins";

// const ThirdPartyServices = lazy(() => import("./thirdPartyServicesSettings"));

const PROXY_BASE_URL = combineUrl(
  AppServerConfig.proxyURL,
  "/portal-settings/integration"
);

const Integration = ({ t, history }) => {
  const [currentTab, setCurrentTab] = React.useState(0);

  React.useEffect(() => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) {
      setCurrentTab(currentTab);
    }
  }, []);

  const data = [
    {
      id: "third-party-services",
      name: "Third party services",
      content: <ThirdPartyServices />,
    },
    {
      id: "plugins",
      name: "Plugins",
      content: <PortalPlugins />,
    },
  ];

  const onSelect = (e) => {
    history.push(combineUrl(PROXY_BASE_URL, `/${e.id}`));
  };

  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
      isLoading={false}
    />
  );
};

export default inject(({ common }) => {
  const { isLoaded, setIsLoadedSubmenu } = common;
  return {
    isLoaded,
    setIsLoadedSubmenu,
  };
})(withLoading(withRouter(withTranslation("Settings")(observer(Integration)))));
