import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import { smallTablet } from "@docspace/components/utils/device";

const StyledBody = styled.div`
  max-width: 272px;
  margin: 0 auto;

  @media ${smallTablet} {
    max-width: 520px;
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
  .payment_price_total-price {
    display: flex;
    justify-content: center;
    min-height: 65px;
    margin-top: 16px;
    margin-bottom: 16px;

    .payment_price_description,
    .payment_price_price-text,
    .total-tariff_description {
      margin-bottom: 0px;
    }
    .payment_price_description {
      margin-top: 16px;
    }

    .payment_price_month-text {
      margin: auto 0;
      margin-bottom: 13px;
    }
  }

  button {
    width: 100%;
  }
`;

const TotalTariffContainer = ({
  t,
  maxAvailableManagersCount,
  pricePerManager,
  isDisabled,
  theme,
  totalPrice,
  isNeedRequest,
  isoCurrencySymbol,
}) => {
  const color = isDisabled ? { color: theme.text.disableColor } : {};

  return (
    <StyledBody>
      <div className="payment_price_user">
        <Text fontSize="16px" textAlign="center" isBold noSelect {...color}>
          {`${isoCurrencySymbol}${pricePerManager}`}
        </Text>
        <Text
          fontSize="11px"
          textAlign="center"
          fontWeight={600}
          color={"#A3A9AE"}
          noSelect
        >
          {t("PerUserMonth")}
        </Text>
      </div>
      <div className="payment_price_total-price">
        {isNeedRequest ? (
          <Text
            noSelect
            fontSize={"14"}
            textAlign="center"
            fontWeight={600}
            className="total-tariff_description"
          >
            <Trans t={t} i18nKey="BusinessRequestDescription" ns="Payments">
              {{ peopleNumber: maxAvailableManagersCount }}
            </Trans>
          </Text>
        ) : (
          <>
            <Text
              fontSize="48px"
              isBold
              textAlign={"center"}
              className="payment_price_price-text"
              noSelect
              {...color}
            >
              {`${isoCurrencySymbol}${totalPrice}`}
            </Text>
            <Text
              fontSize="16px"
              isBold
              textAlign={"center"}
              className="payment_price_month-text"
              noSelect
              {...color}
            >
              {"/month"}
            </Text>
          </>
        )}
      </div>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { pricePerManager, currencies } = auth;
  const { theme } = auth.settingsStore;
  const {
    isLoading,
    totalPrice,
    isNeedRequest,
    maxAvailableManagersCount,
  } = payments;

  return {
    theme,
    pricePerManager,
    totalPrice,
    isLoading,
    isNeedRequest,
    maxAvailableManagersCount,
    isoCurrencySymbol: currencies[0]?.isoCurrencySymbol,
  };
})(observer(TotalTariffContainer));
