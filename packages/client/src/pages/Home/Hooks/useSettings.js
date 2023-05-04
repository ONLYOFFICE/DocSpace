import React from "react";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

const useSettings = ({ t, isSettingsPage, location, clearFiles }) => {
  React.useEffect(() => {
    if (!isSettingsPage) return;

    clearFiles();

    setDocumentTitle(t("Common:Settings"));
  }, [isSettingsPage, location]);
};

export default useSettings;
