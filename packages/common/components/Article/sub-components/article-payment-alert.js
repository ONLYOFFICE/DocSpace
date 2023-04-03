import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";

import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";

import AlertComponent from "../../AlertComponent";

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const ArticlePaymentAlert = ({
  pricePerManager,
  isFreeTariff,
  theme,
  currencySymbol,
  setPortalPaymentQuotas,
  currentTariffPlanTitle,
  toggleArticleOpen,
  tariffPlanTitle,
}) => {
  const { t, ready } = useTranslation("Common");

  useEffect(async () => {
    if (isFreeTariff)
      try {
        await setPortalPaymentQuotas();
      } catch (e) {
        console.error(e);
      }
  }, []);

  const onClick = () => {
    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    history.push(paymentPageUrl);
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
    ? pricePerManager && (
        <Trans t={t} i18nKey="PerUserMonth" ns="Common">
          From {{ currencySymbol }}
          {{ price: pricePerManager }} per admin/power user /month
        </Trans>
      )
    : t("Common:PayBeforeTheEndGracePeriod");

  const additionalDescription = isFreeTariff
    ? t("Common:ActivateBusinessPlan", { planName: tariffPlanTitle })
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

export default withRouter(
  inject(({ auth }) => {
    const { paymentQuotasStore, currentQuotaStore, settingsStore } = auth;
    const { currentTariffPlanTitle } = currentQuotaStore;
    const { theme } = settingsStore;
    const {
      setPortalPaymentQuotas,
      planCost,
      tariffPlanTitle,
    } = paymentQuotasStore;

    return {
      setPortalPaymentQuotas,
      pricePerManager: planCost.value,
      theme,
      currencySymbol: planCost.currencySymbol,
      currentTariffPlanTitle,
      tariffPlanTitle,
    };
  })(observer(ArticlePaymentAlert))
);
