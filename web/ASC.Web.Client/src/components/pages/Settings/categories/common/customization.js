import React, { useEffect, useState } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import LanguageAndTimeZone from "./settingsCustomization/language-and-time-zone";
import WelcomePageSettings from "./settingsCustomization/welcome-page-settings";
import PortalRenaming from "./settingsCustomization/portal-renaming";
import { isSmallTablet } from "@appserver/components/utils/device";
import CustomizationNavbar from "./customization-navbar";
import { Base } from "@appserver/components/themes";
import { setDocumentTitle } from "../../../../../helpers/utils";
import LoaderDescriptionCustomization from "./sub-components/loaderDescriptionCustomization";

const StyledComponent = styled.div`
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
    color: ${(props) => props.theme.studio.settings.common.descriptionColor};
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

const Customization = ({ t, setIsLoadingArticleSettings }) => {
  const [mobileView, setMobileView] = useState(true);
  const [isLoadingCustomization, setIsLoadingCustomization] = useState(false);

  const checkInnerWidth = () => {
    if (isSmallTablet()) {
      setMobileView(true);
    } else {
      setMobileView(false);
    }
  };

  useEffect(() => {
    setDocumentTitle(t("Customization"));
    //TODO: Add method to get the portal name
    setIsLoadingArticleSettings(true);
    setTimeout(() => {
      setIsLoadingCustomization(false);
      setIsLoadingArticleSettings(isLoadingCustomization);
    }, 3000);

    window.addEventListener("resize", checkInnerWidth);
    return () => window.removeEventListener("resize", checkInnerWidth);
  }, []);

  const isMobile = !!(isSmallTablet() && mobileView);

  return isMobile ? (
    <CustomizationNavbar />
  ) : (
    <StyledComponent>
      <div className="category-description">{`${t(
        "Settings:CustomizationDescription"
      )}`}</div>
      {/* <LoaderDescriptionCustomization /> */}
      <LanguageAndTimeZone
        isLoadingCustomization={isLoadingCustomization}
        isMobileView={isMobile}
      />
      <WelcomePageSettings isMobileView={isMobile} />
      <PortalRenaming isMobileView={isMobile} />
    </StyledComponent>
  );
};

export default inject(({ setup }) => {
  const { setIsLoadingArticleSettings } = setup;

  return {
    setIsLoadingArticleSettings,
  };
})(
  withCultureNames(
    withTranslation(["Settings", "Common"])(observer(Customization))
  )
);
