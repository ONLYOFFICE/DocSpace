import React from "react";
import { inject, observer } from "mobx-react";

import ArticleTeamTrainingAlert from "./article-team-training";
import ArticleSubmitToFormGalleryAlert from "./article-submit-to-form-gallery";
import ArticlePaymentAlert from "./article-payment-alert";
import { StyledArticleAlertsComponent } from "../styled-article";

const ArticleAlerts = ({
  showText,
  isNonProfit,
  isGracePeriod,
  isFreeTariff,
  isPaymentPageAvailable,
  isTeamTrainingAlertAvailable,
  isSubmitToGalleryAlertAvailable,
}) => {
  //TODO-mushka clear up about alert switchind functionality, implement and return training alert
  return (
    <StyledArticleAlertsComponent>
      {isPaymentPageAvailable &&
        !isNonProfit &&
        (isFreeTariff || isGracePeriod) &&
        showText && <ArticlePaymentAlert isFreeTariff={isFreeTariff} />}

      {isTeamTrainingAlertAvailable && showText && <ArticleTeamTrainingAlert />}

      {isSubmitToGalleryAlertAvailable && showText && (
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
  } = auth;
  const { isFreeTariff, isNonProfit } = currentQuotaStore;
  const { isGracePeriod } = currentTariffStatusStore;
  const { showText } = settingsStore;

  return {
    showText,
    isNonProfit,
    isGracePeriod,
    isFreeTariff,
    isPaymentPageAvailable,
    isTeamTrainingAlertAvailable,
    isSubmitToGalleryAlertAvailable,
  };
})(observer(ArticleAlerts));
