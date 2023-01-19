import React from "react";
import Row from "@docspace/components/row";
import Text from "@docspace/components/text";

const ApplicationActionsBlock = ({ t, textStyles, keyTextStyles, CtrlKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysShortcuts")}</Text>
          <Text {...keyTextStyles}>
            {CtrlKey} + / {t("Common:Or")} ?
          </Text>
        </>
      </Row>
    </>
  );
};

export default ApplicationActionsBlock;
