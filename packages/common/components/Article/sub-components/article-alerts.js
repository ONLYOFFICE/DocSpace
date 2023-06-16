import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";

import ArticleTeamTrainingAlert from "./article-team-training";
import ArticleSubmitToFormGalleryAlert from "./article-submit-to-form-gallery";
import ArticlePaymentAlert from "./article-payment-alert";
import { StyledArticleAlertsComponent } from "../styled-article";
import { ARTICLE_ALERTS } from "@docspace/client/src/helpers/constants";

const ArticleAlerts = ({
  showText,
  isNonProfit,
  isGracePeriod,
  isFreeTariff,
  isPaymentPageAvailable,
  isTeamTrainingAlertAvailable,
  isSubmitToGalleryAlertAvailable,
  articleAlertsData,
  incrementIndexOfArticleAlertsData,
}) => {
  useEffect(() => {
    incrementIndexOfArticleAlertsData();
  }, []);

  return (
    <StyledArticleAlertsComponent>
      {isPaymentPageAvailable &&
        !isNonProfit &&
        (isFreeTariff || isGracePeriod) &&
        showText && <ArticlePaymentAlert isFreeTariff={isFreeTariff} />}

      {isTeamTrainingAlertAvailable &&
        articleAlertsData.current === ARTICLE_ALERTS.TeamTraining &&
        articleAlertsData.available.includes(ARTICLE_ALERTS.TeamTraining) &&
        showText && <ArticleTeamTrainingAlert />}

      {isSubmitToGalleryAlertAvailable &&
        articleAlertsData.current === ARTICLE_ALERTS.SubmitToFormGallery &&
        articleAlertsData.available.includes(
          ARTICLE_ALERTS.SubmitToFormGallery
        ) &&
        showText && <ArticleSubmitToFormGalleryAlert />}
    </StyledArticleAlertsComponent>
  );
};

export default inject(({ auth }) => {
  const {
    currentQuotaStore,
    settingsStore,
    isPaymentPageAvailable,
    isTeamTrainingAlertAvailable,
    isSubmitToGalleryAlertAvailable,
    currentTariffStatusStore,
  } = auth;
  const { isFreeTariff, isNonProfit } = currentQuotaStore;
  const { isGracePeriod } = currentTariffStatusStore;
  const { showText, articleAlertsData, incrementIndexOfArticleAlertsData } =
    settingsStore;

  return {
    showText,
    isNonProfit,
    isGracePeriod,
    isFreeTariff,
    isPaymentPageAvailable,
    isTeamTrainingAlertAvailable,
    isSubmitToGalleryAlertAvailable,
    articleAlertsData,
    incrementIndexOfArticleAlertsData,
  };
})(observer(ArticleAlerts));
