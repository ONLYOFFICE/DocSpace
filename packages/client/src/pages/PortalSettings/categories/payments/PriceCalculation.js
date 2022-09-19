import React, { useEffect } from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import SelectUsersCountContainer from "./sub-components/SelectUsersCountContainer";
import TotalTariffContainer from "./sub-components/TotalTariffContainer";
import { smallTablet } from "@docspace/components/utils/device";
import toastr from "@docspace/components/toast/toastr";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import axios from "axios";
import { combineUrl } from "@docspace/common/utils";
import ButtonContainer from "./sub-components/ButtonContainer";
import { Trans } from "react-i18next";

const StyledBody = styled.div`
  border-radius: 12px;
  border: 1px solid #d0d5da;
  max-width: 320px;

  @media ${smallTablet} {
    max-width: 600px;
  }

  padding: 24px;
  box-sizing: border-box;

  .payment_main-title {
    margin-bottom: 24px;
  }
  .payment_price_user {
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f3f4f4;
    margin-top: 24px;
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
}) => {
  const isAlreadyPaid = !isFreeTariff; //TODO:

  useEffect(() => {
    initializeInfo(isAlreadyPaid);

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

      await axios
        .put(
          combineUrl(AppServerConfig.apiPrefixURL, "/portal/payment/url"),
          { quantity: { admin: value } },
          {
            cancelToken: source.token,
          }
        )
        .then((response) => {
          setPaymentLink(response.data.response);
          setIsLoading(false);
        })
        .catch((thrown) => {
          setIsLoading(false);
          if (axios.isCancel(thrown)) {
            console.log("Request canceled", thrown.message);
          } else {
            console.error(thrown);
            toastr.error(thrown);
          }
          return;
        });
    }, 1000);
  };

  const isDisabled = isFreeTariff
    ? false
    : (!user.isOwner && !user.isAdmin) || !isPayer;

  const color = isDisabled ? { color: theme.text.disableColor } : {};

  return (
    <StyledBody>
      <Text
        fontSize="16px"
        fontWeight={600}
        noSelect
        {...color}
        className="payment_main-title"
      >
        {!isGracePeriod && !isNotPaidPeriod
          ? t("PriceCalculation")
          : t("YourPrice")}
      </Text>
      {!isGracePeriod && !isNotPaidPeriod && (
        <SelectUsersCountContainer
          isDisabled={isDisabled}
          setShoppingLink={setShoppingLink}
          isAlreadyPaid={isAlreadyPaid}
        />
      )}
      <div className="payment_price_user">
        <Text
          noSelect
          fontSize={"11px"}
          color={
            isDisabled
              ? theme.client.settings.payment.disabledPriceColor
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
                  ? theme.client.settings.payment.disabledPriceColor
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

      <TotalTariffContainer t={t} isDisabled={isDisabled} />
      <ButtonContainer
        isDisabled={isDisabled}
        t={t}
        isAlreadyPaid={isAlreadyPaid}
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
