import React, { useEffect, useState, useTransition, Suspense } from "react";
import styled, { css } from "styled-components";
import Submenu from "@docspace/components/submenu";
import Badge from "@docspace/components/badge";
import Box from "@docspace/components/box";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";

import { useNavigate } from "react-router-dom";
import JavascriptSDK from "./JavascriptSDK";
import Webhooks from "./Webhooks";
import PluginPage from "./Plugins";
import Api from "./Api";

import { useTranslation } from "react-i18next";
import { isMobile, isMobileOnly } from "react-device-detect";
import AppLoader from "@docspace/common/components/AppLoader";
import SSOLoader from "./sub-components/ssoLoader";
import { WebhookConfigsLoader } from "./Webhooks/sub-components/Loaders";

const StyledSubmenu = styled(Submenu)`
  .sticky {
    margin-top: ${() => (isMobile ? "0" : "4px")};
    z-index: 201;
    ${() =>
      isMobileOnly &&
      css`
        top: 58px;
      `}
  }
`;

const DeveloperToolsWrapper = (props) => {
  const { loadBaseInfo, enablePlugins } = props;
  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);

  const { t, ready } = useTranslation([
    "JavascriptSdk",
    "Webhooks",
    "Settings",
    "WebPlugins",
  ]);
  const [isPending, startTransition] = useTransition();

  const sdkLabel = (
    <Box displayProp="flex" style={{ gap: "8px" }}>
      {t("JavascriptSdk")}
      <Box>
        <Badge
          label={t("Settings:BetaLabel")}
          backgroundColor="#7763F0"
          fontSize="9px"
          borderRadius="50px"
          noHover={true}
          isHovered={false}
        />
      </Box>
    </Box>
  );

  const pluginLabel = (
    <Box displayProp="flex" style={{ gap: "8px" }}>
      {t("WebPlugins:Plugins")}
      <Box>
        <Badge
          label={t("Settings:BetaLabel")}
          backgroundColor="#7763F0"
          fontSize="9px"
          borderRadius="50px"
          noHover={true}
          isHovered={false}
        />
      </Box>
    </Box>
  );

  const data = [
    {
      id: "api",
      name: t("Settings:Api"),
      content: <Api />,
    },
    {
      id: "javascript-sdk",
      name: sdkLabel,
      content: <JavascriptSDK />,
    },
    {
      id: "webhooks",
      name: t("Webhooks:Webhooks"),
      content: <Webhooks />,
    },
  ];

  if (enablePlugins) {
    data.push({
      id: "plugins",
      name: pluginLabel,
      content: <PluginPage />,
    });
  }

  const [currentTab, setCurrentTab] = useState(
    data.findIndex((item) => location.pathname.includes(item.id))
  );

  const load = async () => {
    //await loadBaseInfo();
  };

  useEffect(() => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) {
      setCurrentTab(currentTab);
    }
  }, []);

  useEffect(() => {
    ready && startTransition(load);
  }, [ready]);

  const onSelect = (e) => {
    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/developer-tools/${e.id}`
      )
    );
  };

  const loaders = [<SSOLoader />, <AppLoader />];

  return (
    <Suspense fallback={loaders[currentTab] || <AppLoader />}>
      <StyledSubmenu data={data} startSelect={currentTab} onSelect={onSelect} />
    </Suspense>
  );
};

export default inject(({ setup, auth }) => {
  const { initSettings } = setup;

  const { settingsStore } = auth;

  return {
    enablePlugins: settingsStore.enablePlugins,
    loadBaseInfo: async () => {
      await initSettings();
    },
  };
})(observer(DeveloperToolsWrapper));
