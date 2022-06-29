import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Text from "@appserver/components/text";

import SimpleComboBox from "./sub-components/SimpleComboBox";
import SimpleFormField from "./sub-components/SimpleFormField";
import UploadXML from "./sub-components/UploadXML";
import { bindingOptions, nameIdOptions } from "./sub-components/constants";

const IdpSettings = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    ssoBinding,
    enableSso,
    onBindingChange,
    sloBinding,
    nameIdFormat,
    spLoginLabel,
    entityId,
    ssoUrlPost,
    ssoUrlRedirect,
    sloUrlPost,
    sloUrlRedirect,
    spLoginLabelErrorMessage,
    entityIdErrorMessage,
    ssoUrlPostErrorMessage,
    ssoUrlRedirectErrorMessage,
    sloUrlPostErrorMessage,
    sloUrlRedirectErrorMessage,
  } = props;

  return (
    <Box>
      <UploadXML />

      <SimpleFormField
        labelText={t("CustomEntryButton")}
        name="spLoginLabel"
        placeholder="Single Sign-on"
        tabIndex={4}
        tooltipContent={t("CustomEntryTooltip")}
        value={spLoginLabel}
        errorMessage={spLoginLabelErrorMessage}
      />

      <SimpleFormField
        labelText={t("ProviderURL")}
        name="entityId"
        placeholder="https://www.test.com"
        tabIndex={5}
        tooltipContent={t("ProviderURLTooltip")}
        value={entityId}
        errorMessage={entityIdErrorMessage}
      />

      <SimpleFormField
        labelText={t("SignOnEndpointUrl")}
        name={ssoBinding.includes("POST") ? "ssoUrlPost" : "ssoUrlRedirect"}
        placeholder="https://www.test.com/saml/login"
        tabIndex={7}
        tooltipContent={t("SignOnEndpointUrlTooltip")}
        value={ssoBinding.includes("POST") ? ssoUrlPost : ssoUrlRedirect}
        errorMessage={
          ssoBinding.includes("POST")
            ? ssoUrlPostErrorMessage
            : ssoUrlRedirectErrorMessage
        }
      >
        <Box displayProp="flex" flexDirection="row" marginProp="0 0 4px 0">
          <Text noSelect>{t("Binding")}</Text>

          <RadioButtonGroup
            className="radio-button-group"
            isDisabled={!enableSso}
            name="ssoBinding"
            onClick={onBindingChange}
            options={bindingOptions}
            selected={ssoBinding}
            spacing="21px"
            tabIndex={6}
          />
        </Box>
      </SimpleFormField>

      <SimpleFormField
        labelText={t("LogoutEndpointUrl")}
        name={sloBinding.includes("POST") ? "sloUrlPost" : "sloUrlRedirect"}
        placeholder="https://www.test.com/saml/logout"
        tabIndex={9}
        tooltipContent={t("LogoutEndpointUrlTooltip")}
        value={sloBinding.includes("POST") ? sloUrlPost : sloUrlRedirect}
        errorMessage={
          sloBinding.includes("POST")
            ? sloUrlPostErrorMessage
            : sloUrlRedirectErrorMessage
        }
      >
        <Box displayProp="flex" flexDirection="row" marginProp="0 0 4px 0">
          <Text>{t("Binding")}</Text>

          <RadioButtonGroup
            className="radio-button-group"
            isDisabled={!enableSso}
            name="sloBinding"
            onClick={onBindingChange}
            options={bindingOptions}
            selected={sloBinding}
            spacing="21px"
            tabIndex={8}
          />
        </Box>
      </SimpleFormField>

      <SimpleComboBox
        labelText={t("NameIDFormat")}
        value={nameIdFormat}
        name="nameIdFormat"
        options={nameIdOptions}
        tabIndex={8}
      />
    </Box>
  );
};

export default inject(({ ssoStore }) => {
  const {
    ssoBinding,
    enableSso,
    onBindingChange,
    sloBinding,
    nameIdFormat,
    spLoginLabel,
    entityId,
    ssoUrlPost,
    ssoUrlRedirect,
    sloUrlPost,
    sloUrlRedirect,
    spLoginLabelErrorMessage,
    entityIdErrorMessage,
    ssoUrlPostErrorMessage,
    ssoUrlRedirectErrorMessage,
    sloUrlPostErrorMessage,
    sloUrlRedirectErrorMessage,
  } = ssoStore;

  return {
    ssoBinding,
    enableSso,
    onBindingChange,
    sloBinding,
    nameIdFormat,
    spLoginLabel,
    entityId,
    ssoUrlPost,
    ssoUrlRedirect,
    sloUrlPost,
    sloUrlRedirect,
    spLoginLabelErrorMessage,
    entityIdErrorMessage,
    ssoUrlPostErrorMessage,
    ssoUrlRedirectErrorMessage,
    sloUrlPostErrorMessage,
    sloUrlRedirectErrorMessage,
  };
})(observer(IdpSettings));
