import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FormStore from "@appserver/studio/src/store/SsoFormStore";

import FieldMapping from "./FieldMapping";
import Certificates from "./Certificates";
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

      <Certificates FormStore={FormStore} t={t} provider="IdentityProvider" />

      <Certificates FormStore={FormStore} t={t} provider="ServiceProvider" />

      <FieldMapping FormStore={FormStore} t={t} />

      <Box alignItems="center" displayProp="flex" flexDirection="row">
        <Button
          className="save-button"
          label={t("Common:SaveButton")}
          onClick={FormStore.onSubmit}
          primary
          size="medium"
          tabIndex={23}
        />
        <Button
          label={t("ResetSettings")}
          onClick={FormStore.resetForm}
          size="medium"
          tabIndex={24}
        />
      </Box>

      <ProviderMetadata FormStore={FormStore} t={t} />
    </StyledSsoPage>
  );
};

export default observer(SingleSignOn);
