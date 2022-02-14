import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import RadioButtonGroup from "@appserver/components/radio-button-group";

import HideButton from "./sub-components/HideButton";
import SimpleComboBox from "./sub-components/SimpleComboBox";
import SimpleFormField from "./sub-components/SimpleFormField";
import Text from "@appserver/components/text";
import UploadXML from "./sub-components/UploadXML";
import { bindingOptions, nameIdOptions } from "./sub-components/constants";

const IdpSettings = ({ FormStore, t }) => {
  return (
    <Box>
      <HideButton FormStore={FormStore} label="ServiceProviderSettings" t={t} />

      <UploadXML FormStore={FormStore} t={t} />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("CustomEntryButton")}
        name="spLoginLabel"
        placeholder="Single Sign-on"
        t={t}
        tabIndex={4}
        tooltipContent={t("CustomEntryTooltip")}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("ProviderURL")}
        name="entityId"
        placeholder="https://www.test.com"
        t={t}
        tabIndex={5}
        tooltipContent={t("ProviderURLTooltip")}
      />

      <SimpleFormField
        FormStore={FormStore}
        labelText={t("EndpointURL")}
        name="ssoUrl"
        placeholder="https://www.test.com/saml/login"
        t={t}
        tabIndex={7}
        tooltipContent={t("EndpointURLTooltip")}
      >
        <Box displayProp="flex" flexDirection="row" marginProp="0 0 4px 0">
          <Text>{t("Binding")}</Text>

          <RadioButtonGroup
            className="radio-button-group"
            name="binding"
            onClick={FormStore.onBindingChange}
            options={bindingOptions}
            selected={FormStore.ssoBinding}
            spacing="21px"
            tabIndex={6}
          />
        </Box>
      </SimpleFormField>

      <SimpleComboBox
        FormStore={FormStore}
        labelText={t("NameIDFormat")}
        name="nameIdFormat"
        options={nameIdOptions}
        tabIndex={8}
      />
    </Box>
  );
};

export default observer(IdpSettings);
