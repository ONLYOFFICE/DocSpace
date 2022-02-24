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
      <ToggleSSO FormStore={FormStore} t={t} />

      <HideButton FormStore={FormStore} label="ServiceProviderSettings" t={t} />

      <Box className="service-provider-settings">
        <IdpSettings FormStore={FormStore} t={t} />

        <Certificates FormStore={FormStore} t={t} provider="IdentityProvider" />

        <Certificates FormStore={FormStore} t={t} provider="ServiceProvider" />

        <FieldMapping FormStore={FormStore} t={t} />

        <SubmitResetButtons FormStore={FormStore} t={t} />
      </Box>

      <hr className="separator" />

      <HideButton FormStore={FormStore} label="SPMetadata" t={t} />

      <Box className="sp-metadata">
        <ProviderMetadata FormStore={FormStore} t={t} />
      </Box>
    </StyledSsoPage>
  );
};

export default observer(SingleSignOn);
