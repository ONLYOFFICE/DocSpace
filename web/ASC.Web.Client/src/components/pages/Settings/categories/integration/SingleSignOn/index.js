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
  const { onPageLoad, ServiceProviderSettings, SPMetadata } = props;

  if (!isDesktop) return <ForbiddenPage />;

  useEffect(() => {
    onPageLoad();
  }, []);

  return (
    <StyledSsoPage
      hideSettings={ServiceProviderSettings}
      hideMetadata={SPMetadata}
    >
      <ToggleSSO />

      <HideButton
        label="ServiceProviderSettings"
        value={ServiceProviderSettings}
      />

      <Box className="service-provider-settings">
        <IdpSettings />

        <Certificates provider="IdentityProvider" />

        <Certificates provider="ServiceProvider" />

        <FieldMapping />

        <SubmitResetButtons />
      </Box>

      <hr className="separator" />

      <HideButton label="SPMetadata" value={SPMetadata} />

      <Box className="sp-metadata">
        <ProviderMetadata />
      </Box>
    </StyledSsoPage>
  );
};

export default inject(({ ssoStore }) => {
  const { onPageLoad, ServiceProviderSettings, SPMetadata } = ssoStore;

  return { onPageLoad, ServiceProviderSettings, SPMetadata };
})(observer(SingleSignOn));
