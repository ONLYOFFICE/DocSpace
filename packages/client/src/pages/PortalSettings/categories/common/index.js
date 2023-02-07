import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { inject, observer } from "mobx-react";
import Customization from "./customization";
import Branding from "./branding";
import Appearance from "./appearance";
import withLoading from "SRC_DIR/HOCs/withLoading";
import LoaderSubmenu from "./sub-components/loaderSubmenu";

const SubmenuCommon = (props) => {
  const {
    t,
    history,
    tReady,
    setIsLoadedSubmenu,
    loadBaseInfo,
    isLoadedSubmenu,
  } = props;
  const [currentTab, setCurrentTab] = useState(0);

  useEffect(() => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) {
      setCurrentTab(currentTab);
    }
  }, []);

  useEffect(() => {
    if (tReady) setIsLoadedSubmenu(true);
    if (isLoadedSubmenu) {
      load();
    }
  }, [tReady, isLoadedSubmenu]);

  const load = async () => {
    await loadBaseInfo();
  };

  const data = [
    {
      id: "customization",
      name: t("Common:SettingsGeneral"),
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
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/common/${e.id}`
      )
    );
  };

  if (!isLoadedSubmenu) return <LoaderSubmenu />;

  return (
    <Submenu
      data={data}
      startSelect={currentTab}
      onSelect={(e) => onSelect(e)}
    />
  );
};

export default inject(({ common }) => {
  const {
    isLoaded,
    setIsLoadedSubmenu,
    initSettings,
    isLoadedSubmenu,
  } = common;
  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
    isLoaded,
    setIsLoadedSubmenu,
    isLoadedSubmenu,
  };
})(
  withLoading(withRouter(withTranslation("Settings")(observer(SubmenuCommon))))
);
