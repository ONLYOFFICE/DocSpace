import React, { useEffect } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import Heading from "@appserver/components/heading";
import Backdrop from "@appserver/components/backdrop";
import Aside from "@appserver/components/aside";
import { StyledHotkeysPanel, StyledScrollbar } from "./StyledHotkeys";
import SelectionBlock from "./SelectionBlock";
import MoveBlock from "./MoveBlock";
import ActionsBlock from "./ActionsBlock";
import ApplicationActionsBlock from "./ApplicationActionsBlock";
import PreviewActionsBlock from "./PreviewActionsBlock";

const HotkeyPanel = ({ visible, setHotkeyPanelVisible, t, tReady }) => {
  const onClose = () => setHotkeyPanelVisible(false);
  const textStyles = {
    fontSize: "13px",
    fontWeight: 600,
    className: "hotkey-key-description",
  };
  const keyTextStyles = {
    ...textStyles,
    ...{ color: "#657077", className: "hotkeys-key" },
  };

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);

    return () => document.removeEventListener("keyup", onKeyPress);
  });

  return (
    <StyledHotkeysPanel>
      <Backdrop onClick={onClose} visible={visible} isAside={true} />
      <Aside className="hotkeys-panel" visible={visible}>
        <div className="hotkeys_header">
          <Heading className="hotkeys_heading">{t("Hotkeys")}</Heading>
        </div>
        <StyledScrollbar stype="mediumBlack">
          <Heading className="hotkeys_sub-header">
            {t("HotkeysSelection")}
          </Heading>
          <SelectionBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
          />

          <Heading className="hotkeys_sub-header">{t("HotkeysMove")}</Heading>
          <MoveBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
          />

          <Heading className="hotkeys_sub-header">
            {t("HotkeysActions")}
          </Heading>
          <ActionsBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
          />
          <Heading className="hotkeys_sub-header">
            {t("HotkeysApplicationActions")}
          </Heading>
          <ApplicationActionsBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
          />

          <Heading className="hotkeys_sub-header">
            {t("HotkeysActionsInPreview")}
          </Heading>
          <PreviewActionsBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
          />
        </StyledScrollbar>
      </Aside>
    </StyledHotkeysPanel>
  );
};

export default inject(({ dialogsStore }) => {
  const { hotkeyPanelVisible, setHotkeyPanelVisible } = dialogsStore;

  return {
    visible: hotkeyPanelVisible,
    setHotkeyPanelVisible,
  };
})(withTranslation("HotkeysPanel")(observer(HotkeyPanel)));
