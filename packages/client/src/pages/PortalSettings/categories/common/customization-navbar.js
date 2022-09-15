import React, { useEffect } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import Link from "@docspace/components/link";
import { combineUrl } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@docspace/common/constants";
import withCultureNames from "@docspace/common/hoc/withCultureNames";
import history from "@docspace/common/history";
import { Base } from "@docspace/components/themes";
import LoaderCustomizationNavbar from "./sub-components/loaderCustomizationNavbar";
import { StyledArrowRightIcon } from "./Customization/StyledSettings";
import { withRouter } from "react-router";
const StyledComponent = styled.div`
  padding-top: 13px;

  .combo-button-label {
    max-width: 100%;
  }

  .category-item-wrapper {
    padding-bottom: 20px;

    .category-item-heading {
      padding-bottom: 8px;
      svg {
        padding-bottom: 5px;
      }
    }

    .category-item-description {
      color: #657077;
      font-size: 13px;
      max-width: 1024px;
      line-height: 20px;
    }

    .inherit-title-link {
      margin-right: 4px;
      font-size: 16px;
      font-weight: 700;
    }
    .link-learn-more {
      line-height: 15px;
      font-weight: 600;
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

const CustomizationNavbar = ({
  t,
  theme,
  helpUrlCommonSettings,
  isLoaded,
  tReady,
  setIsLoadedCustomizationNavbar,
  isLoadedPage,
}) => {
  const isLoadedSetting = isLoaded && tReady;
  useEffect(() => {
    if (isLoadedSetting) setIsLoadedCustomizationNavbar(isLoadedSetting);
  }, [isLoadedSetting]);

  const onClickLink = (e) => {
    e.preventDefault();
    history.push(e.target.pathname);
  };

  return !isLoadedPage ? (
    <LoaderCustomizationNavbar />
  ) : (
    <StyledComponent>
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            className="inherit-title-link header"
            onClick={onClickLink}
            truncate={true}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/portal-settings/common/customization/language-and-time-zone"
            )}
          >
            {t("StudioTimeLanguageSettings")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("LanguageAndTimeZoneSettingsDescription")}
        </Text>
        <Box paddingProp="10px 0 3px 0">
          <Link
            className="link-learn-more"
            color={theme.client.settings.common.linkColorHelp}
            target="_blank"
            isHovered={true}
            href={helpUrlCommonSettings}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
      </div>
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/portal-settings/common/customization/welcome-page-settings"
            )}
          >
            {t("CustomTitlesWelcome")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("CustomTitlesSettingsDescription")}
        </Text>
      </div>

      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/portal-settings/common/customization/dns-settings"
            )}
          >
            {t("DNSSettings")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("DNSSettingsDescription")}
        </Text>
        <Box paddingProp="10px 0 3px 0">
          <Link
            color={theme.client.settings.common.linkColorHelp}
            target="_blank"
            isHovered={true}
            href={helpUrlCommonSettings}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
      </div>

      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/portal-settings/common/customization/portal-renaming"
            )}
          >
            {t("PortalRenaming")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("PortalRenamingDescription")}
        </Text>
      </div>
    </StyledComponent>
  );
};

export default inject(({ auth, common }) => {
  const { helpUrlCommonSettings, theme } = auth.settingsStore;
  const { isLoaded, setIsLoadedCustomizationNavbar } = common;
  return {
    theme,
    helpUrlCommonSettings,
    isLoaded,
    setIsLoadedCustomizationNavbar,
  };
})(
  withRouter(
    withCultureNames(
      observer(withTranslation(["Settings", "Common"])(CustomizationNavbar))
    )
  )
);
