import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import styled, { css } from "styled-components";

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

const SectionBodyContent = ({ isErrorSettings, user }) => {
  const { t } = useTranslation(["FilesSettings", "Common"]);

  // const commonSettings = {
  //   id: "personal",
  //   name: t("Common:SettingsPersonal"),
  //   content: <PersonalSettings t={t} />,
  // };

  // const adminSettings = {
  //   id: "general",
  //   name: t("Common:SettingsGeneral"),
  //   content: <GeneralSettings t={t} />,
  // };

  // const data = [commonSettings];

  // const showAdminSettings = user.isAdmin || user.isOwner;

  // if (showAdminSettings) {
  //   data.unshift(adminSettings);
  // }

  // const onSelect = useCallback(
  //   (e) => {
  //     const { id } = e;

  //     if (id === setting) return;

  //     navigate(
  //       combineUrl(
  //         window.DocSpaceConfig?.proxy?.url,
  //         config.homepage,
  //         `/settings/${id}`
  //       )
  //     );
  //   },
  //   [setting, navigate]
  // );

  //const showAdminSettings = user.isAdmin || user.isOwner;

  return isErrorSettings ? (
    <Error520 />
  ) : (
    <StyledContainer>
      <PersonalSettings
        t={t}
        showTitle={true}
        showAdminSettings={false} //showAdminSettings
      />
    </StyledContainer>
  );
};

export default inject(({ auth, settingsStore }) => {
  const { settingsIsLoaded } = settingsStore;

  return {
    settingsIsLoaded,
    user: auth.userStore.user,
  };
})(observer(SectionBodyContent));
