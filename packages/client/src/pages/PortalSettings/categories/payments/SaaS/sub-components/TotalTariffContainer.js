import React from "react";
import styled, { css } from "styled-components";
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
    .total-tariff_description {
      margin: auto;
    }
    .payment_price_month-text {
      margin: auto 0;
      margin-bottom: 9px;
      margin-left: 8px;
    }
    .payment_price_month-text,
    .payment_price_price-text {
      ${(props) =>
        props.isDisabled &&
        css`
          color: ${props.theme.client.settings.payment.priceContainer
            .disableColor};
        `};
    }
  }

  button {
    width: 100%;
  }
`;

const TotalTariffContainer = ({
  t,
  maxAvailableManagersCount,
  isDisabled,
  theme,
  totalPrice,
  isNeedRequest,
  currencySymbol,
}) => {
  

  return (
    <StyledBody isDisabled={isDisabled} theme={theme}>
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
            <Trans t={t} i18nKey="TotalPricePerMonth" ns="Payments">
              ""
              <Text
                fontSize="48px"
                as="span"
                textAlign={"center"}
                fontWeight={600}
                className="payment_price_price-text"
                noSelect
              >
                {{ currencySymbol }}
              </Text>
              <Text
                fontSize="48px"
                as="span"
                fontWeight={600}
                className="payment_price_price-text"
                noSelect
              >
                {{ price: totalPrice }}
              </Text>
              <Text
                as="span"
                fontWeight={600}
                fontSize="16px"
                className="payment_price_month-text"
                noSelect
              >
                /month
              </Text>
            </Trans>
          </>
        )}
      </div>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { paymentQuotasStore } = auth;
  const { theme } = auth.settingsStore;
  const {
    isLoading,
    totalPrice,
    isNeedRequest,
    maxAvailableManagersCount,
  } = payments;

  const { planCost } = paymentQuotasStore;
  return {
    theme,
    totalPrice,
    isLoading,
    isNeedRequest,
    maxAvailableManagersCount,
    currencySymbol: planCost.currencySymbol,
  };
})(observer(TotalTariffContainer));
