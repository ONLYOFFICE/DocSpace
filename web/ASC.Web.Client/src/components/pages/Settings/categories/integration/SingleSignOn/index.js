import React, { useEffect } from "react";
import { isDesktop } from "react-device-detect";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import FormStore from "@appserver/studio/src/store/SsoFormStore";

import Certificates from "./Certificates";
import FieldMapping from "./FieldMapping";
import ForbiddenPage from "./sub-components/ForbiddenPage";
import HideButton from "./sub-components/HideButton";
import IdpSettings from "./IdpSettings";
import ProviderMetadata from "./ProviderMetadata";
import StyledSsoPage from "./styled-containers/StyledSsoPageContainer";
import SubmitResetButtons from "./SubmitButton";
import ToggleSSO from "./sub-components/ToggleSSO";

const SingleSignOn = () => {
  if (!isDesktop) return <ForbiddenPage />;

  useEffect(() => {
    FormStore.onPageLoad();
  }, []);

  return (
    <StyledSsoPage
      hideSettings={FormStore.ServiceProviderSettings}
      hideMetadata={FormStore.SPMetadata}
    >
      <ToggleSSO />

      <HideButton label="ServiceProviderSettings" />

      <Box className="service-provider-settings">
        <IdpSettings />

        <Certificates provider="IdentityProvider" />

        <Certificates provider="ServiceProvider" />

        <FieldMapping />

        <SubmitResetButtons />
      </Box>

      <hr className="separator" />

      <HideButton label="SPMetadata" />

      <Box className="sp-metadata">
        <ProviderMetadata />
      </Box>
    </StyledSsoPage>
  );
};

export default observer(SingleSignOn);
