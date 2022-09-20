import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { inject, observer } from "mobx-react";
import Customization from "./customization";
import Branding from "./branding";
import Appearance from "./appearance";
import withLoading from "SRC_DIR/HOCs/withLoading";
import LoaderSubmenu from "./sub-components/loaderSubmenu";

const SubmenuCommon = (props) => {
  const { t, history, isLoaded, tReady, setIsLoadedSubmenu } = props;
  const [currentTab, setCurrentTab] = useState(0);

  const isLoadedSetting = isLoaded && tReady;

  useEffect(() => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) {
      setCurrentTab(currentTab);
    }
  }, []);

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedSubmenu(isLoadedSetting);
  }, [isLoadedSetting]);

  const data = [
    {
      id: "customization",
      name: t("Customization"),
      content: <Customization />,
    },
    {
      id: "branding",
      name: t("Branding"),
      content: <Branding />,
    },
    {
      id: "appearance",
      name: t("Appearance"),
      content: <Appearance />,
    },
  ];

  const onSelect = (e) => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/portal-settings/common/${e.id}`
      )
    );
  };

  if (!isLoadedSetting) return <LoaderSubmenu />;

  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default inject(({ common }) => {
  const { isLoaded, setIsLoadedSubmenu } = common;
  return {
    isLoaded,
    setIsLoadedSubmenu,
  };
})(
  withLoading(withRouter(withTranslation("Settings")(observer(SubmenuCommon))))
);
