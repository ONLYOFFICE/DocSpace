import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";

import { useStore } from "SRC_DIR/store";

const Restore = () => {
  const { t } = useTranslation(["Settings"]);

  const { authStore } = useStore();
  const { setDocumentTitle } = authStore;

  useEffect(() => {
    setDocumentTitle(t("RestoreBackup"));
  }, []);

  return <h1>Restore</h1>;
};

export default Restore;
