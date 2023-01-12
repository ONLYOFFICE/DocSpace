import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { Trans } from "react-i18next";
import Text from "@docspace/components/text";
import ArrowRightIcon from "@docspace/client/public/images/arrow.right.react.svg";
import { StyledArticlePaymentAlert } from "../styled-article";
import styled from "styled-components";
import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  margin: auto 0;
  path {
    fill: ${(props) => props.color};
  }
`;

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const ArticlePaymentAlert = ({
  pricePerManager,
  isFreeTariff,
  theme,
  currencySymbol,
  currentTariffPlanTitle,
  toggleArticleOpen,
  tariffPlanTitle,
  t,
}) => {
  const onClick = () => {
    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    history.push(paymentPageUrl);
    toggleArticleOpen();
  };

  return (
    <StyledArticlePaymentAlert
      onClick={onClick}
      isFreeTariff={isFreeTariff}
      theme={theme}
      id="document_catalog-payment-alert"
    >
      <div>
        <Text className="article-payment_border">
          {isFreeTariff ? (
            <Trans t={t} i18nKey="FreeStartupPlan" ns="Payments">
              {{ planName: currentTariffPlanTitle }}
            </Trans>
          ) : (
            t("Payments:LatePayment")
          )}
        </Text>
        <Text fontWeight={600}>
          {isFreeTariff
            ? t("Payments:ActivateBusinessPlan", { planName: tariffPlanTitle })
            : t("Payments:GracePeriodActivated")}
        </Text>
        <Text noSelect fontSize={"12px"}>
          {isFreeTariff ? (
            <>
              {pricePerManager ? (
                <Trans t={t} i18nKey="PerUserMonth" ns="Payments">
                  From {{ currencySymbol }}
                  {{ price: pricePerManager }} per admin/month
                </Trans>
              ) : (
                <></>
              )}
            </>
          ) : (
            t("Payments:PayBeforeTheEndGracePeriod")
          )}
        </Text>
      </div>

      <StyledArrowRightIcon />
    </StyledArticlePaymentAlert>
  );
};

export default withRouter(
  inject(({ auth }) => {
    const { paymentQuotasStore, currentQuotaStore } = auth;
    const { currentTariffPlanTitle, isFreeTariff } = currentQuotaStore;
    const { theme } = auth;
    const { planCost, tariffPlanTitle } = paymentQuotasStore;

    return {
      isFreeTariff,
      pricePerManager: planCost.value,
      theme,
      currencySymbol: planCost.currencySymbol,
      currentTariffPlanTitle,
      tariffPlanTitle,
    };
  })(observer(ArticlePaymentAlert))
);
