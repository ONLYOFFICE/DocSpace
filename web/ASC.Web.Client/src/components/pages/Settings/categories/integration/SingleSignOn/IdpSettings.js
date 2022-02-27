import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Text from "@appserver/components/text";

import SimpleComboBox from "./sub-components/SimpleComboBox";
import SimpleFormField from "./sub-components/SimpleFormField";
import UploadXML from "./sub-components/UploadXML";
import { bindingOptions, nameIdOptions } from "./sub-components/constants";

const IdpSettings = () => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <Box>
      <UploadXML />

      <SimpleFormField
        labelText={t("CustomEntryButton")}
        name="spLoginLabel"
        placeholder="Single Sign-on"
        tabIndex={4}
        tooltipContent={t("CustomEntryTooltip")}
      />

      <SimpleFormField
        labelText={t("ProviderURL")}
        name="entityId"
        placeholder="https://www.test.com"
        tabIndex={5}
        tooltipContent={t("ProviderURLTooltip")}
      />

      <SimpleFormField
        labelText={t("SignOnEndpointUrl")}
        name="ssoUrl"
        placeholder="https://www.test.com/saml/login"
        tabIndex={7}
        tooltipContent={t("SignOnEndpointUrlTooltip")}
      >
        <Box displayProp="flex" flexDirection="row" marginProp="0 0 4px 0">
          <Text>{t("Binding")}</Text>

          <RadioButtonGroup
            className="radio-button-group"
            name="ssoBinding"
            onClick={FormStore.onBindingChange}
            options={bindingOptions}
            selected={FormStore.ssoBinding}
            spacing="21px"
            tabIndex={6}
          />
        </Box>
      </SimpleFormField>

      <SimpleFormField
        labelText={t("LogoutEndpointUrl")}
        name="sloUrl"
        placeholder="https://www.test.com/saml/logout"
        tabIndex={9}
        tooltipContent={t("LogoutEndpointUrlTooltip")}
      >
        <Box displayProp="flex" flexDirection="row" marginProp="0 0 4px 0">
          <Text>{t("Binding")}</Text>

          <RadioButtonGroup
            className="radio-button-group"
            name="sloBinding"
            onClick={FormStore.onBindingChange}
            options={bindingOptions}
            selected={FormStore.sloBinding}
            spacing="21px"
            tabIndex={8}
          />
        </Box>
      </SimpleFormField>

      <SimpleComboBox
        labelText={t("NameIDFormat")}
        name="nameIdFormat"
        options={nameIdOptions}
        tabIndex={8}
      />
    </Box>
  );
};

export default observer(IdpSettings);
