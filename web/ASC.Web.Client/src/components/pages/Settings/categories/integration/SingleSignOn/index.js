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
import { ReactSVG } from "react-svg";
import styled, { css } from "styled-components";

const StyledContainer = styled.div`
  box-sizing: border-box;
  outline: none;
  width: 100%;

  .toggle {
    position: static;
  }

  .tooltip-button,
  .icon-button {
    padding: 0 5px;
  }

  .hide-button {
    margin-left: 16px;
  }

  .hide-additional-button {
    margin-left: 8px;
  }

  .field-label-icon {
    align-items: center;
    margin-bottom: 4px;
    max-width: 350px;
  }

  .field-label {
    height: auto;
    font-weight: 600;
    line-height: 20px;
    overflow: visible;
    white-space: normal;
  }

  .xml-input {
    .field-label-icon {
      margin-bottom: 8px;
      max-width: 350px;
    }

    .field-label {
      font-weight: 400;
    }
  }

  .or-text {
    margin: 0 24px;
  }

  .radio-button-group {
    margin-left: 25px;
  }

  .combo-button-label {
    max-width: 100%;
  }

  .checkbox-input {
    margin: 6px 8px 6px 0;
  }

  .upload-button {
    margin-left: 9px;
    overflow: inherit;

    & > div {
      margin-top: 2px;
    }
  }

  .save-button {
    margin-right: 8px;
  }

  .download-button {
    max-width: 404px;
    width: 100%;
  }
`;

const StyledInputWrapper = styled.div`
  width: 100%;
  max-width: ${(props) => props.maxWidth || "350px"};
`;

