import React from "react";
import { ReactSVG } from "react-svg";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FieldContainer from "@appserver/components/field-container";
import Text from "@appserver/components/text";

import SimpleTextInput from "./SimpleTextInput";
import FileInput from "@appserver/components/file-input";

const uploadIcon = <ReactSVG src="images/actions.upload.react.svg" />;

const UploadXML = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    enableSso,
    uploadXmlUrl,
    onLoadXML,
    onLoadXmlMetadata,
    onUploadXmlMetadata,
  } = props;

  return (
    <FieldContainer
      className="xml-input"
      errorMessage="Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
      isVertical
      labelText={t("UploadXML")}
    >
      <Box alignItems="center" displayProp="flex" flexDirection="row">
        <SimpleTextInput
          maxWidth="300px"
          name="uploadXmlUrl"
          placeholder={t("UploadXMLPlaceholder")}
          tabIndex={1}
          value={uploadXmlUrl}
        />

        <Button
          className="upload-button"
          icon={uploadIcon}
          isDisabled={
            !enableSso || uploadXmlUrl.trim().length === 0 || onLoadXML
          }
          onClick={onLoadXmlMetadata}
          size="small"
          tabIndex={2}
        />

        <Text className="or-text" noSelect>
          {t("Or")}
        </Text>

        <FileInput
          accept=".xml"
          buttonLabel={t("ChooseFile")}
          className="xml-upload-file"
          isDisabled={!enableSso || onLoadXML}
          onInput={onUploadXmlMetadata}
          size="middle"
          tabIndex={3}
        />
      </Box>
    </FieldContainer>
  );
};

export default inject(({ ssoStore }) => {
  const {
    enableSso,
    uploadXmlUrl,
    onLoadXML,
    onLoadXmlMetadata,
    onUploadXmlMetadata,
  } = ssoStore;

  return {
    enableSso,
    uploadXmlUrl,
    onLoadXML,
    onLoadXmlMetadata,
    onUploadXmlMetadata,
  };
})(observer(UploadXML));
