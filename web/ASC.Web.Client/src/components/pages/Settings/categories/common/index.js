import React, { useEffect, useState } from "react";
import Submenu from "@appserver/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import config from "../../../../../../package.json";
import { inject, observer } from "mobx-react";
import Customization from "./customization";
import Branding from "./branding";
import Appearance from "./appearance";
import withLoading from "../../../../../HOCs/withLoading";

const SubmenuCommon = (props) => {
  const {
    t,
    history,
    isLoaded,
    tReady,
    setIsLoadedSubmenu,
    isLoadedPage,
  } = props;
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
      name: "Branding",
      content: <Branding />,
    },
    {
      id: "appearance",
      name: "Appearance",
      content: <Appearance />,
    },
  ];

  const onSelect = (e) => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/settings/common/${e.id}`
      )
    );
  };

  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
      isLoading={!isLoadedPage}
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
