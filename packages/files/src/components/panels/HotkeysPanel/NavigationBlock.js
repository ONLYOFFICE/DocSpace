import React from "react";
import Row from "@docspace/components/row";
import Text from "@docspace/components/text";

const NavigationBlock = ({ t, textStyles, keyTextStyles, AltKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysGoToParentFolder")}</Text>
          <Text {...keyTextStyles}>Backspace</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysChangeView")}</Text>
          <Text {...keyTextStyles}>v</Text>
        </>
      </Row>
    </>
  );
};

export default NavigationBlock;
