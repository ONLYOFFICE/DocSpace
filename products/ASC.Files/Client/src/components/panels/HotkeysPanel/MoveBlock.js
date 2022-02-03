import React from "react";
import Row from "@appserver/components/row";
import Text from "@appserver/components/text";

const MoveBlock = ({ t, textStyles, keyTextStyles }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveDown")}</Text>
          <Text {...keyTextStyles}>Ctrl+↓</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveUp")}</Text>
          <Text {...keyTextStyles}>Ctrl+↑</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveLeft")}</Text>
          <Text {...keyTextStyles}>Ctrl+←</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveRight")}</Text>
          <Text {...keyTextStyles}>Ctrl+→</Text>
        </>
      </Row>
    </>
  );
};

export default MoveBlock;
