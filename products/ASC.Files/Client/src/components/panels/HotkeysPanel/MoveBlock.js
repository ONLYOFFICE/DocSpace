import React from "react";
import Row from "@appserver/components/row";
import Text from "@appserver/components/text";

const MoveBlock = ({ t, textStyles, keyTextStyles, CtrlKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveDown")}</Text>
          <Text {...keyTextStyles}>{CtrlKey}+↓</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveUp")}</Text>
          <Text {...keyTextStyles}>{CtrlKey}+↑</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveLeft")}</Text>
          <Text {...keyTextStyles}>{CtrlKey}+←</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysMoveRight")}</Text>
          <Text {...keyTextStyles}>{CtrlKey}+→</Text>
        </>
      </Row>
    </>
  );
};

export default MoveBlock;
