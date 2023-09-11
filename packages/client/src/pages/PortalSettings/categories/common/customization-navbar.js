import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import withCultureNames from "@docspace/common/hoc/withCultureNames";
import { Base } from "@docspace/components/themes";

import LoaderCustomizationNavbar from "./sub-components/loaderCustomizationNavbar";
import MobileCategoryWrapper from "../../components/MobileCategoryWrapper";

const StyledComponent = styled.div`
  .combo-button-label {
    max-width: 100%;
  }
`;

StyledComponent.defaultProps = { theme: Base };

const CustomizationNavbar = ({
  t,
  isLoaded,
  tReady,
  setIsLoadedCustomizationNavbar,
  isLoadedPage,
  isSettingPaid,
}) => {
  const isLoadedSetting = isLoaded && tReady;
  const navigate = useNavigate();

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedCustomizationNavbar(isLoadedSetting);
  }, [isLoadedSetting]);

  const onClickLink = (e) => {
    e.preventDefault();
    navigate(e.target.pathname);
  };

  return !isLoadedPage ? (
    <LoaderCustomizationNavbar />
  ) : (
    <StyledComponent>
      <MobileCategoryWrapper
        title={t("StudioTimeLanguageSettings")}
        subtitle={t("LanguageAndTimeZoneSettingsNavDescription")}
        url="/portal-settings/customization/general/language-and-time-zone"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("CustomTitlesWelcome")}
        subtitle={t("CustomTitlesSettingsNavDescription")}
        url="/portal-settings/customization/general/welcome-page-settings"
        onClickLink={onClickLink}
      />
      <MobileCategoryWrapper
        title={t("DNSSettings")}
        subtitle={t("DNSSettingsNavDescription")}
        url="/portal-settings/customization/general/dns-settings"
        onClickLink={onClickLink}
        withPaidBadge={!isSettingPaid}
        badgeLabel={t("Common:Paid")}
      />
      <MobileCategoryWrapper
        title={t("PortalRenaming")}
        subtitle={t("PortalRenamingNavDescription")}
        url="/portal-settings/customization/general/portal-renaming"
        onClickLink={onClickLink}
      />
    </StyledComponent>
  );
};

export default inject(({ common }) => {
  const { isLoaded, setIsLoadedCustomizationNavbar } = common;
  return {
    isLoaded,
    setIsLoadedCustomizationNavbar,
  };
})(
  withCultureNames(
    observer(withTranslation(["Settings", "Common"])(CustomizationNavbar))
  )
);
