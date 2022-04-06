import React from "react";
import { withTranslation } from "react-i18next";

const InfoPanelHeaderContent = ({ t }) => {
  return <>{t("Info")}</>;
};

export default withTranslation(["InfoPanel"])(InfoPanelHeaderContent);
