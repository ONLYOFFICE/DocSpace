import React from "react";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

const useSettings = ({ t, isSettingsPage, location }) => {
  React.useEffect(() => {
    if (!isSettingsPage) return;

    setDocumentTitle(t("Common:Settings"));
  }, [isSettingsPage, location]);
};

export default useSettings;
