import React, { useEffect } from "react";
import { isDesktop } from "react-device-detect";
import { inject, observer } from "mobx-react";

import Box from "@appserver/components/box";

import Certificates from "./Certificates";
import FieldMapping from "./FieldMapping";
import ForbiddenPage from "./sub-components/ForbiddenPage";
import HideButton from "./sub-components/HideButton";
import IdpSettings from "./IdpSettings";
import ProviderMetadata from "./ProviderMetadata";
import StyledSsoPage from "./styled-containers/StyledSsoPageContainer";
import SubmitResetButtons from "./SubmitButton";
import ToggleSSO from "./sub-components/ToggleSSO";

const SingleSignOn = (props) => {
  const { onPageLoad, serviceProviderSettings, spMetadata } = props;

  if (!isDesktop) return <ForbiddenPage />;

  useEffect(() => {
    onPageLoad();
  }, []);

  return (
    <StyledSsoPage
      hideSettings={serviceProviderSettings}
      hideMetadata={spMetadata}
    >
      <ToggleSSO />

      <HideButton
        label="serviceProviderSettings"
        value={serviceProviderSettings}
      />

      <Box className="service-provider-settings">
        <IdpSettings />

        <Certificates provider="IdentityProvider" />

        <Certificates provider="ServiceProvider" />

        <FieldMapping />

        <SubmitResetButtons />
      </Box>

      <hr className="separator" />

      <HideButton label="spMetadata" value={spMetadata} />

      <Box className="sp-metadata">
        <ProviderMetadata />
      </Box>
    </StyledSsoPage>
  );
};

export default inject(({ ssoStore }) => {
  const { onPageLoad, serviceProviderSettings, spMetadata } = ssoStore;

  return { onPageLoad, serviceProviderSettings, spMetadata };
})(observer(SingleSignOn));
