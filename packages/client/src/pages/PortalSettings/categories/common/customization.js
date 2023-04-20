import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import withCultureNames from "@docspace/common/hoc/withCultureNames";
import LanguageAndTimeZone from "./Customization/language-and-time-zone";
import WelcomePageSettings from "./Customization/welcome-page-settings";
import PortalRenaming from "./Customization/portal-renaming";
import DNSSettings from "./Customization/dns-settings";
import CustomizationNavbar from "./customization-navbar";
import { Base } from "@docspace/components/themes";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import LoaderDescriptionCustomization from "./sub-components/loaderDescriptionCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";
import StyledSettingsSeparator from "SRC_DIR/pages/PortalSettings/StyledSettingsSeparator";

const StyledComponent = styled.div`
  width: 100%;

  .combo-button-label {
    max-width: 100%;
  }

  .category-description {
    margin-top: 5px;
    line-height: 20px;
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
    margin-bottom: 20px;
    max-width: 700px;
  }

  .category-item-description {
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
    font-size: 12px;
    max-width: 1024px;
  }

  .category-item-heading {
    display: flex;
    align-items: center;
    padding-bottom: 16px;
  }

  .category-item-title {
    font-weight: bold;
    font-size: 16px;
    line-height: 22px;
    margin-right: 4px;
  }

  .settings-block {
    margin-bottom: 24px;
  }

  @media (min-width: 600px) {
    .settings-block {
      max-width: 350px;
      height: auto;
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

const Customization = (props) => {
  const {
    t,
    isLoaded,
    tReady,
    setIsLoadedCustomization,
    isLoadedPage,
    viewMobile,
  } = props;

  const isLoadedSetting = isLoaded && tReady;

  useEffect(() => {
    setDocumentTitle(t("Customization"));
  }, []);

  useEffect(() => {
    if (isLoadedSetting) {
      setIsLoadedCustomization(isLoadedSetting);
    }
  }, [isLoadedSetting]);

  return viewMobile ? (
    <CustomizationNavbar isLoadedPage={isLoadedPage} />
  ) : (
    <StyledComponent>
      {!isLoadedPage ? (
        <LoaderDescriptionCustomization />
      ) : (
        <div className="category-description">
          {t("Settings:CustomizationDescription")}
        </div>
      )}
      <LanguageAndTimeZone isMobileView={viewMobile} />
      <StyledSettingsSeparator />
      <WelcomePageSettings isMobileView={viewMobile} />
      <StyledSettingsSeparator />
      <DNSSettings isMobileView={viewMobile} />
      <StyledSettingsSeparator />
      <PortalRenaming isMobileView={viewMobile} />
    </StyledComponent>
  );
};

export default inject(({ common }) => {
  const { isLoaded, setIsLoadedCustomization } = common;

  return {
    isLoaded,
    setIsLoadedCustomization,
  };
})(
  withLoading(withTranslation(["Settings", "Common"])(observer(Customization)))
);
