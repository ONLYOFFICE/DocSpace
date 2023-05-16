import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
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
  isLicenseDateExpires,
  isEnterprise,
}) => {
  return (
    <StyledArticleAlertsComponent>
      {isPaymentPageAvailable &&
        !isNonProfit &&
        (isFreeTariff || isGracePeriod) &&
        showText && <ArticlePaymentAlert isFreeTariff={isFreeTariff} />}

      {isEnterprise &&
        (isLicenseExpiring || isLicenseDateExpires) &&
        showText && <ArticleEnterpriseAlert />}

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
      isEnterprise,
    } = auth;
    const { isFreeTariff, isNonProfit } = currentQuotaStore;
    const {
      isGracePeriod,
      isLicenseExpiring,
      isLicenseDateExpires,
    } = currentTariffStatusStore;
    const { showText } = settingsStore;

    return {
      isEnterprise,
      showText,
      isNonProfit,
      isGracePeriod,
      isFreeTariff,
      isPaymentPageAvailable,
      isTeamTrainingAlertAvailable,
      isLicenseExpiring,
      isLicenseDateExpires,
    };
  })(observer(ArticleAlerts))
);
