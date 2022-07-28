import React from "react";
import { inject, observer } from "mobx-react";
import AboutDialog from "SRC_DIR/pages/About/AboutDialog";
import DebugInfoDialog from "SRC_DIR/pages/DebugInfo";

const DialogsWrapper = (props) => {
  const {
    t,
    isAboutDialogVisible,
    setIsAboutDialogVisible,
    personal,
    buildVersionInfo,
    isDebugDialogVisible,
    setIsDebugDialogVisible,
  } = props;

  return (
    <>
      <AboutDialog
        t={t}
        visible={isAboutDialogVisible}
        onClose={() => setIsAboutDialogVisible(false)}
        personal={personal}
        buildVersionInfo={buildVersionInfo}
      />
      <DebugInfoDialog
        visible={isDebugDialogVisible}
        onClose={() => setIsDebugDialogVisible(false)}
      />
    </>
  );
};

export default inject(({ auth, profileActionsStore }) => {
  const { settingsStore } = auth;
  const { personal, buildVersionInfo } = settingsStore;
  const {
    isAboutDialogVisible,
    setIsAboutDialogVisible,
    isDebugDialogVisible,
    setIsDebugDialogVisible,
  } = profileActionsStore;

  return {
    personal,
    buildVersionInfo,
    isAboutDialogVisible,
    setIsAboutDialogVisible,
    isDebugDialogVisible,
    setIsDebugDialogVisible,
  };
})(observer(DialogsWrapper));
