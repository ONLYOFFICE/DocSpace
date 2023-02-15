import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import Error520 from "client/Error520";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import Submenu from "@docspace/components/submenu";
import PersonalSettings from "./CommonSettings";
import GeneralSettings from "./AdminSettings";
import { tablet } from "@docspace/components/utils/device";
import { isMobile } from "react-device-detect";

const StyledContainer = styled.div`
  margin-top: -22px;

  @media ${tablet} {
    margin-top: 0px;
  }

  ${isMobile &&
  css`
    margin-top: 0px;
  `}
`;

const SectionBodyContent = ({ isErrorSettings, history, user }) => {
  const { t } = useTranslation(["FilesSettings", "Common"]);

  const setting = window.location.pathname.endsWith("/settings/common")
    ? "common"
    : "admin";

  const commonSettings = {
    id: "common",
    name: t("Common:SettingsPersonal"),
    content: <PersonalSettings t={t} />,
  };

  const adminSettings = {
    id: "admin",
    name: t("Common:SettingsGeneral"),
    content: <GeneralSettings t={t} />,
  };

  const data = [adminSettings, commonSettings];

  const onSelect = useCallback(
    (e) => {
      const { id } = e;

      if (id === setting) return;

      history.push(
        combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          config.homepage,
          `/settings/${id}`
        )
      );
    },
    [setting, history]
  );

  const showAdminSettings = user.isAdmin || user.isOwner;

  return isErrorSettings ? (
    <Error520 />
  ) : (
    <StyledContainer>
      {!showAdminSettings ? (
        <PersonalSettings
          t={t}
          showTitle={true}
          showAdminSettings={showAdminSettings}
        />
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
    settingsIsLoaded,
    user: auth.userStore.user,
  };
})(withRouter(observer(SectionBodyContent)));
