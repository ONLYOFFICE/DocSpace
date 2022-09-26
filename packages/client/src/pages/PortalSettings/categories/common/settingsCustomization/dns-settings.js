import React, { useState, useEffect, useCallback } from "react";
import { withTranslation } from "react-i18next";
import HelpButton from "@docspace/components/help-button";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import history from "@docspace/common/history";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@docspace/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { DNSSettingsTooltip } from "../sub-components/common-tooltips";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";
const DNSSettings = (props) => {
  const {
    t,
    isMobileView,
    tReady,
    isLoaded,
    setIsLoadedDNSSettings,
    isLoadedPage,
    helpLink,
    theme,
    initSettings,
    setIsLoaded,
  } = props;
  const [hasScroll, setHasScroll] = useState(false);
  const isLoadedSetting = isLoaded && tReady;
  const [isCustomizationView, setIsCustomizationView] = useState(false);

  useEffect(() => {
    setDocumentTitle(t("DNSSettings"));

    if (!isLoaded) initSettings().then(() => setIsLoaded(true));

    const checkScroll = checkScrollSettingsBlock();
    checkInnerWidth();
    window.addEventListener("resize", checkInnerWidth);

    const scrollPortalName = checkScroll();

    if (scrollPortalName !== hasScroll) {
      setHasScroll(scrollPortalName);
    }

    // TODO: Remove div with height 64 and remove settings-mobile class
    const settingsMobile = document.getElementsByClassName(
      "settings-mobile"
    )[0];

    if (settingsMobile) {
      settingsMobile.style.display = "none";
    }

    return () => window.removeEventListener("resize", checkInnerWidth);
  }, []);

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedDNSSettings(isLoadedSetting);
  }, [isLoadedSetting]);

  const onSendRequest = () => {
    window.open("https://helpdesk.onlyoffice.com/hc/en-us/requests/new");
  };

  const checkInnerWidth = useCallback(() => {
    if (!isSmallTablet()) {
      setIsCustomizationView(true);

      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          "/portal-settings/common/customization"
        )
      );
    } else {
      setIsCustomizationView(false);
    }
  }, [isSmallTablet, setIsCustomizationView]);

  const tooltipDNSSettingsTooltip = (
    <DNSSettingsTooltip t={t} theme={theme} helpLink={helpLink} />
  );

  const settingsBlock = (
    <div className="settings-block">
      <div className="settings-block-description">{t("DNSSettingsMobile")}</div>
      <FieldContainer
        id="fieldContainerDNSSettings"
        className="field-container-width"
        labelText={`${t("YourCurrentDomain")}`}
        isVertical={true}
      >
        <TextInput
          id="textInputContainerDNSSettings"
          scale={true}
          value={location.hostname}
          isDisabled={true}
        />
      </FieldContainer>
    </div>
  );

  return !isLoadedPage ? (
    <LoaderCustomization dnsSettings={true} />
  ) : (
    <StyledSettingsComponent
      hasScroll={hasScroll}
      className="category-item-wrapper"
    >
      {isCustomizationView && !isMobileView && (
        <div className="category-item-heading">
          <div className="category-item-title">{t("DNSSettings")}</div>
          <HelpButton
            iconName="static/images/combined.shape.svg"
            size={12}
            tooltipContent={tooltipDNSSettingsTooltip}
          />
        </div>
      )}
      {(isMobileOnly && isSmallTablet()) || isSmallTablet() ? (
        <StyledScrollbar stype="mediumBlack">{settingsBlock}</StyledScrollbar>
      ) : (
        <> {settingsBlock}</>
      )}
      <div className="send-request-container">
        <Button
          label={t("Common:SendRequest")}
          className="save-cancel-buttons send-request-button"
          onClick={onSendRequest}
          primary
          size="small"
        />
      </div>
    </StyledSettingsComponent>
  );
};

export default inject(({ auth, common }) => {
  const { theme, helpLink } = auth.settingsStore;
  const {
    isLoaded,
    setIsLoadedDNSSettings,
    initSettings,
    setIsLoaded,
  } = common;
  return {
    theme,
    isLoaded,
    setIsLoadedDNSSettings,
    helpLink,
    initSettings,
    setIsLoaded,
  };
})(withLoading(withTranslation(["Settings", "Common"])(observer(DNSSettings))));
