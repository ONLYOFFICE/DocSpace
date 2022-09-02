import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import withCultureNames from "@docspace/common/hoc/withCultureNames";
import LanguageAndTimeZone from "./settingsCustomization/language-and-time-zone";
import WelcomePageSettings from "./settingsCustomization/welcome-page-settings";
import PortalRenaming from "./settingsCustomization/portal-renaming";
import DNSSettings from "./settingsCustomization/dns-settings";
import { isSmallTablet } from "@docspace/components/utils/device";
import CustomizationNavbar from "./customization-navbar";
import { Base } from "@docspace/components/themes";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import LoaderDescriptionCustomization from "./sub-components/loaderDescriptionCustomization";
import { withRouter } from "react-router";
import withLoading from "SRC_DIR/HOCs/withLoading";

const StyledComponent = styled.div`
  width: 100%;

  .combo-button-label {
    max-width: 100%;
  }

  .settings-block {
    margin-bottom: 24px;
  }

  .category-description {
    line-height: 20px;
    color: #657077;
    margin-bottom: 20px;
    max-width: 700px;
  }

  .category-item-wrapper:not(:last-child) {
    border-bottom: 1px solid #eceef1;
    margin-bottom: 24px;
    padding-bottom: 24px;
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

  @media (min-width: 600px) {
    .settings-block {
      max-width: 350px;
      height: auto;
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

const Customization = (props) => {
  const { t, isLoaded, tReady, setIsLoadedCustomization, isLoadedPage } = props;
  const [mobileView, setMobileView] = useState(true);

  const isLoadedSetting = isLoaded && tReady;

  useEffect(() => {
    setDocumentTitle(t("Customization"));
    window.addEventListener("resize", checkInnerWidth);

    return () => window.removeEventListener("resize", checkInnerWidth);
  }, []);

  useEffect(() => {
    if (isLoadedSetting) {
      setIsLoadedCustomization(isLoadedSetting);
    }
  }, [isLoadedSetting]);

  const checkInnerWidth = () => {
    if (isSmallTablet()) {
      setMobileView(true);
    } else {
      setMobileView(false);
    }
  };

  const isMobile = !!(isSmallTablet() && mobileView);

  console.log("isLoadedPage", isLoadedPage);

  return isMobile ? (
    <CustomizationNavbar isLoadedPage={isLoadedPage} />
  ) : (
    <StyledComponent>
      {!isLoadedPage ? (
        <LoaderDescriptionCustomization />
      ) : (
        <div className="category-description">{`${t(
          "Settings:CustomizationDescription"
        )}`}</div>
      )}
      <LanguageAndTimeZone isMobileView={isMobile} />
      <WelcomePageSettings isMobileView={isMobile} />
      <DNSSettings isMobileView={isMobile} />
      <PortalRenaming isMobileView={isMobile} />
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
  withLoading(
    withRouter(withTranslation(["Settings", "Common"])(observer(Customization)))
  )
);
