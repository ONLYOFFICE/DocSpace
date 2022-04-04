import React from "react";
import styled, { css } from "styled-components";

import Error403 from "studio/Error403";
import Error520 from "studio/Error520";
import ConnectClouds from "./ConnectedClouds";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../package.json";
import TabsContainer from "@appserver/components/tabs-container";
import CommonSettings from "./CommonSettings";
import AdminSettings from "./AdminSettings";

import { tablet } from "@appserver/components/utils/device";
import { isMobile } from "react-device-detect";

const StyledContainer = styled.div`
  position: absolute;
  top: 3px;

  width: calc(100% - 40px);

  height: auto;

  @media ${tablet} {
    position: static;
    width: calc(100% - 32px);
  }

  ${isMobile &&
  css`
    position: static;
    width: calc(100% - 32px);
  `}
`;

const SectionBodyContent = ({
  setting,
  isAdmin,
  enableThirdParty,
  settingsIsLoaded,
  isErrorSettings,
  history,
  setExpandSettingsTree,
  setSelectedNode,
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

  if (isAdmin) {
    elements.push(adminSettings);
  }

  elements.push(commonSettings);

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
        return 1;
      case "admin":
        return 0;
      case "connected-clouds":
        return 2;
      default:
        return 1;
    }
  }, [setting]);

  return !settingsIsLoaded ? null : (!enableThirdParty &&
      setting === "thirdParty") ||
    (!isAdmin && setting === "admin") ? (
    <Error403 />
  ) : isErrorSettings ? (
    <Error520 />
  ) : (
    <StyledContainer>
      <TabsContainer
        elements={elements}
        onSelect={onSelect}
        selectedItem={selectedTab()}
      />
    </StyledContainer>
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
    enableThirdParty,
    settingsIsLoaded,

    setExpandSettingsTree,
    setSelectedNode,
  };
})(observer(SectionBodyContent));
