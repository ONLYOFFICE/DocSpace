import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import PropTypes from "prop-types";
import Loaders from "@docspace/common/components/Loaders";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import CurrentTariffContainer from "./CurrentTariffContainer";
import PriceCalculation from "./PriceCalculation";
import BenefitsContainer from "./BenefitsContainer";
import { smallTablet } from "@docspace/components/utils/device";
import ContactContainer from "./ContactContainer";
import toastr from "@docspace/components/toast/toastr";
import moment from "moment";
import { HelpButton } from "@docspace/components";
import PayerInformationContainer from "./PayerInformationContainer";
import { TariffState } from "@docspace/common/constants";
import { getUserByEmail } from "@docspace/common/api/people";

const StyledBody = styled.div`
  max-width: 660px;

  .payment-info_suggestion,
  .payment-info_grace-period {
    margin-bottom: 12px;
  }
  .payment-info_grace-period,
  .payment-info_expired-period {
    color: ${(props) => props.theme.client.payments.delayColor};
  }
  .payment-info {
    display: grid;
    grid-template-columns: repeat(2, minmax(100px, 320px));
    grid-gap: 20px;
    margin-bottom: 20px;
    @media ${smallTablet} {
      grid-template-columns: 1fr;
      grid-template-rows: 1fr 1fr;
    }
  }
  .payment-info_wrapper {
    display: flex;
    margin-bottom: 20px;
    margin-top: 12px;
    div {
      margin: auto 0;
    }
    .payment-info_managers-price {
      margin-right: 8px;
    }
  }
`;

let paymentTerm,
  fromDate,
  byDate,
  delayDaysCount,
  payerInfo = null;
