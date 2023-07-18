import React, { useEffect, useState } from "react";
import { isMobile, isDesktop } from "react-device-detect";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";

import Certificates from "./Certificates";
import FieldMapping from "./FieldMapping";
import HideButton from "./sub-components/HideButton";
import IdpSettings from "./IdpSettings";
import ProviderMetadata from "./ProviderMetadata";
import StyledSsoPage from "./styled-containers/StyledSsoPageContainer";
import StyledSettingsSeparator from "SRC_DIR/pages/PortalSettings/StyledSettingsSeparator";
import SubmitResetButtons from "./SubmitButton";
import ToggleSSO from "./sub-components/ToggleSSO";

import BreakpointWarning from "SRC_DIR/components/BreakpointWarning";

const SERVICE_PROVIDER_SETTINGS = "serviceProviderSettings";
const SP_METADATA = "spMetadata";

const SingleSignOn = (props) => {
  const { load, serviceProviderSettings, spMetadata, isSSOAvailable } = props;
  const { t } = useTranslation(["SingleSignOn", "Settings"]);
  const [isSmallWindow, setIsSmallWindow] = useState(false);

  useEffect(() => {
    isSSOAvailable && load();
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  const onCheckView = () => {
    if (isDesktop && window.innerWidth < 795) {
      setIsSmallWindow(true);
    } else {
      setIsSmallWindow(false);
    }
  };

  if (isSmallWindow)
    return (
      <BreakpointWarning
        sectionName={t("Settings:SingleSignOn")}
        isSmallWindow
      />
    );

  if (isMobile)
    return <BreakpointWarning sectionName={t("Settings:SingleSignOn")} />;

  return (
    <StyledSsoPage
      hideSettings={serviceProviderSettings}
      hideMetadata={spMetadata}
      isSettingPaid={isSSOAvailable}
    >
      <ToggleSSO isSSOAvailable={isSSOAvailable} />

      <HideButton
        text={t("ServiceProviderSettings")}
        label={SERVICE_PROVIDER_SETTINGS}
        value={serviceProviderSettings}
        isDisabled={!isSSOAvailable}
      />

      <Box className="service-provider-settings">
        <IdpSettings />

        <Certificates provider="IdentityProvider" />

        <Certificates provider="ServiceProvider" />

        <FieldMapping />

        <SubmitResetButtons />
      </Box>

      <StyledSettingsSeparator />

      <HideButton
        text={t("SpMetadata")}
        label={SP_METADATA}
        value={spMetadata}
        isDisabled={!isSSOAvailable}
      />

      <Box className="sp-metadata">
        <ProviderMetadata />
      </Box>
    </StyledSsoPage>
  );
};

export default inject(({ auth, ssoStore }) => {
  const { currentQuotaStore } = auth;
  const { isSSOAvailable } = currentQuotaStore;

  const { load, serviceProviderSettings, spMetadata } = ssoStore;

  return {
    load,
    serviceProviderSettings,
    spMetadata,
    isSSOAvailable,
  };
})(observer(SingleSignOn));
