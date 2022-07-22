import React from "react";
import Row from "@docspace/components/row";
import Text from "@docspace/components/text";

const CreationBlock = ({ t, textStyles, keyTextStyles, AltKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysCreateDocument")}</Text>
          <Text {...keyTextStyles}>Shift + d</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysCreateSpreadsheet")}</Text>
          <Text {...keyTextStyles}>Shift + s</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysCreatePresentation")}</Text>
          <Text {...keyTextStyles}>Shift + p</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysCreateForm")}</Text>
          <Text {...keyTextStyles}>Shift + o</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysCreateFormFromFile")}</Text>
          <Text {...keyTextStyles}>{AltKey} + Shift + o</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysCreateFolder")}</Text>
          <Text {...keyTextStyles}>Shift + f</Text>
        </>
      </Row>
    </>
  );
};

export default CreationBlock;
