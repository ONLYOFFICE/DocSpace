import React from "react";
import { withTranslation } from "react-i18next";

const InfoPanelHeaderContent = ({ t }) => {
  return <>{t("Common:Info")}</>;
};

export default withTranslation(["InfoPanel", "Common"])(InfoPanelHeaderContent);
