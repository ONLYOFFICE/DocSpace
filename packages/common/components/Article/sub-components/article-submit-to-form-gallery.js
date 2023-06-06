import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import { useTranslation } from "react-i18next";

import AlertComponent from "../../AlertComponent";
import Loaders from "../../Loaders";

const ArticleSubmitToFormGalleryAlert = ({
  theme,
  setSubmitToGalleryDialogVisible,
}) => {
  const { t, ready } = useTranslation("Common", "FormGallery");
  const [isClose, setIsClose] = useState(
    localStorage.getItem("submitToFormGalleryAlertClose")
  );

  const onSubmitToFormGallery = () => {
    setSubmitToGalleryDialogVisible(true);
  };

  const onClose = () => {
    localStorage.setItem("submitToFormGalleryAlertClose", true);
    setIsClose(true);
  };

  if (isClose) return null;

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
  const { theme } = auth.settingsStore;
  const { setSubmitToGalleryDialogVisible } = dialogsStore;

  return {
    theme,
    setSubmitToGalleryDialogVisible,
  };
})(observer(ArticleSubmitToFormGalleryAlert));
