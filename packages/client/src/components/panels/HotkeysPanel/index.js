import React, { useEffect, useRef } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import Heading from "@docspace/components/heading";
import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import { StyledHotkeysPanel, StyledScrollbar } from "./StyledHotkeys";
import SelectionBlock from "./SelectionBlock";
import MoveBlock from "./MoveBlock";
import ActionsBlock from "./ActionsBlock";
import ApplicationActionsBlock from "./ApplicationActionsBlock";
import PreviewActionsBlock from "./PreviewActionsBlock";
import NavigationBlock from "./NavigationBlock";
import CreationBlock from "./CreationBlock";
import UploadBlock from "./UploadBlock";
import { isMacOs } from "react-device-detect";
import Base from "@docspace/components/themes/base";

const HotkeyPanel = ({
  visible,
  setHotkeyPanelVisible,
  t,
  theme,
  tReady,
  isVisitor,
}) => {
  const scrollRef = useRef(null);

  const onClose = () => setHotkeyPanelVisible(false);
  const textStyles = {
    fontSize: "13px",
    fontWeight: 600,
    className: "hotkey-key-description",
    noSelect: true,
  };
  const keyTextStyles = {
    ...textStyles,
    ...{ color: theme.hotkeys.key.color, className: "hotkeys-key" },
  };

  const CtrlKey = isMacOs ? "⌘" : "Ctrl";
  const AltKey = isMacOs ? "⌥" : "Alt";

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    scrollRef.current && scrollRef.current.view.focus();

    document.addEventListener("keyup", onKeyPress);

    return () => document.removeEventListener("keyup", onKeyPress);
  });

  return (
    <StyledHotkeysPanel>
      <Backdrop
        onClick={onClose}
        visible={visible}
        isAside={true}
        zIndex={210}
      />
      <Aside className="hotkeys-panel" visible={visible} onClose={onClose}>
        <div className="hotkeys_header">
          <Heading className="hotkeys_heading">{t("Common:Hotkeys")}</Heading>
        </div>
        <StyledScrollbar ref={scrollRef} stype="mediumBlack">
          <Heading className="hotkeys_sub-header">
            {t("HotkeysNavigation")}
          </Heading>
          <NavigationBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
            AltKey={AltKey}
          />
          {!isVisitor && (
            <>
              <Heading className="hotkeys_sub-header">
                {t("HotkeysCreatingObjects")}
              </Heading>
              <CreationBlock
                t={t}
                textStyles={textStyles}
                keyTextStyles={keyTextStyles}
                AltKey={AltKey}
              />
              <Heading className="hotkeys_sub-header">
                {t("HotkeysUploadingObjects")}
              </Heading>
              <UploadBlock
                t={t}
                textStyles={textStyles}
                keyTextStyles={keyTextStyles}
              />
            </>
          )}
          <Heading className="hotkeys_sub-header">
            {t("HotkeysSelection")}
          </Heading>
          <SelectionBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
            CtrlKey={CtrlKey}
          />
          <Heading className="hotkeys_sub-header">{t("HotkeysMove")}</Heading>
          <MoveBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
            CtrlKey={CtrlKey}
          />
          <Heading className="hotkeys_sub-header">
            {t("HotkeysActions")}
          </Heading>
          <ActionsBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
            CtrlKey={CtrlKey}
          />
          <Heading className="hotkeys_sub-header">
            {t("HotkeysApplicationActions")}
          </Heading>
          <ApplicationActionsBlock
            t={t}
            textStyles={textStyles}
            keyTextStyles={keyTextStyles}
            CtrlKey={CtrlKey}
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

HotkeyPanel.defaultProps = { theme: Base };

export default inject(({ auth, publicRoomStore }) => {
  const {
    hotkeyPanelVisible,
    setHotkeyPanelVisible,
    theme,
  } = auth.settingsStore;

  return {
    visible: hotkeyPanelVisible,
    setHotkeyPanelVisible,
    theme,
    isVisitor: auth?.userStore?.user?.isVisitor || publicRoomStore.isPublicRoom,
  };
})(
  withTranslation(["HotkeysPanel", "Article", "Common", "Files"])(
    observer(HotkeyPanel)
  )
);
