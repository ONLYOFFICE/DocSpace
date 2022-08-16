import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
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

  .total-tariff_user {
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
  .total-tariff_price {
    display: flex;
    justify-content: center;
    .total-tariff_price-text {
      margin-bottom: 10px;
      border-bottom: 1px solid #eceef1;
    }
    .total-tariff_month-text {
      margin: auto 0;
    }
  }

  button {
    width: 100%;
  }
`;

const TotalTariffContainer = ({ t, usersCount, price, isDisabled, theme }) => {
  useEffect(() => {}, []);

  const totalPrice = price * usersCount;

  const color = isDisabled ? { color: theme.text.disableColor } : {};

  return (
    <StyledBody>
      <div className="total-tariff_user">
        <Text fontSize="16px" textAlign="center" isBold noSelect {...color}>
          {price}
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
      <div className="total-tariff_price">
        <Text
          fontSize="48px"
          isBold
          textAlign={"center"}
          className="total-tariff_price-text"
          noSelect
          {...color}
        >
          {totalPrice}
        </Text>
        <Text
          fontSize="16px"
          isBold
          textAlign={"center"}
          className="total-tariff_month-text"
          noSelect
          {...color}
        >
          {"/month"}
        </Text>
      </div>
      <Button
        label={t("UpgradeNow")}
        size={"medium"}
        primary
        isDisabled={isDisabled}
      />
    </StyledBody>
  );
};

export default inject(({ auth }) => {
  const { theme } = auth.settingsStore;
  return { theme };
})(observer(TotalTariffContainer));
