import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import ToggleButton from "@appserver/components/toggle-button";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import HelpButton from "@appserver/components/help-button";
import Link from "@appserver/components/link";
import FieldContainer from "@appserver/components/field-container";
import TextInput from "@appserver/components/text-input";
import Button from "@appserver/components/button";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import ComboBox from "@appserver/components/combobox";
import Checkbox from "@appserver/components/checkbox";
import Label from "@appserver/components/label";

const SingleSignOn = () => {
  const { t } = useTranslation("SingleSignOn");
  const [isSSOEnabled, setIsSSOEnabled] = useState(true);
  const [showServiceProvider, setShowServiceProvider] = useState(true);
  const [showAdditionalParameters, setShowAdditionalParameters] = useState(
    false
  );
  const [showProviderMetadata, setShowProviderMetadata] = useState(true);

  const onSSOToggle = () => {
    setIsSSOEnabled(!isSSOEnabled);
  };

  const showServiceProviderClick = () => {
    setShowServiceProvider(!showServiceProvider);
  };

  const showAdditionalParametersClick = () => {
    setShowAdditionalParameters(!showAdditionalParameters);
  };

  const showProviderMetadataClick = () => {
    setShowProviderMetadata(!showProviderMetadata);
  };

  return (
    <Box displayProp="flex" flexDirection="column" widthProp="100%">
      <Box
        backgroundProp="#F8F9F9 "
        borderProp={{ "border-radius": "4px" }}
        displayProp="flex"
        flexDirection="row"
        paddingProp="12px"
        widthProp="100%"
      >
        <ToggleButton
          isChecked={isSSOEnabled}
          onChange={onSSOToggle}
          style={{ position: "static" }}
        />

        <Box>
          <Box alignItems="center" displayProp="flex" flexDirection="row">
            <Text fontWeight={600} lineHeight="20px">
              {t("TurnOn")}
            </Text>
            <HelpButton
              offsetRight={0}
              style={{ padding: "0 6px" }}
              tooltipContent={<Text>{t("TurnOnTooltip")}</Text>}
            />
          </Box>
          <Text>{t("TurnOnCaption")}</Text>
        </Box>
      </Box>

      <Box displayProp="flex" flexDirection="row" marginProp="24px 0">
        <Text as="h2" fontWeight={600}>
          {t("ServiceProviderSettings")}
        </Text>
        <Link
          isHovered
          onClick={showServiceProviderClick}
          style={{ margin: "0 16px" }}
          type="action"
        >
          {showServiceProvider ? t("Hide") : t("Show")}
        </Link>
      </Box>

      <Box as="form">
        <Box>
          <FieldContainer
            errorMessage="Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("UploadXML")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent="Paste you tooltip content here"
          >
            <Box displayProp="flex" flexDirection="row">
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder={t("UploadXMLPlaceholder")}
              />
              <Button label="icon" size="medium" />
              <Text>{t("Or")}</Text>
              <Button label={t("ChooseFile")} size="medium" />
            </Box>
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("CustomEntryButton")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("CustomEntryTooltip")}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="Single Sign-on"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("ProviderURL")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("ProviderURLTooltip")}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="https://www.test.com"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("EndpointURL")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("EndpointURLTooltip")}
          >
            <Box displayProp="flex" flexDirection="row">
              <Label text={t("Binding")} htmlFor="binding" />
              <RadioButtonGroup
                name="binding"
                selected="b"
                options={[
                  {
                    value: "a",
                    label: "POST",
                  },
                  {
                    value: "b",
                    label: "Redirect",
                  },
                ]}
              />
            </Box>

            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="https://www.test.com/saml/login"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("NameIDFormat")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <ComboBox
              selectedOption={{
                key: 1,
                label: "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
              }}
              options={[
                {
                  key: 1,
                  label:
                    "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
                },
                {
                  key: 2,
                  label:
                    "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress",
                },
                {
                  key: 3,
                  label: "urn:oasis:names:tc:SAML:2.0:nameid-format:entity",
                },
                {
                  key: 4,
                  label: "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",
                },
                {
                  key: 5,
                  label: "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent",
                },
                {
                  key: 6,
                  label: "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted",
                },
                {
                  key: 7,
                  label:
                    "urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified",
                },
                {
                  key: 8,
                  label:
                    "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName",
                },
                {
                  key: 9,
                  label:
                    "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName",
                },
                {
                  key: 10,
                  label: "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos",
                },
              ]}
            />
          </FieldContainer>
        </Box>

        <Box
          alignItems="center"
          displayProp="flex"
          flexDirection="row"
          marginProp="24px 0"
        >
          <Text as="h2" fontWeight={600}>
            {t("OpenCertificates")}
          </Text>
          <HelpButton
            offsetRight={0}
            style={{ padding: "0 6px" }}
            tooltipContent={<Text>{t("TurnOnTooltip")}</Text>}
          />
        </Box>

        <Box alignItems="center" displayProp="flex" flexDirection="row">
          <Button label="Добавить сертификат" size="medium" />
          <Link
            isHovered
            onClick={showAdditionalParametersClick}
            style={{ margin: "0 16px" }}
            type="action"
          >
            {showAdditionalParameters
              ? t("HideAdditionalParameters")
              : t("ShowAdditionalParameters")}
          </Link>
        </Box>
        {/*Additional parameters, needs to be hidable*/}
        <Box>
          <Checkbox label={t("SignAuthRequest")} />
          <Checkbox label={t("SignExitRequest")} />
          <Checkbox label={t("SignResponseRequest")} />
          <Checkbox label={t("DecryptStatements")} />

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("SigningAlgorithm")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <ComboBox
              selectedOption={{
                key: 1,
                label: "rsa-sha1",
              }}
              options={[
                {
                  key: 1,
                  label: "rsa-sha1",
                },
                {
                  key: 2,
                  label: "rsa-sha256",
                },
                {
                  key: 3,
                  label: "rsa-sha512",
                },
              ]}
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("StandardDecryptionAlgorithm")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <ComboBox
              selectedOption={{
                key: 1,
                label: "aes128-cbc",
              }}
              options={[
                {
                  key: 1,
                  label: "aes128-cbc",
                },
                {
                  key: 2,
                  label: "aes256-cbc",
                },
                {
                  key: 1,
                  label: "tripledes-cbc",
                },
              ]}
            />
          </FieldContainer>
        </Box>

        <Box>
          <Box alignItems="center" displayProp="flex" flexDirection="row">
            <Text as="h2" fontWeight={600}>
              {t("AttributeMatching")}
            </Text>
            <HelpButton
              offsetRight={0}
              style={{ padding: "0 6px" }}
              tooltipContent={<Text>{t("AttributeMatchingTooltip")}</Text>}
            />
          </Box>
        </Box>

        <Box>
          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("FirstName")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="givenName"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("LastName")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="sn"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("Email")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="sn"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("Location")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="sn"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("Title")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="sn"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("Phone")}
            place="top"
            style={{ maxWidth: "350px" }}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="sn"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("AdvancedSettings")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("AdvancedSettingsTooltip")}
          >
            <Checkbox label={t("HideAuthPage")} />
          </FieldContainer>

          <Box alignItems="center" displayProp="flex" flexDirection="row">
            <Button label={t("Save")} primary size="medium" />
            <Button label={t("ResetSettings")} size="medium" />
          </Box>
        </Box>

        <Box displayProp="flex" flexDirection="column">
          <Box displayProp="flex" flexDirection="row" marginProp="24px 0">
            <Text as="h2" fontWeight={600}>
              {t("SPMetadata")}
            </Text>
            <Link
              isHovered
              onClick={showProviderMetadataClick}
              style={{ margin: "0 16px" }}
              type="action"
            >
              {showProviderMetadata ? t("Hide") : t("Show")}
            </Link>
          </Box>
        </Box>

        <Box displayProp="flex" flexDirection="column">
          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("SPEntityId")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("SPEntityIdTooltip")}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="Line input"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("SPAssertionConsumerURL")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("SPAssertionConsumerURLTooltip")}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="Line input"
            />
          </FieldContainer>

          <FieldContainer
            helpButtonHeaderContent="Tooltip header"
            isVertical
            labelText={t("SPSingleLogoutURL")}
            place="top"
            style={{ maxWidth: "350px" }}
            tooltipContent={t("SPSingleLogoutURLTooltip")}
          >
            <TextInput
              className="field-input"
              onChange={() => {}}
              placeholder="Line input"
            />
          </FieldContainer>

          <Button label="DownloadMetadataXML" primary size="medium" />
        </Box>
      </Box>
    </Box>
  );
};

export default SingleSignOn;
