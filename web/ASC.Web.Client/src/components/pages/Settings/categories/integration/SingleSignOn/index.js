import React, { useEffect } from "react";
import { isDesktop } from "react-device-detect";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation(["SingleSignOn", "Common"]);

  if (!isDesktop) return <ForbiddenPage t={t} />;

  useEffect(() => {
    FormStore.onPageLoad();
  }, []);

  return (
    <StyledSsoPage
      hideSettings={FormStore.ServiceProviderSettings}
      hideMetadata={FormStore.SPMetadata}
    >
      <ToggleSSO t={t} />

      <HideButton label="ServiceProviderSettings" t={t} />

      <Box className="service-provider-settings">
        <IdpSettings t={t} />

        <Certificates t={t} provider="IdentityProvider" />

        <Certificates t={t} provider="ServiceProvider" />

        <FieldMapping t={t} />

        <SubmitResetButtons t={t} />
      </Box>

      <hr className="separator" />

      <HideButton label="SPMetadata" t={t} />

      <Box className="sp-metadata">
        <ProviderMetadata t={t} />
      </Box>
    </StyledSsoPage>
  );
};

export default observer(SingleSignOn);
