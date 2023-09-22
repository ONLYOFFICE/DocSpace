import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import { useTranslation } from "react-i18next";

import AlertComponent from "../../AlertComponent";
import Loaders from "../../Loaders";
import { ARTICLE_ALERTS } from "@docspace/client/src/helpers/constants";

const ArticleSubmitToFormGalleryAlert = ({
  theme,
  setSubmitToGalleryDialogVisible,
  removeAlertFromArticleAlertsData,
}) => {
  const { t, ready } = useTranslation("Common", "FormGallery");

  const onSubmitToFormGallery = () => setSubmitToGalleryDialogVisible(true);
  const onClose = () =>
    removeAlertFromArticleAlertsData(ARTICLE_ALERTS.SubmitToFormGallery);

  return !ready ? (
    <Loaders.Rectangle width="210px" height="112px" />
  ) : (
    <AlertComponent
      titleColor={theme.catalog.teamTrainingAlert.titleColor}
      linkColor={theme.catalog.teamTrainingAlert.linkColor}
      borderColor={theme.catalog.teamTrainingAlert.borderColor}
      title={t("FormGallery:SubmitToGalleryBlockHeader")}
      description={t("FormGallery:SubmitToGalleryBlockBody")}
      linkTitle={t("Common:SubmitToFormGallery")}
      onLinkClick={onSubmitToFormGallery}
      onCloseClick={onClose}
      needCloseIcon
    />
  );
};

export default inject(({ auth, dialogsStore }) => {
  const { theme, removeAlertFromArticleAlertsData } = auth.settingsStore;
  const { setSubmitToGalleryDialogVisible } = dialogsStore;

  return {
    theme,
    removeAlertFromArticleAlertsData,
    setSubmitToGalleryDialogVisible,
  };
})(observer(ArticleSubmitToFormGalleryAlert));
