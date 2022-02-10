import React from "react";
import Box from "@appserver/components/box";
import FieldMapping from "./FieldMapping";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import IdpCertificates from "./IdpCertificates";
import IdpSettings from "./IdpSettings";
import ProviderMetadata from "./ProviderMetadata";
import StyledSsoPage from "./styled-containers/StyledSsoPageContainer";
import ToggleSSO from "./sub-components/ToggleSSO";
import styled from "styled-components";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

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
  const { t } = useTranslation(["SingleSignOn", "Common"]);

  return (
    <StyledSsoPage>
      <ToggleSSO FormStore={FormStore} t={t} />

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
        <IdpSettings FormStore={FormStore} t={t} />

        <IdpCertificates FormStore={FormStore} t={t} />

        <FieldMapping FormStore={FormStore} t={t} />

        <ProviderMetadata FormStore={FormStore} t={t} />
      </Box>
    </StyledSsoPage>
  );
};

export default observer(SingleSignOn);
