import React, { useEffect, useState } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { useTranslation, Trans } from "react-i18next";
import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
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
import { getUser } from "@docspace/common/api/people";

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

let dueDate, fromDate, byDate, payerInfo, delayDaysCount;
const PaymentsPage = ({
  pricePerManager,
  setPortalPaymentsQuotas,
  setPortalTariff,
  language,
  portalTariff,
  portalPaymentQuotas,
  isFreeTariff,
  isGracePeriod,
  theme,
  setPaymentAccount,
  currencySymbol,
  isNotPaid,
  setSalesEmail,
  setRangeBound,
  range,
  payerId,
  user,
}) => {
  const { t, ready } = useTranslation(["Payments", "Settings"]);

  const [isInitialLoading, setIsInitialLoading] = useState(true);

  useEffect(() => {
    setDocumentTitle(t("Settings:Payments"));
  }, [ready]);

  useEffect(() => {
    moment.locale(language);

    if (portalTariff) {
      dueDate = moment(
        isGracePeriod || isNotPaid
          ? portalTariff.delayDueDate
          : portalTariff.dueDate
      ).format("LL");

      if (isGracePeriod) {
        const fromDateMoment = moment(portalTariff.dueDate);
        const byDateMoment = moment(portalTariff.delayDueDate);

        fromDate = fromDateMoment.format("L");
        byDate = byDateMoment.format("L");

        delayDaysCount = moment(portalTariff.dueDate).to(
          moment(portalTariff.delayDueDate),
          true
        );

        return;
      }
    }
  }, [portalTariff]);
  useEffect(() => {
    (async () => {
      const requests = [];

      requests.push(
        setSalesEmail(),
        setRangeBound(range.value, range.min, range.max)
      );

      if (Object.keys(portalPaymentQuotas).length === 0)
        requests.push(setPortalPaymentsQuotas());

      if (Object.keys(portalTariff).length === 0) {
        requests.push(setPortalTariff(), setPaymentAccount());
      }

      if (portalTariff && portalTariff.state !== TariffState.Trial)
        requests.push(setPaymentAccount());

      try {
        await Promise.all(requests);
      } catch (error) {
        toastr.error(error);
      }

      try {
        if (isFreeTariff) return;
        payerInfo = await getUser(payerId);
      } catch (e) {
        payerInfo = null;
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

  const convertedPrice = `${currencySymbol}${pricePerManager}`;
  const payer = user.id === payerId;

  return isInitialLoading ? (
    <Loaders.PaymentsLoader />
  ) : (
    <StyledBody theme={theme}>
      {isNotPaid ? (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className="payment-info_expired-period"
        >
          <Trans t={t} i18nKey="BusinessExpired" ns="Payments">
            {{ date: dueDate }}
          </Trans>
        </Text>
      ) : (
        <Text noSelect fontSize="16px" isBold>
          {isFreeTariff ? t("StartupTitle") : t("BusinessTitle")}
        </Text>
      )}

      {!isFreeTariff && (
        <PayerInformationContainer payerInfo={payerInfo} payer={payer} />
      )}

      <CurrentTariffContainer />

      {isNotPaid ? (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className="payment-info_suggestion "
        >
          {t("RenewSubscription")}
        </Text>
      ) : isGracePeriod ? (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className="payment-info_grace-period"
        >
          <Trans t={t} i18nKey="DelayedPayment" ns="Payments">
            {{ date: dueDate }}
          </Trans>
        </Text>
      ) : (
        <Text
          noSelect
          fontSize="16px"
          isBold
          className="payment-info_suggestion "
        >
          {isFreeTariff ? t("StartupSuggestion") : t("BusinessSuggestion")}
        </Text>
      )}

      {isGracePeriod && (
        <Text noSelect fontSize={"14"}>
          <Trans t={t} i18nKey="GracePeriodActivatedDescription" ns="Payments">
            Grace period activated from <strong>{{ fromDate }}</strong> -
            <strong>{{ byDate }}</strong> (<strong>{{ delayDaysCount }}</strong>
            ).
          </Trans>
        </Text>
      )}

      {!isFreeTariff && !isGracePeriod && !isNotPaid && (
        <Text noSelect fontSize={"14"} className="payment-info_managers-price">
          <Trans t={t} i18nKey="BusinessFinalDateInfo" ns="Payments">
            {{ finalDate: dueDate }}
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
        <PriceCalculation t={t} payer={payer} />
        {!isGracePeriod && !isNotPaid && <BenefitsContainer t={t} />}
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
    setPortalPaymentsQuotas,
    setPortalTariff,
    language,
    portalTariff,
    priceInfoPerManager,
    portalPaymentQuotas,
    isFreeTariff,
    isGracePeriod,

    setCurrencies,
    isNotPaid,
    userStore,
  } = auth;

  const { organizationName, theme } = auth.settingsStore;
  const {
    setTariffsInfo,
    tariffsInfo,
    setPaymentAccount,
    setSalesEmail,
    setRangeBound,
  } = payments;
  const { user } = userStore;
  const { currencySymbol, value, range } = priceInfoPerManager;

  const payerId = portalTariff.customerId;

  return {
    isFreeTariff,
    setPortalPaymentsQuotas,
    setPortalTariff,
    portalTariff,
    language,
    organizationName,
    setTariffsInfo,
    tariffsInfo,
    isGracePeriod,
    pricePerManager: value,
    portalPaymentQuotas,
    theme,
    setPaymentAccount,

    setCurrencies,
    currencySymbol: currencySymbol,
    isNotPaid,
    setSalesEmail,
    setRangeBound,
    range,
    payerId,
    user,
  };
})(withRouter(observer(PaymentsPage)));
