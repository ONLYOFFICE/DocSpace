import React from "react";
import Error403 from "client/Error403";
import Error520 from "client/Error520";
import ConnectClouds from "./ConnectedClouds";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import TabsContainer from "@docspace/components/tabs-container";
import CommonSettings from "./CommonSettings";
import AdminSettings from "./AdminSettings";

const SectionBodyContent = ({
  setting,
  isAdmin,
  enableThirdParty,
  settingsIsLoaded,
  isErrorSettings,
  history,
  setExpandSettingsTree,
  setSelectedNode,
  isPersonal,
  t,
}) => {
  const commonSettings = {
    content: <CommonSettings t={t} />,
    key: "common",
    title: t("CommonSettings"),
  };

  const adminSettings = {
    content: <AdminSettings t={t} />,
    key: "admin",
    title: t("Common:AdminSettings"),
  };

  const connectedCloud = {
    content: <ConnectClouds />,
    key: "connected-clouds",
    title: t("ThirdPartySettings"),
  };

  const elements = [];

  if (isAdmin && !isPersonal) {
    elements.push(adminSettings);
  }

  if (!isPersonal) {
    elements.push(commonSettings);
  }

  if (enableThirdParty) {
    elements.push(connectedCloud);
  }

  const onSelect = React.useCallback(
    (data) => {
      const { key } = data;

      if (key === setting) return;

      setSelectedNode([key]);
      setExpandSettingsTree([key]);

      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/settings/${key}`
        )
      );
    },
    [setting, history, setExpandSettingsTree, setSelectedNode]
  );

  const selectedTab = React.useCallback(() => {
    switch (setting) {
      case "common":
        return isAdmin ? 1 : 0;
      case "admin":
        return 0;
      case "connected-clouds":
        return isPersonal ? 0 : isAdmin ? 2 : 1;
      default:
        return isAdmin ? 1 : 0;
    }
  }, [setting, isAdmin, isPersonal]);

  return !settingsIsLoaded ? null : (!enableThirdParty &&
      setting === "connected-clouds") ||
    (!isAdmin && setting === "admin") ||
    (isPersonal && setting !== "connected-clouds") ? (
    <Error403 />
  ) : isErrorSettings ? (
    <Error520 />
  ) : !isPersonal ? (
    <div>
      <TabsContainer
        elements={elements}
        onSelect={onSelect}
        selectedItem={selectedTab()}
      />
    </div>
  ) : (
    <div>{elements[0].content}</div>
  );
};

export default inject(({ auth, treeFoldersStore, settingsStore }) => {
  const {
    enableThirdParty,
    settingsIsLoaded,

    setExpandSettingsTree,
  } = settingsStore;

  const { setSelectedNode } = treeFoldersStore;
  return {
    isAdmin: auth.isAdmin,
    isPersonal: auth.settingsStore.personal,
    enableThirdParty,
    settingsIsLoaded,

    setExpandSettingsTree,
    setSelectedNode,
  };
})(observer(SectionBodyContent));
