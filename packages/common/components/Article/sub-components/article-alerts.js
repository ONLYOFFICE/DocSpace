import React from "react";
import { inject, observer } from "mobx-react";

import ArticleTeamTrainingAlert from "./article-team-training";
import ArticlePaymentAlert from "./article-payment-alert";
import ArticleEnterpriseAlert from "./article-enterprise-alert";
import { StyledArticleAlertsComponent } from "../styled-article";

const ArticleAlerts = ({
  showText,
  isNonProfit,
  isGracePeriod,
  isFreeTariff,
  isPaymentPageAvailable,
  isTeamTrainingAlertAvailable,
  isLicenseExpiring,
  isLicenseDateExpired,
  isEnterprise,
  isTrial,
  standalone,
}) => {
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
      {isTeamTrainingAlertAvailable && showText && <ArticleTeamTrainingAlert />}
    </StyledArticleAlertsComponent>
  );
};

export default inject(({ auth }) => {
  const {
    currentQuotaStore,
    settingsStore,
    isPaymentPageAvailable,
    isTeamTrainingAlertAvailable,
    currentTariffStatusStore,
    isEnterprise,
  } = auth;
  const { isFreeTariff, isNonProfit, isTrial } = currentQuotaStore;
  const { isGracePeriod, isLicenseExpiring, isLicenseDateExpired } =
    currentTariffStatusStore;
  const { showText, standalone } = settingsStore;

  return {
    isEnterprise,
    showText,
    isNonProfit,
    isGracePeriod,
    isFreeTariff,
    isPaymentPageAvailable,
    isTeamTrainingAlertAvailable,
    isLicenseExpiring,
    isLicenseDateExpired,
    isTrial,
    standalone,
  };
})(observer(ArticleAlerts));
