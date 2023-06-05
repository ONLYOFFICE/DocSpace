import React, { useEffect, useState } from "react";
import Submenu from "@docspace/components/submenu";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import { useNavigate } from "react-router-dom";

import JavascriptSDK from "./JavascriptSDK";
import Webhooks from "./Webhooks";

import AppLoader from "@docspace/common/components/AppLoader";
import SSOLoader from "./sub-components/ssoLoader";
import { WebhookConfigsLoader } from "./Webhooks/sub-components/Loaders";

import { useTranslation } from "react-i18next";

const DeveloperToolsWrapper = (props) => {
  const { loadBaseInfo, developerToolsTab, setTab } = props;
  const [currentTab, setCurrentTab] = useState(developerToolsTab);
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const { t, ready } = useTranslation(["JavascriptSdk", "Webhooks"]);

  const data = [
    {
      id: "javascript-sdk",
      name: t("JavascriptSdk"),
      content: <JavascriptSDK />,
    },
    {
      id: "webhooks",
      name: t("Webhooks:Webhooks"),
      content: <Webhooks />,
    },
  ];

  const load = async () => {
    await loadBaseInfo();
    setIsLoading(true);
  };

  useEffect(() => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) {
      setCurrentTab(currentTab);
      setTab(currentTab);
    }

    load();
  }, []);

  const onSelect = (e) => {
    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/developer-tools/${e.id}`,
      ),
    );
  };

  if (!isLoading && !ready)
    return currentTab === 0 ? (
      <SSOLoader />
    ) : currentTab === 1 ? (
      <WebhookConfigsLoader />
    ) : (
      <AppLoader />
    );

  return <Submenu data={data} startSelect={currentTab} onSelect={onSelect} />;
};

export default inject(({ setup, webhooksStore }) => {
  const { initSettings } = setup;
  const { developerToolsTab, setTab } = webhooksStore;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
    developerToolsTab,
    setTab,
  };
})(observer(DeveloperToolsWrapper));
