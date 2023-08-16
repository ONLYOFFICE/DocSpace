import React, { useEffect, useState } from "react";
import styled, { css } from "styled-components";
import Submenu from "@docspace/components/submenu";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { withRouter } from "react-router";

import JavascriptSDK from "./JavascriptSDK";
import Api from "./Api";

import AppLoader from "@docspace/common/components/AppLoader";

import { useTranslation } from "react-i18next";
import { isMobile, isMobileOnly } from "react-device-detect";

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
  const { loadBaseInfo, history } = props;
  const [currentTab, setCurrentTab] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const { t, ready } = useTranslation(["JavascriptSdk", "Settings"]);

  const data = [
    {
      id: "api",
      name: t("Settings:Api"),
      content: <Api />,
    },
    {
      id: "javascript-sdk",
      name: t("JavascriptSdk"),
      content: <JavascriptSDK />,
    },
  ];

  const load = async () => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) setCurrentTab(currentTab);

    await loadBaseInfo();
    setIsLoading(true);
  };

  useEffect(() => {
    load();
  }, []);

  const onSelect = (e) => {
    history.push(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/portal-settings/developer-tools/${e.id}`
      )
    );
  };

  if (!isLoading && !ready) return <AppLoader />;

  return (
    <StyledSubmenu data={data} startSelect={currentTab} onSelect={onSelect} />
  );
};

export default inject(({ setup }) => {
  const { initSettings } = setup;

  return {
    loadBaseInfo: async () => {
      await initSettings();
    },
  };
})(withRouter(observer(DeveloperToolsWrapper)));
