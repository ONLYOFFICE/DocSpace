import React from "react";
import Row from "@appserver/components/row";
import Text from "@appserver/components/text";

const CreationBlock = ({ t, textStyles, keyTextStyles, CtrlKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysUploadFile")}</Text>
          <Text {...keyTextStyles}>Shift + u</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysUploadFolder")}</Text>
          <Text {...keyTextStyles}>Shift + i</Text>
        </>
      </Row>
    </>
  );
};

export default CreationBlock;
