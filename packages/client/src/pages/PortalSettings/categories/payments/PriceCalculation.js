import React, { useEffect } from "react";
import styled, { css } from "styled-components";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import SelectUsersCountContainer from "./sub-components/SelectUsersCountContainer";
import TotalTariffContainer from "./sub-components/TotalTariffContainer";
import toastr from "@docspace/components/toast/toastr";
import axios from "axios";
//import { combineUrl } from "@docspace/common/utils";
import ButtonContainer from "./sub-components/ButtonContainer";
import { Trans } from "react-i18next";
import { getPaymentLink } from "@docspace/common/api/portal";
import CurrentUsersCountContainer from "./sub-components/CurrentUsersCount";

const StyledBody = styled.div`
  border-radius: 12px;
  border: ${(props) =>
    props.theme.client.settings.payment.priceContainer.border};
  background: ${(props) =>
    props.theme.client.settings.payment.priceContainer.background};
  max-width: 320px;

  padding: 24px;
  box-sizing: border-box;

  .payment_main-title {
    margin-bottom: 24px;
    ${(props) =>
      props.isDisabled &&
      css`
        color: ${props.theme.client.settings.payment.priceContainer
          .disableColor};
      `}
  }
  .payment_price_user {
    display: flex;
    align-items: center;
    justify-content: center;
    background: ${(props) =>
      props.theme.client.settings.payment.priceContainer.backgroundText};
    margin-top: 24px;
    min-height: 38px;
    border-radius: 6px;
    p:first-child {
      margin-right: 8px;
    }
    p {
      margin-bottom: 5px;
      margin-top: 5px;
    }
  }
`;

let timeout = null,
  CancelToken,
  source;

const backUrl = window.location.origin;
const PriceCalculation = ({
  t,
  user,
  theme,
  setPaymentLink,
  setIsLoading,
  maxAvailableManagersCount,
  isFreeTariff,
  isPayer,
  isGracePeriod,
  isNotPaidPeriod,
  initializeInfo,
  priceManagerPerMonth,
  currencySymbol,
  isAlreadyPaid,
  isFreeAfterPaidPeriod,
  managersCount,
}) => {
  useEffect(async () => {
    initializeInfo();

    if (isAlreadyPaid) return;

    try {
      const link = await getPaymentLink(managersCount, source?.token, backUrl);
      setPaymentLink(link);
    } catch (e) {
      toastr.error(t("ErrorNotification"));
    }

    return () => {
      timeout && clearTimeout(timeout);
      timeout = null;
    };
  }, []);

  const setShoppingLink = (value) => {
    if (isAlreadyPaid || value > maxAvailableManagersCount) {
      timeout && clearTimeout(timeout);
      setIsLoading(false);
      return;
    }

    setIsLoading(true);

    timeout && clearTimeout(timeout);
    timeout = setTimeout(async () => {
      if (source) {
        source.cancel();
      }

      CancelToken = axios.CancelToken;
      source = CancelToken.source();

      await getPaymentLink(value, source.token, backUrl)
        .then((link) => {
          setPaymentLink(link);
          setIsLoading(false);
        })
        .catch((thrown) => {
          setIsLoading(false);
          if (axios.isCancel(thrown)) {
            console.log("Request canceled", thrown.message);
          } else {
            console.error(thrown);
            toastr.error(t("ErrorNotification"));
          }
          return;
        });
    }, 1000);
  };

  const isDisabled = isFreeTariff
    ? false
    : (!user.isOwner && !user.isAdmin) || !isPayer;

  const priceInfoPerManager = (
    <div className="payment_price_user">
      <Text
        noSelect
        fontSize={"13px"}
        color={
          isDisabled
            ? theme.client.settings.payment.priceContainer.disablePriceColor
            : theme.client.settings.payment.priceColor
        }
        fontWeight={600}
      >
        <Trans t={t} i18nKey="PerUserMonth" ns="Payments">
          ""
          <Text
            fontSize="16px"
            isBold
            as="span"
            fontWeight={600}
            color={
              isDisabled
                ? theme.client.settings.payment.priceContainer.disablePriceColor
                : theme.client.settings.payment.priceColor
            }
          >
            {{ currencySymbol }}
          </Text>
          <Text fontSize="16px" isBold as="span" fontWeight={600}>
            {{ price: priceManagerPerMonth }}
          </Text>
          per manager/month
        </Trans>
      </Text>
    </div>
  );

  const isNeedPlusSign = managersCount > maxAvailableManagersCount;

  return (
    <StyledBody className="price-calculation-container" isDisabled={isDisabled}>
      <Text
        fontSize="16px"
        fontWeight={600}
        noSelect
        className="payment_main-title"
      >
        {isGracePeriod || isNotPaidPeriod || isFreeAfterPaidPeriod
          ? t("YourPrice")
          : t("PriceCalculation")}
      </Text>
      {isGracePeriod || isNotPaidPeriod || isFreeAfterPaidPeriod ? (
        <CurrentUsersCountContainer isNeedPlusSign={isNeedPlusSign} t={t} />
      ) : (
        <SelectUsersCountContainer
          isNeedPlusSign={isNeedPlusSign}
          isDisabled={isDisabled}
          setShoppingLink={setShoppingLink}
          isAlreadyPaid={isAlreadyPaid}
        />
      )}

      {priceInfoPerManager}

      <TotalTariffContainer t={t} isDisabled={isDisabled} />
      <ButtonContainer
        isDisabled={isDisabled}
        t={t}
        isAlreadyPaid={isAlreadyPaid}
        isFreeAfterPaidPeriod={isFreeAfterPaidPeriod}
      />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const {
    tariffsInfo,
    setPaymentLink,
    setIsLoading,
    setManagersCount,
    maxAvailableManagersCount,
    initializeInfo,
    managersCount,
  } = payments;
  const { theme } = auth.settingsStore;
  const {
    userStore,
    currentTariffStatusStore,
    currentQuotaStore,
    paymentQuotasStore,
  } = auth;
  const { isFreeTariff } = currentQuotaStore;
  const { planCost } = paymentQuotasStore;
  const { isNotPaidPeriod, isGracePeriod } = currentTariffStatusStore;
  const { user } = userStore;

  return {
    managersCount,

    isFreeTariff,
    setManagersCount,
    tariffsInfo,
    theme,
    setPaymentLink,
    setIsLoading,
    maxAvailableManagersCount,
    user,
    isGracePeriod,
    isNotPaidPeriod,
    initializeInfo,
    priceManagerPerMonth: planCost.value,
    currencySymbol: planCost.currencySymbol,
  };
})(observer(PriceCalculation));
