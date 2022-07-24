import React from "react";
import Row from "@docspace/components/row";
import Text from "@docspace/components/text";

const ActionsBlock = ({ t, textStyles, keyTextStyles, CtrlKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysOpen")}</Text>
          <Text {...keyTextStyles}>{t("HotkeysEnterKey")}</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysRemove")}</Text>
          <Text {...keyTextStyles}># {t("HotkeysOr")} Delete</Text>
        </>
      </Row>
      {/* <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysUndoLastAction")}</Text>
          <Text {...keyTextStyles}>{CtrlKey}+z</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysRedoLastUndoneAction")}</Text>
          <Text {...keyTextStyles}>{CtrlKey}+Shift+z</Text>
        </>
      </Row> */}
    </>
  );
};

export default ActionsBlock;
