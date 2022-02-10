import React from "react";
import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FieldContainer from "@appserver/components/field-container";
import SimpleTextInput from "./SimpleTextInput";
import Text from "@appserver/components/text";
import { ReactSVG } from "react-svg";
import { observer } from "mobx-react";

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
          icon={<ReactSVG src="images/actions.upload.react.svg" />}
          size="medium"
          tabIndex={2}
        />
        <Text className="or-text">{t("Or")}</Text>
        <Button label={t("ChooseFile")} size="medium" tabIndex={3} />
      </Box>
    </FieldContainer>
  );
};

export default observer(UploadXML);
