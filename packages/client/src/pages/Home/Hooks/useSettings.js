import React from "react";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

const useSettings = ({ t, isSettingsPage, setIsLoading }) => {
  React.useEffect(() => {
    if (!isSettingsPage) return;

    setDocumentTitle(t("Common:Settings"));

    setIsLoading(false);
  }, [isSettingsPage]);
};

export default useSettings;