const PaymentsPage = ({
  setPortalPaymentQuotas,
  language,
  isFreeTariff,
  isGracePeriod,
  theme,
  setPaymentAccount,
  isNotPaidPeriod,
  getSettingsPayment,
  setRangeBound,
  payerEmail,
  user,
  isPaidPeriod,
  currencySymbol,
  startValue,
  dueDate,
  delayDueDate,
  portalStatus,
  replaceFeaturesValues,
  portalPaymentQuotasFeatures,
  currentTariffPlanTitle,
}) => {
  const { t, ready } = useTranslation(["Payments", "Settings"]);

  const [isInitialLoading, setIsInitialLoading] = useState(true);

  useEffect(() => {
    setDocumentTitle(t("Settings:Payments"));
  }, [ready]);
  useEffect(() => {
    if (ready && portalPaymentQuotasFeatures.length !== 0)
      replaceFeaturesValues(t);
  }, [ready, portalPaymentQuotasFeatures]);

  const gracePeriodDays = () => {
    const fromDateMoment = moment(dueDate);
    const byDateMoment = moment(delayDueDate);

    fromDate = fromDateMoment.format("L");
    byDate = byDateMoment.format("L");

    delayDaysCount = fromDateMoment.to(byDateMoment, true);
  };

  const setPortalDates = () => {
    paymentTerm = moment(
      isGracePeriod || isNotPaidPeriod ? delayDueDate : dueDate
    ).format("LL");

    isGracePeriod && gracePeriodDays();
  };

  useEffect(() => {
    (async () => {
      moment.locale(language);

      const requests = [];

      requests.push(getSettingsPayment());

      if (!currencySymbol && !startValue)
        requests.push(setPortalPaymentQuotas());

      if (portalStatus !== TariffState.Trial)
        requests.push(setPaymentAccount());

      try {
        await Promise.all(requests);
        setRangeBound();
        setPortalDates();
      } catch (error) {
        toastr.error(error);
      }

      try {
        if (!isFreeTariff) payerInfo = await getUserByEmail(payerEmail);
      } catch (e) {
        console.error(e);
      }

      setIsInitialLoading(false);
    })();
  }, []);

  const renderTooltip = () => {
    return (
      <>
        <HelpButton
          iconName={"/static/images/help.react.svg"}
          tooltipContent={
            <>
              <Text isBold>{t("ManagerTypesDescription")}</Text>
              <br />
              <Text isBold>{t("Administrator")}</Text>
              <Text>{t("AdministratorDescription")}</Text>
              <br />
              <Text isBold>{t("RoomManager")}</Text>
              <Text>{t("RoomManagerDescription")}</Text>
            </>
          }
        />
      </>
    );
  };

  const textComponent = (elem, className) => {
    return (
      <Text noSelect fontSize="16px" isBold className={className}>
        {elem}
      </Text>
    );
  };

  const currentPlanTitle = () => {
    if (isFreeTariff) {
      return textComponent(
        <Trans t={t} i18nKey="StartupTitle" ns="Payments">
          {{ planName: currentTariffPlanTitle }}
        </Trans>
      );
    }

    if (isPaidPeriod || isGracePeriod) {
      return textComponent(
        <Trans t={t} i18nKey="BusinessTitle" ns="Payments">
          {{ planName: currentTariffPlanTitle }}
        </Trans>
      );
    }

    return;
  };

  const expiredTitleSubscriptionWarning = () => {
    return textComponent(
      <Trans t={t} i18nKey="BusinessExpired" ns="Payments">
        {{ date: paymentTerm }} {{ planName: currentTariffPlanTitle }}
      </Trans>,
      "payment-info_expired-period"
    );
  };

  const planSuggestion = () => {
    if (isFreeTariff) {
      return textComponent(
        <Trans t={t} i18nKey="StartupSuggestion" ns="Payments">
          {{ planName: currentTariffPlanTitle }}
        </Trans>,
        "payment-info_suggestion"
      );
    }

    if (isPaidPeriod) {
      return textComponent(
        <Trans t={t} i18nKey="BusinessSuggestion" ns="Payments">
          {{ planName: currentTariffPlanTitle }}
        </Trans>,
        "payment-info_suggestion"
      );
    }

    if (isNotPaidPeriod) {
      return textComponent(
        <Trans t={t} i18nKey="RenewSubscription" ns="Payments">
          {{ planName: currentTariffPlanTitle }}
        </Trans>,
        "payment-info_suggestion"
      );
    }

    if (isGracePeriod) {
      return textComponent(
        <Trans t={t} i18nKey="DelayedPayment" ns="Payments">
          {{ date: paymentTerm }} {{ planName: currentTariffPlanTitle }}
        </Trans>,
        "payment-info_grace-period"
      );
    }

    return;
  };

  const convertedPrice = `${currencySymbol}${startValue}`;
  const isPayer = user.email === payerEmail;

  return isInitialLoading || !ready ? (
    <Loaders.PaymentsLoader />
  ) : (
    <StyledBody theme={theme}>
      {isNotPaidPeriod ? expiredTitleSubscriptionWarning() : currentPlanTitle()}

      {(!isFreeTariff || (isFreeTariff && isNotPaidPeriod)) && (
        <PayerInformationContainer
          payerInfo={payerInfo}
          isPayer={isPayer}
          payerEmail={payerEmail}
        />
      )}

      <CurrentTariffContainer />

      {planSuggestion()}

      {isGracePeriod && (
        <Text noSelect fontSize={"14px"}>
          <Trans t={t} i18nKey="GracePeriodActivatedDescription" ns="Payments">
            Grace period activated from <strong>{{ fromDate }}</strong> -
            <strong>{{ byDate }}</strong> (<strong>{{ delayDaysCount }}</strong>
            ).
          </Trans>
        </Text>
      )}

      {isPaidPeriod && (
        <Text
          noSelect
          fontSize={"14px"}
          className="payment-info_managers-price"
        >
          <Trans t={t} i18nKey="BusinessFinalDateInfo" ns="Payments">
            {{ finalDate: paymentTerm }}
          </Trans>
        </Text>
      )}

      <div className="payment-info_wrapper">
        <Text
          noSelect
          fontWeight={600}
          fontSize={"14"}
          className="payment-info_managers-price"
        >
          <Trans t={t} i18nKey="StartPrice" ns="Payments">
            {{ price: convertedPrice }}
          </Trans>
        </Text>

        {renderTooltip()}
      </div>
      <div className="payment-info">
        <PriceCalculation t={t} isPayer={isPayer} />
        {!isGracePeriod && !isNotPaidPeriod && <BenefitsContainer t={t} />}
      </div>
      <ContactContainer t={t} />
    </StyledBody>
  );
};

PaymentsPage.propTypes = {
  isLoaded: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const {
    language,
    currentQuotaStore,
    paymentQuotasStore,
    currentTariffStatusStore,
    userStore,
  } = auth;

  const { isFreeTariff, currentTariffPlanTitle } = currentQuotaStore;
  const {
    isNotPaidPeriod,
    isPaidPeriod,
    isGracePeriod,
    customerId,
    dueDate,
    delayDueDate,
    portalStatus,
  } = currentTariffStatusStore;

  const {
    setPortalPaymentQuotas,
    planCost,
    replaceFeaturesValues,
    portalPaymentQuotasFeatures,
  } = paymentQuotasStore;
  const { organizationName, theme } = auth.settingsStore;

  const {
    tariffsInfo,
    setPaymentAccount,
    getSettingsPayment,
    setRangeBound,
  } = payments;

  const { user } = userStore;

  return {
    isFreeTariff,

    language,
    organizationName,
    tariffsInfo,
    isGracePeriod,
    theme,
    setPaymentAccount,
    currencySymbol: planCost.currencySymbol,
    startValue: planCost.value,
    isNotPaidPeriod,
    getSettingsPayment,
    setRangeBound,
    payerEmail: customerId,
    user,
    isPaidPeriod,
    setPortalPaymentQuotas,
    dueDate,
    delayDueDate,
    portalStatus,
    replaceFeaturesValues,
    portalPaymentQuotasFeatures,
    currentTariffPlanTitle,
  };
})(withRouter(observer(PaymentsPage)));
