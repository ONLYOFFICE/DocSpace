import React from "react";
import Row from "@appserver/components/row";
import Text from "@appserver/components/text";

const ActionsBlock = ({ t, textStyles, keyTextStyles }) => {
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
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysUndoLastAction")}</Text>
          <Text {...keyTextStyles}>Ctrl+z</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysRedoLastUndoneAction")}</Text>
          <Text {...keyTextStyles}>Ctrl+Shift+z</Text>
        </>
      </Row>
    </>
  );
};

export default ActionsBlock;
