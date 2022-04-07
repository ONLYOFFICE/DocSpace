import React from "react";
import { withTranslation } from "react-i18next";

const InfoPanelHeaderContent = ({ t, isGallery }) => {
  return <>{isGallery ? t("FormTemplateInfo") : t("Info")}</>;
};

export default withTranslation(["InfoPanel", "FormGallery"])(
  InfoPanelHeaderContent
);
