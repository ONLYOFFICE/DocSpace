import React from "react";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";

const useSettings = ({
  t,
  isSettingsPage,
  isPluginsSettingsPage,
  enablePlugins,
  navigate,
  setIsLoading,
}) => {
  React.useEffect(() => {
    if (!enablePlugins && isPluginsSettingsPage) {
      return navigate("/settings", { replace: true });
    }

    setDocumentTitle(t("Common:Settings"));

    setIsLoading(false);
  }, [enablePlugins, isPluginsSettingsPage]);

  React.useEffect(() => {
    if (!isSettingsPage) return;

    setDocumentTitle(t("Common:Settings"));

    setIsLoading(false);
  }, [isSettingsPage]);
};

export default useSettings;
