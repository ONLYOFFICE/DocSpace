import HelpReactSvgUrl from "PUBLIC_DIR/images/help.react.svg?url";
import React from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { Trans } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import { size, desktop } from "@docspace/components/utils/device";
import { Consumer } from "@docspace/components/utils/context";
import { HelpButton } from "@docspace/components";

import CurrentTariffContainer from "./CurrentTariffContainer";
import PriceCalculation from "./PriceCalculation";
import BenefitsContainer from "./BenefitsContainer";
import ContactContainer from "./ContactContainer";
import PayerInformationContainer from "./PayerInformationContainer";

const StyledBody = styled.div`
  max-width: 660px;

  .payment-info_suggestion,
  .payment-info_grace-period {
    margin-bottom: 12px;
  }

  .payment-info {
    margin-top: 18px;
    display: grid;
    grid-template-columns: repeat(2, minmax(100px, 320px));
    grid-gap: 20px;
    margin-bottom: 20px;

    @media (max-width: ${size.smallTablet + 40}px) {
      grid-template-columns: 1fr;

      grid-template-rows: ${(props) => "1fr max-content"};

      .price-calculation-container,
      .benefits-container {
        max-width: 600px;
      }
      .select-users-count-container {
        max-width: 520px;
      }
    }

    ${(props) =>
      props.isChangeView &&
      css`
        grid-template-columns: 1fr;
        grid-template-rows: ${(props) => "1fr max-content"};

        .price-calculation-container,
        .benefits-container {
          -webkit-transition: all 0.8s ease;
          transition: all 0.4s ease;
          max-width: 600px;
        }
        .select-users-count-container {
          -webkit-transition: all 0.8s ease;
          transition: all 0.4s ease;
          max-width: 520px;
        }

        @media ${desktop} {
          grid-template-columns: repeat(2, minmax(100px, 320px));
        }
      `}
  }
  .payment-info_wrapper {
    display: flex;

    margin-top: 11px;
    div {
      margin: auto 0;
    }
    .payment-info_managers-price {
      margin-right: 6px;
    }
  }
`;

