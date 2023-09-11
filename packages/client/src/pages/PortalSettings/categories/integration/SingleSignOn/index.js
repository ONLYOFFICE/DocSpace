import { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";

import HideButton from "./sub-components/HideButton";
import SPSettings from "./SPSettings";
import ProviderMetadata from "./ProviderMetadata";
import StyledSsoPage from "./styled-containers/StyledSsoPageContainer";
import StyledSettingsSeparator from "SRC_DIR/pages/PortalSettings/StyledSettingsSeparator";
import ToggleSSO from "./sub-components/ToggleSSO";
import SSOLoader from "./sub-components/ssoLoader";

import MobileView from "./MobileView";
import { useIsMobileView } from "../../../utils/useIsMobileView";

const SERVICE_PROVIDER_SETTINGS = "serviceProviderSettings";
const SP_METADATA = "spMetadata";

const SingleSignOn = (props) => {
  const {
    init,
    serviceProviderSettings,
    spMetadata,
    isSSOAvailable,
    setDocumentTitle,
    isInit,
  } = props;
  const { t } = useTranslation(["SingleSignOn", "Settings"]);
  const isMobileView = useIsMobileView();

  useEffect(() => {
    isSSOAvailable && init();
    setDocumentTitle(t("Settings:SingleSignOn"));
  }, []);

  if (!isInit) return <SSOLoader />;

  return (
    <StyledSsoPage
      hideSettings={serviceProviderSettings}
      hideMetadata={spMetadata}
      isSettingPaid={isSSOAvailable}
    >
      <ToggleSSO isSSOAvailable={isSSOAvailable} />
      {isMobileView ? (
        <MobileView />
      ) : (
        <>
          <HideButton
            id="sp-settings-hide-button"
            text={t("ServiceProviderSettings")}
            label={SERVICE_PROVIDER_SETTINGS}
            value={serviceProviderSettings}
            isDisabled={!isSSOAvailable}
          />

          <SPSettings />
          <StyledSettingsSeparator />

          <HideButton
            id="sp-metadata-hide-button"
            text={t("SpMetadata")}
            label={SP_METADATA}
            value={spMetadata}
            isDisabled={!isSSOAvailable}
          />

          <Box className="sp-metadata">
            <ProviderMetadata />
          </Box>
        </>
      )}
    </StyledSsoPage>
  );
};

export default inject(({ auth, ssoStore }) => {
  const { currentQuotaStore, setDocumentTitle } = auth;
  const { isSSOAvailable } = currentQuotaStore;

  const { init, serviceProviderSettings, spMetadata, isInit } = ssoStore;

  return {
    init,
    serviceProviderSettings,
    spMetadata,
    isSSOAvailable,
    setDocumentTitle,
    isInit,
  };
})(observer(SingleSignOn));
