import React from "react";
import Row from "@docspace/components/row";
import Text from "@docspace/components/text";

const SelectionBlock = ({ t, textStyles, keyTextStyles, CtrlKey }) => {
  return (
    <>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysSelectItem")}</Text>
          <Text {...keyTextStyles}>x</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysSelectDown")}</Text>
          <Text {...keyTextStyles}>j {t("HotkeysOr")} ↓</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysSelectUp")}</Text>
          <Text {...keyTextStyles}>k {t("HotkeysOr")} ↑</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysSelectLeft")}</Text>
          <Text {...keyTextStyles}>h {t("HotkeysOr")} ←</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysSelectRight")}</Text>
          <Text {...keyTextStyles}>l {t("HotkeysOr")} →</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysExtendSelectionDown")}</Text>
          <Text {...keyTextStyles}>Shift + ↓</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysExtendSelectionUp")}</Text>
          <Text {...keyTextStyles}>Shift + ↑</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysExtendSelectionLeft")}</Text>
          <Text {...keyTextStyles}>Shift + ←</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysExtendSelectionRight")}</Text>
          <Text {...keyTextStyles}>Shift + →</Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysSelectAll")}</Text>
          <Text {...keyTextStyles}>
            {CtrlKey} + a {t("HotkeysOr")} Shift + а
          </Text>
        </>
      </Row>
      <Row className="hotkeys_row">
        <>
          <Text {...textStyles}>{t("HotkeysClearAll")}</Text>
          <Text {...keyTextStyles}>Shift + n {t("HotkeysOr")} Esc</Text>
        </>
      </Row>
    </>
  );
};

export default SelectionBlock;