const PaymentContainer = ({
  isFreeTariff,
  isGracePeriod,
  theme,
  isNotPaidPeriod,
  payerEmail,
  user,
  isPaidPeriod,
  currencySymbol,
  startValue,
  currentTariffPlanTitle,
  tariffPlanTitle,
  expandArticle,
  gracePeriodEndDate,
  delayDaysCount,

  isValidPaymentDateYear,
  isAlreadyPaid,
  paymentDate,
  t,
}) => {
  const renderTooltip = () => {
    return (
      <>
        <HelpButton
          offsetRight={0}
          iconName={HelpReactSvgUrl}
          tooltipContent={
            <>
              <Text isBold>{t("ManagerTypesDescription")}</Text>
              <br />
              <Text isBold>{t("Common:DocSpaceAdmin")}</Text>
              <Text>{t("AdministratorDescription")}</Text>
              <br />
              <Text isBold>{t("Common:RoomAdmin")}</Text>
              <Text>{t("RoomManagerDescription")}</Text>
            </>
          }
        />
      </>
    );
  };

  const currentPlanTitle = () => {
    if (isPaidPeriod || isGracePeriod) {
      return (
        <Text noSelect fontSize="16px" isBold>
          <Trans t={t} i18nKey="BusinessTitle" ns="Payments">
            {{ planName: currentTariffPlanTitle }}
          </Trans>
        </Text>
      );
    }

    return (
      <Text noSelect fontSize="16px" isBold>
        <Trans t={t} i18nKey="StartupTitle" ns="Payments">
          {{ planName: currentTariffPlanTitle }}
        </Trans>
      </Text>
    );
  };

  const expiredTitleSubscriptionWarning = () => {
    return (
      <Text
        noSelect
        fontSize="16px"
        isBold
        color={theme.client.settings.payment.warningColor}
      >
        <Trans t={t} i18nKey="BusinessExpired" ns="Payments">
          {{ date: gracePeriodEndDate }} {{ planName: tariffPlanTitle }}
        </Trans>
      </Text>
    );
  };

  const planSuggestion = () => {
    if (isFreeTariff) {
      return (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className={"payment-info_suggestion"}
        >
          <Trans t={t} i18nKey="StartupSuggestion" ns="Payments">
            {{ planName: tariffPlanTitle }}
          </Trans>
        </Text>
      );
    }

    if (isPaidPeriod) {
      return (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className={"payment-info_suggestion"}
        >
          <Trans t={t} i18nKey="BusinessSuggestion" ns="Payments">
            {{ planName: tariffPlanTitle }}
          </Trans>
        </Text>
      );
    }

    if (isNotPaidPeriod) {
      return (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className={"payment-info_suggestion"}
        >
          <Trans t={t} i18nKey="RenewSubscription" ns="Payments">
            {{ planName: tariffPlanTitle }}
          </Trans>
        </Text>
      );
    }

    if (isGracePeriod) {
      return (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className={"payment-info_grace-period"}
          color={theme.client.settings.payment.warningColor}
        >
          <Trans t={t} i18nKey="DelayedPayment" ns="Payments">
            {{ date: paymentDate }} {{ planName: currentTariffPlanTitle }}
          </Trans>
        </Text>
      );
    }

    return;
  };

  const isPayer = user.email === payerEmail;
  const isFreeAfterPaidPeriod = isFreeTariff && payerEmail?.length !== 0;

  return (
    <Consumer>
      {(context) => (
        <StyledBody
          theme={theme}
          isChangeView={
            context.sectionWidth < size.smallTablet && expandArticle
          }
        >
          {isNotPaidPeriod && isValidPaymentDateYear
            ? expiredTitleSubscriptionWarning()
            : currentPlanTitle()}

          {isAlreadyPaid && <PayerInformationContainer isPayer={isPayer} />}

          <CurrentTariffContainer />

          {planSuggestion()}

          {isGracePeriod && (
            <Text noSelect fontSize={"14px"} lineHeight={"16px"}>
              <Trans t={t} i18nKey="GracePeriodActivatedInfo" ns="Payments">
                Grace period activated
                <strong>
                  from {{ fromDate: paymentDate }} to
                  {{ byDate: gracePeriodEndDate }}
                </strong>
                (days remaining: {{ delayDaysCount }})
              </Trans>{" "}
              <Text as="span" fontSize="14px" lineHeight="16px">
                {t("GracePeriodActivatedDescription")}
              </Text>
            </Text>
          )}

          {isPaidPeriod && !isFreeTariff && (
            <Text
              noSelect
              fontSize={"14px"}
              lineHeight={"16px"}
              className="payment-info_managers-price"
            >
              <Trans t={t} i18nKey="BusinessFinalDateInfo" ns="Payments">
                {{ finalDate: paymentDate }}
              </Trans>
            </Text>
          )}

          {!isGracePeriod && !isNotPaidPeriod && !isFreeAfterPaidPeriod && (
            <div className="payment-info_wrapper">
              <Text
                noSelect
                fontWeight={600}
                fontSize={"14px"}
                className="payment-info_managers-price"
              >
                <Trans t={t} i18nKey="PerUserMonth" ns="Payments">
                  From {{ currencySymbol }}
                  {{ price: startValue }} per admin/month
                </Trans>
              </Text>

              {renderTooltip()}
            </div>
          )}
          <div className="payment-info">
            <PriceCalculation
              t={t}
              isPayer={isPayer}
              isFreeAfterPaidPeriod={isFreeAfterPaidPeriod}
            />

            <BenefitsContainer t={t} />
          </div>
          <ContactContainer t={t} />
        </StyledBody>
      )}
    </Consumer>
  );
};

export default inject(({ auth, payments }) => {
  const {
    currentQuotaStore,
    paymentQuotasStore,
    currentTariffStatusStore,
    userStore,
    settingsStore,
  } = auth;
  const { showText: expandArticle } = settingsStore;

  const { isFreeTariff, currentTariffPlanTitle } = currentQuotaStore;
  const {
    isNotPaidPeriod,
    isPaidPeriod,
    isGracePeriod,
    customerId,
    portalTariffStatus,
  } = currentTariffStatusStore;

  const { planCost, tariffPlanTitle, portalPaymentQuotas } = paymentQuotasStore;

  const { theme } = auth.settingsStore;

  const {
    gracePeriodEndDate,
    delayDaysCount,

    isValidPaymentDateYear,
    isAlreadyPaid,
    paymentDate,
  } = payments;

  const { user } = userStore;

  return {
    paymentDate,
    isAlreadyPaid,
    isValidPaymentDateYear,

    gracePeriodEndDate,
    delayDaysCount,

    expandArticle,
    isFreeTariff,
    tariffPlanTitle,

    isGracePeriod,
    theme,
    currencySymbol: planCost.currencySymbol,
    startValue: planCost.value,
    isNotPaidPeriod,
    payerEmail: customerId,
    user,
    isPaidPeriod,
    currentTariffPlanTitle,
    portalTariffStatus,
    portalPaymentQuotas,
  };
})(withRouter(observer(PaymentContainer)));
