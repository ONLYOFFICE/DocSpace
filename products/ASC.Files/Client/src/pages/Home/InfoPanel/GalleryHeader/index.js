import React from "react";
import { withTranslation } from "react-i18next";

const InfoPanelHeaderContent = ({ t }) => {
  return <>{t("FormTemplateInfo")}</>;
};

export default withTranslation(["FormGallery"])(InfoPanelHeaderContent);
