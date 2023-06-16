import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import { useTranslation } from "react-i18next";

import AlertComponent from "../../AlertComponent";
import Loaders from "../../Loaders";
import { ARTICLE_ALERTS } from "@docspace/client/src/helpers/constants";

const ArticleTeamTrainingAlert = ({
  theme,
  bookTrainingEmail,
  removeAlertFromArticleAlertsData,
}) => {
  const { t, ready } = useTranslation("Common");
  const isShowLoader = !ready;

  const onClose = () =>
    removeAlertFromArticleAlertsData(ARTICLE_ALERTS.TeamTraining);

  return isShowLoader ? (
    <Loaders.Rectangle width="210px" height="88px" />
  ) : (
    <AlertComponent
      titleColor={theme.catalog.teamTrainingAlert.titleColor}
      linkColor={theme.catalog.teamTrainingAlert.linkColor}
      borderColor={theme.catalog.teamTrainingAlert.borderColor}
      title={t("Common:UseLikePro")}
      description={t("Common:BookTeamTraining")}
      link={`mailto:${bookTrainingEmail}`}
      linkTitle={t("Common:BookNow")}
      onCloseClick={onClose}
      needCloseIcon
    />
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;

  const { theme, bookTrainingEmail, removeAlertFromArticleAlertsData } =
    settingsStore;

  return {
    theme,
    bookTrainingEmail,
    removeAlertFromArticleAlertsData,
  };
})(observer(ArticleTeamTrainingAlert));
