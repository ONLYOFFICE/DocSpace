import React, { useState, useEffect, useCallback } from "react";
import styled, { css } from "styled-components";
import Error403 from "client/Error403";
import Error520 from "client/Error520";
//import ConnectClouds from "./ConnectedClouds";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import Submenu from "@docspace/components/submenu";
import CommonSettings from "./CommonSettings";
import AdminSettings from "./AdminSettings";
import { isDesktop } from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  margin-top: -22px;

  ${isDesktop &&
  css`
    margin-top: -19px;
  `}
`;

const SectionBodyContent = ({
  setting,
  isAdmin,
  enableThirdParty,
  settingsIsLoaded,
  isErrorSettings,
  history,
  isPersonal,
  t,
}) => {
  const [currentTab, setCurrentTab] = useState(0);

  const commonSettings = {
    id: "common",
    name: t("CommonSettings"),
    content: <CommonSettings t={t} />,
  };

  const adminSettings = {
    id: "admin",
    name: t("Common:AdminSettings"),
    content: <AdminSettings t={t} />,
  };

  // const connectedCloud = {
  //   id: "connected-clouds",
  //   name: t("ThirdPartySettings"),
  //   content: <ConnectClouds />,
  // };

  const data = [];

  if (isAdmin) {
    data.push(adminSettings);
  }

  data.push(commonSettings);

  // if (enableThirdParty) {
  //   data.push(connectedCloud);
  // }

  const onSelect = useCallback(
    (e) => {
      const { id } = e;

      if (id === setting) return;

      // setSelectedNode([key]);
      // setExpandSettingsTree([key]);

      history.push(
        combineUrl(AppServerConfig.proxyURL, config.homepage, `/settings/${id}`)
      );
    },
    [setting, history]
  );

  // const selectedTab = React.useCallback(() => {
  //   switch (setting) {
  //     case "common":
  //       return isAdmin ? 1 : 0;
  //     case "admin":
  //       return 0;
  //     case "connected-clouds":
  //       return isPersonal ? 0 : isAdmin ? 2 : 1;
  //     default:
  //       return isAdmin ? 1 : 0;
  //   }
  // }, [setting, isAdmin, isPersonal]);

  useEffect(() => {
    const path = location.pathname;
    const currentTab = data.findIndex((item) => path.includes(item.id));
    if (currentTab !== -1) {
      setCurrentTab(currentTab);
    }
  }, []);

  return !settingsIsLoaded ? null : (!enableThirdParty &&
      setting === "connected-clouds") ||
    (!isAdmin && setting === "admin") ? (
    <Error403 />
  ) : isErrorSettings ? (
    <Error520 />
  ) : (
    <StyledContainer>
      <Submenu
        data={data}
        startSelect={currentTab}
        onSelect={onSelect}
        //selectedItem={selectedTab()}
      />
    </StyledContainer>
  );
};

export default inject(({ auth, settingsStore }) => {
  const { enableThirdParty, settingsIsLoaded } = settingsStore;

  return {
    isAdmin: auth.isAdmin,
    enableThirdParty,
    settingsIsLoaded,
  };
})(observer(SectionBodyContent));