const InputLabel = ({ text, tooltipContent }) => (
  <Box
    alignItems="center"
    className="field-label-icon"
    displayProp="flex"
    flexDirection="row"
  >
    <span className="field-label">
      {text}
      {tooltipContent && (
        <HelpButton
          className="tooltip-button"
          offsetRight={0}
          tooltipContent={<Text>{tooltipContent}</Text>}
          style={{ display: "inline-flex" }}
        />
      )}
    </span>
  </Box>
);

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
    <StyledContainer>
      <Box
        backgroundProp="#F8F9F9 "
        borderProp={{ radius: "4px" }}
        displayProp="flex"
        flexDirection="row"
        paddingProp="12px"
      >
        <ToggleButton
          className="toggle"
          isChecked={isSSOEnabled}
          onChange={onSSOToggle}
        />

        <Box>
          <Box alignItems="center" displayProp="flex" flexDirection="row">
            <Text as="span" fontWeight={600} lineHeight="20px">
              {t("TurnOnSSO")}
              <HelpButton
                offsetRight={0}
                style={{ display: "inline-flex" }}
                tooltipContent={<Text>{t("TurnOnSSOTooltip")}</Text>}
              />
            </Text>
          </Box>

          <Text lineHeight="16px">{t("TurnOnSSOCaption")}</Text>
        </Box>
      </Box>

      <Box as="form">
        <Box>
          <Box
            alignItems="center"
            displayProp="flex"
            flexDirection="row"
            marginProp="24px 0"
          >
            <Text as="h2" fontSize="16px" fontWeight={600}>
              {t("ServiceProviderSettings")}
            </Text>

            <Link
              className="hide-button"
              isHovered
              onClick={showServiceProviderClick}
              type="action"
            >
              {showServiceProvider ? t("Hide") : t("Show")}
            </Link>
          </Box>

          <FieldContainer
            className="xml-input"
            errorMessage="Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
            isVertical
            labelText={t("UploadXML")}
          >
            <Box alignItems="center" displayProp="flex" flexDirection="row">
              <StyledInputWrapper maxWidth="300px">
                <TextInput
                  className="field-input"
                  onChange={() => {}}
                  placeholder={t("UploadXMLPlaceholder")}
                  scale
                  tabIndex={1}
                />
              </StyledInputWrapper>

              <Button
                className="upload-button"
                size="medium"
                icon={<ReactSVG src="images/actions.upload.react.svg" />}
                tabIndex={2}
              />
              <Text className="or-text">{t("Or")}</Text>
              <Button label={t("ChooseFile")} size="medium" tabIndex={3} />
            </Box>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("CustomEntryButton")}
                tooltipContent={t("CustomEntryTooltip")}
              />
            }
          >
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="Single Sign-on"
                scale
                tabIndex={4}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("ProviderURL")}
                tooltipContent={t("ProviderURLTooltip")}
              />
            }
          >
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="https://www.test.com"
                scale
                tabIndex={5}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("EndpointURL")}
                tooltipContent={t("EndpointURLTooltip")}
              />
            }
          >
            <Box displayProp="flex" flexDirection="row" marginProp="0 0 4px 0">
              <Text>{t("Binding")}</Text>

              <RadioButtonGroup
                className="radio-button-group"
                name="binding"
                selected="b"
                spacing="21px"
                tabIndex={6}
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

            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="https://www.test.com/saml/login"
                scale
                tabIndex={7}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer isVertical labelText={t("NameIDFormat")}>
            <StyledInputWrapper>
              <ComboBox
                scaled
                scaledOptions
                showDisabledItems
                tabIndex={8}
                selectedOption={{
                  key: 1,
                  label:
                    "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
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
                    label:
                      "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",
                  },
                  {
                    key: 5,
                    label:
                      "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent",
                  },
                  {
                    key: 6,
                    label:
                      "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted",
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
            </StyledInputWrapper>
          </FieldContainer>
        </Box>

        <Box>
          <Box
            alignItems="center"
            displayProp="flex"
            flexDirection="row"
            marginProp="24px 0"
          >
            <Text as="h2" fontSize="14px" fontWeight={600}>
              {t("OpenCertificates")}
            </Text>

            <HelpButton
              offsetRight={0}
              tooltipContent={<Text>{t("TurnOnTooltip")}</Text>}
            />
          </Box>

          <Box alignItems="center" displayProp="flex" flexDirection="row">
            <Button label="Добавить сертификат" size="medium" tabIndex={9} />

            <Link
              className="hide-additional-button"
              isHovered
              onClick={showAdditionalParametersClick}
              type="action"
            >
              {showAdditionalParameters
                ? t("HideAdditionalParameters")
                : t("ShowAdditionalParameters")}
            </Link>
          </Box>

          <Box marginProp="12px 0">
            <Checkbox
              className="checkbox-input"
              label={t("SignAuthRequest")}
              tabIndex={10}
            />
            <Checkbox
              className="checkbox-input"
              label={t("SignExitRequest")}
              tabIndex={11}
            />
            <Checkbox
              className="checkbox-input"
              label={t("SignResponseRequest")}
              tabIndex={12}
            />
            <Checkbox
              className="checkbox-input"
              label={t("DecryptStatements")}
              tabIndex={13}
            />
          </Box>

          <FieldContainer isVertical labelText={t("SigningAlgorithm")}>
            <StyledInputWrapper>
              <ComboBox
                scaled
                scaledOptions
                showDisabledItems
                tabIndex={14}
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
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={t("StandardDecryptionAlgorithm")}
          >
            <StyledInputWrapper>
              <ComboBox
                scaled
                scaledOptions
                showDisabledItems
                tabIndex={15}
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
                    key: 3,
                    label: "tripledes-cbc",
                  },
                ]}
              />
            </StyledInputWrapper>
          </FieldContainer>
        </Box>

        <Box>
          <Box>
            <Box alignItems="center" displayProp="flex" flexDirection="row">
              <Text as="h2" fontWeight={600}>
                {t("AttributeMatching")}
              </Text>

              <HelpButton
                offsetRight={0}
                tooltipContent={<Text>{t("AttributeMatchingTooltip")}</Text>}
              />
            </Box>
          </Box>

          <FieldContainer isVertical labelText={t("FirstName")}>
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="givenName"
                scale
                tabIndex={16}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer isVertical labelText={t("LastName")}>
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="sn"
                scale
                tabIndex={17}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer isVertical labelText={t("Email")}>
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="sn"
                scale
                tabIndex={18}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer isVertical labelText={t("Location")}>
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="sn"
                scale
                tabIndex={19}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer isVertical labelText={t("Title")}>
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="sn"
                scale
                tabIndex={20}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer isVertical labelText={t("Phone")}>
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="sn"
                scale
                tabIndex={21}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("AdvancedSettings")}
                tooltipContent={t("AdvancedSettingsTooltip")}
              />
            }
          >
            <Checkbox label={t("HideAuthPage")} tabIndex={22} />
          </FieldContainer>

          <Box alignItems="center" displayProp="flex" flexDirection="row">
            <Button
              className="save-button"
              label={t("Save")}
              primary
              size="medium"
              tabIndex={23}
            />
            <Button label={t("ResetSettings")} size="medium" tabIndex={24} />
          </Box>
        </Box>

        <Box>
          <Box
            alignItems="center"
            displayProp="flex"
            flexDirection="row"
            marginProp="24px 0"
          >
            <Text as="h2" fontSize="14x" fontWeight={600}>
              {t("SPMetadata")}
            </Text>

            <Link
              className="hide-button"
              isHovered
              onClick={showProviderMetadataClick}
              type="action"
            >
              {showProviderMetadata ? t("Hide") : t("Show")}
            </Link>
          </Box>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("SPEntityId")}
                tooltipContent={t("SPEntityIdTooltip")}
              />
            }
          >
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="Line input"
                scale
                tabIndex={25}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("SPAssertionConsumerURL")}
                tooltipContent={t("SPAssertionConsumerURLTooltip")}
              />
            }
          >
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="Line input"
                scale
                tabIndex={26}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <FieldContainer
            isVertical
            labelText={
              <InputLabel
                text={t("SPSingleLogoutURL")}
                tooltipContent={t("SPSingleLogoutURLTooltip")}
              />
            }
          >
            <StyledInputWrapper>
              <TextInput
                className="field-input"
                onChange={() => {}}
                placeholder="Line input"
                scale
                tabIndex={27}
              />
            </StyledInputWrapper>
          </FieldContainer>

          <Box>
            <Button
              className="download-button"
              label={t("DownloadMetadataXML")}
              primary
              size="medium"
              tabIndex={28}
            />
          </Box>
        </Box>
      </Box>
    </StyledContainer>
  );
};

export default SingleSignOn;
