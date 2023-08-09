import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";

import { useStore } from "SRC_DIR/store";
import BackupPage from "client/BackupPage";

const Backup = () => {
  const { t } = useTranslation(["Settings"]);

  const { authStore } = useStore();
  const { setDocumentTitle } = authStore;

  useEffect(() => {
    setDocumentTitle(t("Backup"));
  }, []);

  return <BackupPage />;
};

export default Backup;
