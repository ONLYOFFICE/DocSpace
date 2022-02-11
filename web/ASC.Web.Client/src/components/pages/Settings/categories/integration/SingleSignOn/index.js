import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import FormStore from "@appserver/studio/src/store/SsoFormStore";

import FieldMapping from "./FieldMapping";
import IdpCertificates from "./IdpCertificates";
import IdpSettings from "./IdpSettings";
import ProviderMetadata from "./ProviderMetadata";
import StyledSsoPage from "./styled-containers/StyledSsoPageContainer";
import ToggleSSO from "./sub-components/ToggleSSO";

const SingleSignOn = () => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);

  return (
    <StyledSsoPage>
      <ToggleSSO FormStore={FormStore} t={t} />

      <IdpSettings FormStore={FormStore} t={t} />

      <IdpCertificates FormStore={FormStore} t={t} />

      <FieldMapping FormStore={FormStore} t={t} />

      <ProviderMetadata FormStore={FormStore} t={t} />
    </StyledSsoPage>
  );
};

export default observer(SingleSignOn);
