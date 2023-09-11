import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";

import ArticleTeamTrainingAlert from "./article-team-training";
import ArticleSubmitToFormGalleryAlert from "./article-submit-to-form-gallery";
import ArticlePaymentAlert from "./article-payment-alert";
import ArticleEnterpriseAlert from "./article-enterprise-alert";
import { StyledArticleAlertsComponent } from "../styled-article";
import { ARTICLE_ALERTS } from "@docspace/client/src/helpers/constants";

const ArticleAlerts = ({
  articleAlertsData,
  incrementIndexOfArticleAlertsData,

  showText,
  isNonProfit,
  isGracePeriod,
  isFreeTariff,
  isPaymentPageAvailable,
  isTeamTrainingAlertAvailable,
  isSubmitToGalleryAlertAvailable,
  isLicenseExpiring,
  isLicenseDateExpired,
  isEnterprise,
  isTrial,
  standalone,
}) => {
  const currentAlert = articleAlertsData.current;
  const availableAlerts = articleAlertsData.available;

  useEffect(() => {
    incrementIndexOfArticleAlertsData();
  }, []);

  const paymentsAlertsComponent = () => {
    if (!standalone) {
      return (
        isPaymentPageAvailable &&
        !isNonProfit &&
        (isFreeTariff || isGracePeriod) &&
        showText && <ArticlePaymentAlert isFreeTariff={isFreeTariff} />
      );
    }

    const isVisibleStandaloneAlert =
      isTrial || isLicenseExpiring || isLicenseDateExpired;

    return (
      isPaymentPageAvailable &&
      isEnterprise &&
      isVisibleStandaloneAlert &&
      showText && <ArticleEnterpriseAlert />
    );
  };

  return (
    <StyledArticleAlertsComponent>
      {paymentsAlertsComponent()}

      {isTeamTrainingAlertAvailable &&
        showText &&
        availableAlerts.includes(ARTICLE_ALERTS.TeamTraining) &&
        currentAlert === ARTICLE_ALERTS.TeamTraining && (
          <ArticleTeamTrainingAlert />
        )}

      {isSubmitToGalleryAlertAvailable &&
        showText &&
        availableAlerts.includes(ARTICLE_ALERTS.SubmitToFormGallery) &&
        currentAlert === ARTICLE_ALERTS.SubmitToFormGallery && (
          <ArticleSubmitToFormGalleryAlert />
        )}
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
    isEnterprise,
  } = auth;
  const { isFreeTariff, isNonProfit, isTrial } = currentQuotaStore;
  const { isGracePeriod, isLicenseExpiring, isLicenseDateExpired } =
    currentTariffStatusStore;
  const {
    showText,
    standalone,
    articleAlertsData,
    incrementIndexOfArticleAlertsData,
  } = settingsStore;

  return {
    articleAlertsData,
    incrementIndexOfArticleAlertsData,
    isEnterprise,
    showText,
    isNonProfit,
    isGracePeriod,
    isFreeTariff,
    isPaymentPageAvailable,
    isTeamTrainingAlertAvailable,
    isSubmitToGalleryAlertAvailable,
    isLicenseExpiring,
    isLicenseDateExpired,
    isTrial,
    standalone,
  };
})(observer(ArticleAlerts));
