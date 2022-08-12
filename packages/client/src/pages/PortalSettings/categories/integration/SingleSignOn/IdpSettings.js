import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import Text from "@docspace/components/text";

import SsoComboBox from "./sub-components/SsoComboBox";
import SsoFormField from "./sub-components/SsoFormField";
import UploadXML from "./sub-components/UploadXML";
import { bindingOptions, nameIdOptions } from "./sub-components/constants";

const IdpSettings = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    ssoBinding,
    enableSso,
    setInput,
    sloBinding,
    nameIdFormat,
    spLoginLabel,
    entityId,
    ssoUrlPost,
    ssoUrlRedirect,
    sloUrlPost,
    sloUrlRedirect,
    entityIdHasError,
    spLoginLabelHasError,
    ssoUrlPostHasError,
    ssoUrlRedirectHasError,
    sloUrlPostHasError,
    sloUrlRedirectHasError,
  } = props;

  return (
    <Box>
      <UploadXML />

      <SsoFormField
        labelText={t("CustomEntryButton")}
        name="spLoginLabel"
        placeholder="Single Sign-on"
        tabIndex={4}
        tooltipContent={t("CustomEntryTooltip")}
        value={spLoginLabel}
        hasError={spLoginLabelHasError}
      />

      <SsoFormField
        labelText={t("ProviderURL")}
        name="entityId"
        placeholder="https://idpservice/idp"
        tabIndex={5}
        tooltipContent={t("ProviderURLTooltip")}
        value={entityId}
        hasError={entityIdHasError}
      />

      <SsoFormField
        labelText={t("SignOnEndpointUrl")}
        name={ssoBinding.includes("POST") ? "ssoUrlPost" : "ssoUrlRedirect"}
        placeholder={
          ssoBinding.includes("POST")
            ? "https://idpservice/SSO/POST"
            : "https://idpservice/SSO/REDIRECT"
        }
        tabIndex={7}
        tooltipContent={t("SignOnEndpointUrlTooltip")}
        value={ssoBinding.includes("POST") ? ssoUrlPost : ssoUrlRedirect}
        hasError={
          ssoBinding.includes("POST")
            ? ssoUrlPostHasError
            : ssoUrlRedirectHasError
        }
      >
        <Box
          displayProp="flex"
          alignItems="center"
          flexDirection="row"
          marginProp="5px 0"
        >
          <Text fontSize="12px" fontWeight={400} noSelect>
            {t("Binding")}
          </Text>

          <RadioButtonGroup
            className="radio-button-group"
            isDisabled={!enableSso}
            name="ssoBinding"
            onClick={setInput}
            options={bindingOptions}
            selected={ssoBinding}
            spacing="20px"
            tabIndex={6}
          />
        </Box>
      </SsoFormField>

      <SsoFormField
        labelText={t("LogoutEndpointUrl")}
        name={sloBinding.includes("POST") ? "sloUrlPost" : "sloUrlRedirect"}
        placeholder={
          sloBinding.includes("POST")
            ? "https://idpservice/SLO/POST"
            : "https://idpservice/SLO/REDIRECT"
        }
        tabIndex={9}
        tooltipContent={t("LogoutEndpointUrlTooltip")}
        value={sloBinding.includes("POST") ? sloUrlPost : sloUrlRedirect}
        hasError={
          ssoBinding.includes("POST")
            ? sloUrlPostHasError
            : sloUrlRedirectHasError
        }
      >
        <Box
          displayProp="flex"
          alignItems="center"
          flexDirection="row"
          marginProp="5px 0"
        >
          <Text fontSize="12px" fontWeight={400}>
            {t("Binding")}
          </Text>

          <RadioButtonGroup
            className="radio-button-group"
            isDisabled={!enableSso}
            name="sloBinding"
            onClick={setInput}
            options={bindingOptions}
            selected={sloBinding}
            spacing="20px"
            tabIndex={8}
          />
        </Box>
      </SsoFormField>

      <SsoComboBox
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
    setInput,
    sloBinding,
    nameIdFormat,
    spLoginLabel,
    entityId,
    ssoUrlPost,
    ssoUrlRedirect,
    sloUrlPost,
    sloUrlRedirect,
    entityIdHasError,
    spLoginLabelHasError,
    ssoUrlPostHasError,
    ssoUrlRedirectHasError,
    sloUrlPostHasError,
    sloUrlRedirectHasError,
  } = ssoStore;

  return {
    ssoBinding,
    enableSso,
    setInput,
    sloBinding,
    nameIdFormat,
    spLoginLabel,
    entityId,
    ssoUrlPost,
    ssoUrlRedirect,
    sloUrlPost,
    sloUrlRedirect,
    entityIdHasError,
    spLoginLabelHasError,
    ssoUrlPostHasError,
    ssoUrlRedirectHasError,
    sloUrlPostHasError,
    sloUrlRedirectHasError,
  };
})(observer(IdpSettings));
