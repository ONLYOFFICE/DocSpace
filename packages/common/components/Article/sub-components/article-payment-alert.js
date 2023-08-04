import React from "react";
import { inject, observer } from "mobx-react";
import { useNavigate } from "react-router-dom";
import { useTranslation, Trans } from "react-i18next";

import { combineUrl } from "@docspace/common/utils";

import AlertComponent from "../../AlertComponent";
import Loaders from "../../Loaders";

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const ArticlePaymentAlert = ({
  isFreeTariff,
  theme,
  currentTariffPlanTitle,
  toggleArticleOpen,
}) => {
  const { t, ready } = useTranslation("Common");

  const navigate = useNavigate();

  const onClick = () => {
    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    navigate(paymentPageUrl);
    toggleArticleOpen();
  };

  const title = isFreeTariff ? (
    <Trans t={t} i18nKey="FreeStartupPlan" ns="Common">
      {{ planName: currentTariffPlanTitle }}
    </Trans>
  ) : (
    t("Common:LatePayment")
  );

  const description = isFreeTariff
    ? t("Common:GetMoreOptions")
    : t("Common:PayBeforeTheEndGracePeriod");

  const additionalDescription = isFreeTariff
    ? t("Common:ActivatePremiumFeatures")
    : t("Common:GracePeriodActivated");

  const color = isFreeTariff
    ? theme.catalog.paymentAlert.color
    : theme.catalog.paymentAlert.warningColor;

  const isShowLoader = !ready;

  return isShowLoader ? (
    <Loaders.Rectangle width="210px" height="88px" />
  ) : (
    <AlertComponent
      id="document_catalog-payment-alert"
      borderColor={color}
      titleColor={color}
      onAlertClick={onClick}
      title={title}
      titleFontSize="11px"
      description={description}
      additionalDescription={additionalDescription}
      needArrowIcon
    />
  );
};

export default inject(({ auth }) => {
  const { currentQuotaStore, settingsStore } = auth;
  const { currentTariffPlanTitle } = currentQuotaStore;
  const { theme, toggleArticleOpen } = settingsStore;

  return {
    toggleArticleOpen,
    theme,
    currentTariffPlanTitle,
  };
})(observer(ArticlePaymentAlert));
