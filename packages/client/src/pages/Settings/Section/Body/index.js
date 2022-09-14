import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
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
  //enableThirdParty,
  settingsIsLoaded,
  isErrorSettings,
  history,
}) => {
  const { t } = useTranslation(["FilesSettings", "Common"]);

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

      history.push(
        combineUrl(AppServerConfig.proxyURL, config.homepage, `/settings/${id}`)
      );
    },
    [setting, history]
  );

  return isErrorSettings ? (
    <Error520 />
  ) : (
    <StyledContainer>
      <Submenu
        data={data}
        startSelect={setting === "common" ? commonSettings : adminSettings}
        onSelect={onSelect}
      />
    </StyledContainer>
  );
};

export default inject(({ auth, settingsStore }) => {
  const { settingsIsLoaded } = settingsStore;

  return {
    isAdmin: auth.isAdmin,
    settingsIsLoaded,
  };
})(withRouter(observer(SectionBodyContent)));
