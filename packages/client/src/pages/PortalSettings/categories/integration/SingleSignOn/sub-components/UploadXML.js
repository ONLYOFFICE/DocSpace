import UploadIcon from "PUBLIC_DIR/images/actions.upload.react.svg";

import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Button from "@docspace/components/button";
import FieldContainer from "@docspace/components/field-container";
import Text from "@docspace/components/text";

import SsoTextInput from "./SsoTextInput";
import FileInput from "@docspace/components/file-input";
import { Base } from "@docspace/components/themes";
import { smallTablet } from "@docspace/components/utils/device";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;

  .upload-input {
    width: 340px;
    display: flex;
    flex-direction: row;
    gap: 9px;

    @media ${smallTablet} {
      width: 100%;

      .upload-xml-input {
        max-width: 100%;
      }
    }
  }

  .xml-upload-file {
    width: auto;

    .text-input {
      display: none;
    }

    .icon {
      position: static;
    }

    @media ${smallTablet} {
      width: 100%;

      button {
        width: 100%;
      }
    }
  }

  .upload-button {
    height: 32px;
    width: 45px;
    overflow: inherit;
  }

  @media ${smallTablet} {
    flex-direction: column;
    gap: 8px;
  }
`;

const StyledUploadIcon = styled(UploadIcon)`
  path {
    stroke: ${(props) =>
      props.disabled
        ? props.theme.client.settings.integration.sso.iconButtonDisabled
        : props.theme.client.settings.integration.sso.iconButton} !important;
  }
`;

StyledUploadIcon.defaultProps = { theme: Base };

const UploadXML = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    enableSso,
    uploadXmlUrl,
    isLoadingXml,
    uploadByUrl,
    uploadXml,
  } = props;

  const isDisabledProp = {
    disabled: !enableSso || uploadXmlUrl.trim().length === 0 || isLoadingXml,
  };

  const onUploadClick = () => {
    uploadByUrl(t);
  };

  return (
    <FieldContainer
      className="xml-input"
      errorMessage="Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
      isVertical
      labelText={t("UploadXML")}
    >
      <StyledWrapper>
        <div className="upload-input">
          <SsoTextInput
            className="upload-xml-input"
            maxWidth="297px"
            name="uploadXmlUrl"
            placeholder={t("UploadXMLPlaceholder")}
            tabIndex={1}
            value={uploadXmlUrl}
          />

          <Button
            className="upload-button"
            icon={<StyledUploadIcon {...isDisabledProp} />}
            isDisabled={
              !enableSso || uploadXmlUrl.trim().length === 0 || isLoadingXml
            }
            onClick={onUploadClick}
            tabIndex={2}
          />
        </div>
        <Text className="or-text" noSelect>
          {t("Common:Or")}
        </Text>

        <FileInput
          idButton="select-file"
          accept=".xml"
          buttonLabel={t("Common:SelectFile")}
          className="xml-upload-file"
          isDisabled={!enableSso || isLoadingXml}
          onInput={uploadXml}
          size="middle"
          tabIndex={3}
        />
      </StyledWrapper>
    </FieldContainer>
  );
};

export default inject(({ ssoStore }) => {
  const {
    enableSso,
    uploadXmlUrl,
    isLoadingXml,
    uploadByUrl,
    uploadXml,
  } = ssoStore;

  return {
    enableSso,
    uploadXmlUrl,
    isLoadingXml,
    uploadByUrl,
    uploadXml,
  };
})(observer(UploadXML));
