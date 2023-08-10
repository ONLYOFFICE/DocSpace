import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";

import { useStore } from "SRC_DIR/store";
import BrandingPage from "client/BrandingPage";

const Branding = () => {
  const { t } = useTranslation(["Settings"]);

  const { authStore } = useStore();
  const { setDocumentTitle } = authStore;

  useEffect(() => {
    setDocumentTitle(t("Branding"));
  }, []);

  return <BrandingPage />;
};

export default Branding;
