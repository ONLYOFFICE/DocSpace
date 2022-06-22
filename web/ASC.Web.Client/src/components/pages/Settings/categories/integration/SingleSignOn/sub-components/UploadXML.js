import React from "react";
import { ReactSVG } from "react-svg";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FieldContainer from "@appserver/components/field-container";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import Text from "@appserver/components/text";

import SimpleTextInput from "./SimpleTextInput";
import FileInput from "@appserver/components/file-input";

const uploadIcon = <ReactSVG src="images/actions.upload.react.svg" />;

const UploadXML = () => {
  const { t } = useTranslation("SingleSignOn");

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
        />

        <Button
          className="upload-button"
          icon={uploadIcon}
          isDisabled={
            !FormStore.enableSso ||
            FormStore.uploadXmlUrl.trim().length === 0 ||
            FormStore.onLoadXML
          }
          onClick={FormStore.onLoadXmlMetadata}
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
          isDisabled={!FormStore.enableSso || FormStore.onLoadXML}
          onInput={FormStore.onUploadXmlMetadata}
          size="middle"
          tabIndex={3}
        />
      </Box>
    </FieldContainer>
  );
};

export default observer(UploadXML);
