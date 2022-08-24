import React, { useEffect } from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
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
    .payment_price_price-text {
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
  usersCount,
  maxUsersCount,
  maxSliderNumber,
  pricePerManager,
  isDisabled,
  theme,
  onClick,
  isLoading,
  isAlreadyPaid,
  countAdmin,
  totalPrice,
}) => {
  useEffect(() => {}, []);

  const color = isDisabled ? { color: theme.text.disableColor } : {};
  const isNeedRequest = usersCount >= maxUsersCount;

  return (
    <StyledBody>
      <div className="payment_price_user">
        <Text fontSize="16px" textAlign="center" isBold noSelect {...color}>
          {pricePerManager}
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
              {{ peopleNumber: maxSliderNumber }}
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
              {totalPrice}
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
      <Button
        label={isNeedRequest ? t("SendRequest") : t("UpgradeNow")}
        size={"medium"}
        primary
        isDisabled={
          (!isNeedRequest && isAlreadyPaid && usersCount === countAdmin) ||
          isLoading ||
          isDisabled
        }
        onClick={onClick}
        isLoading={isLoading}
      />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { pricePerManager } = auth;
  const { theme } = auth.settingsStore;
  const { isLoading, totalPrice } = payments;
  return { theme, pricePerManager, totalPrice, isLoading };
})(observer(TotalTariffContainer));
