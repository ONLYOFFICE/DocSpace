import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import Error520 from "client/Error520";
//import ConnectClouds from "./ConnectedClouds";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import Submenu from "@docspace/components/submenu";
import CommonSettings from "./CommonSettings";
import AdminSettings from "./AdminSettings";
import { tablet } from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  margin-top: -22px;

  @media ${tablet} {
    margin-top: 0px;
  }
`;

const SectionBodyContent = ({ isAdmin, isErrorSettings, history }) => {
  const { t } = useTranslation(["FilesSettings", "Common"]);

  const setting = window.location.pathname.endsWith("/settings/common")
    ? "common"
    : "admin";

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

  const data = [adminSettings, commonSettings];

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
      {!isAdmin ? (
        <CommonSettings t={t} showTitle={true} />
      ) : (
        <Submenu
          data={data}
          startSelect={setting === "common" ? commonSettings : adminSettings}
          onSelect={onSelect}
        />
      )}
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
