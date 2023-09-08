import React from "react";
import Row from "@docspace/components/row";
import Text from "@docspace/components/text";

const PreviewActionsBlock = ({ t, textStyles, keyTextStyles }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysClose")}</Text>
          <Text {...keyTextStyles}>Esc</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysPlayPause")}</Text>
          <Text {...keyTextStyles}>{t("HotkeysSpaceKey")}</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysZoomIn")}</Text>
          <Text {...keyTextStyles}>+ {t("Common:Or")} =</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysZoomOut")}</Text>
          <Text {...keyTextStyles}>-</Text>
        </>
      </Row>
    </>
  );
};

export default PreviewActionsBlock;
