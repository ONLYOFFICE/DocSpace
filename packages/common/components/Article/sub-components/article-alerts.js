import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import ArticleTeamTrainingAlert from "./article-team-training";
import ArticlePaymentAlert from "./article-payment-alert";
import { StyledArticleAlertsComponent } from "../styled-article";

const ArticleAlerts = ({
  showText,
  isNonProfit,
  isGracePeriod,
  isFreeTariff,
  isPaymentPageAvailable,
  isTeamTrainingAlertAvailable,
}) => {
  return (
    <StyledArticleAlertsComponent>
      {isPaymentPageAvailable &&
        !isNonProfit &&
        (isFreeTariff || isGracePeriod) &&
        showText && <ArticlePaymentAlert isFreeTariff={isFreeTariff} />}

      {isTeamTrainingAlertAvailable && showText && <ArticleTeamTrainingAlert />}
    </StyledArticleAlertsComponent>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const {
      currentQuotaStore,
      settingsStore,
      isPaymentPageAvailable,
      isTeamTrainingAlertAvailable,
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
    };
  })(observer(ArticleAlerts))
);
