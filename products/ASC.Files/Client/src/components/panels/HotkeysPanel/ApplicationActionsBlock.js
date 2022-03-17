import React from "react";
import Row from "@appserver/components/row";
import Text from "@appserver/components/text";

const ApplicationActionsBlock = ({ t, textStyles, keyTextStyles, CtrlKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysShortcuts")}</Text>
          <Text {...keyTextStyles}>
            {CtrlKey} + / {t("HotkeysOr")} ?
          </Text>
        </>
      </Row>
    </>
  );
};

export default ApplicationActionsBlock;
