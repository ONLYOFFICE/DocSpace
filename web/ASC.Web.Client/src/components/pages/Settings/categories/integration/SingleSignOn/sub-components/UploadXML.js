import React from "react";
import { ReactSVG } from "react-svg";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FieldContainer from "@appserver/components/field-container";
import Text from "@appserver/components/text";

import SimpleTextInput from "./SimpleTextInput";
import { FileInput } from "@appserver/components";

const uploadIcon = <ReactSVG src="images/actions.upload.react.svg" />;

const UploadXML = ({ FormStore, t }) => {
  return (
    <FieldContainer
      className="xml-input"
      errorMessage="Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
      isVertical
      labelText={t("UploadXML")}
    >
      <Box alignItems="center" displayProp="flex" flexDirection="row">
        <SimpleTextInput
          FormStore={FormStore}
          maxWidth="300px"
          name="uploadXmlUrl"
          placeholder={t("UploadXMLPlaceholder")}
          tabIndex={1}
        />

        <Button
          className="upload-button"
          icon={uploadIcon}
          size="medium"
          onClick={FormStore.onLoadXmlMetadata}
          tabIndex={2}
        />

        <Text className="or-text">{t("Or")}</Text>

        <FileInput
          accept=".xml"
          buttonLabel={t("ChooseFile")}
          onInput={FormStore.onUploadXmlMetadata}
          className="xml-upload-file"
          size="middle"
          tabIndex={3}
        />
      </Box>
    </FieldContainer>
  );
};

export default observer(UploadXML);
