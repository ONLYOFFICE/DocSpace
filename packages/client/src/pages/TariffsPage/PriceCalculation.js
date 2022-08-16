import React, { useState } from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";

import SelectUsersCountContainer from "./sub-components/SelectUsersCountContainer";
import TotalTariffContainer from "./sub-components/TotalTariffContainer";
import { smallTablet } from "@docspace/components/utils/device";

const StyledBody = styled.div`
  border-radius: 12px;
  border: 1px solid #d0d5da;
  max-width: 320px;

  @media ${smallTablet} {
    max-width: 600px;
  }

  padding: 24px;
  box-sizing: border-box;
  p {
    margin-bottom: 24px;
  }
`;

const step = 1,
  minUsersCount = 1,
  maxUsersCount = 1000,
  maxSliderNumber = 999;

const PriceCalculation = ({ t, price }) => {
  const [usersCount, setUsersCount] = useState(minUsersCount);

  const onSliderChange = (e) => {
    const count = parseFloat(e.target.value);
    count > minUsersCount ? setUsersCount(count) : setUsersCount(minUsersCount);
  };

  const onClickOperations = (e) => {
    const operation = e.currentTarget.dataset.operation;

    let value = +usersCount;

    if (operation === "plus") {
      if (usersCount < maxUsersCount) {
        value += step;
      }
    }
    if (operation === "minus") {
      if (usersCount >= maxUsersCount) {
        value = maxSliderNumber;
      } else {
        if (usersCount > minUsersCount) {
          value -= step;
        }
      }
    }

    value !== +usersCount && setUsersCount(value);
  };
  const onChangeNumber = (e) => {
    const { target } = e;
    let value = target.value;

    if (usersCount >= maxUsersCount) {
      value = value.slice(0, -1);
    }

    const numberValue = +value;

    if (isNaN(numberValue)) return;

    if (numberValue === 0) {
      setUsersCount(minUsersCount);
      return;
    }

    setUsersCount(numberValue);
  };

  return (
    <StyledBody>
      <Text fontSize="16px" fontWeight={600} noSelect>
        {t("PriceCalculation")}
      </Text>
      <SelectUsersCountContainer
        maxUsersCount={maxUsersCount}
        maxSliderNumber={maxSliderNumber}
        step={step}
        usersCount={usersCount}
        onClickOperations={onClickOperations}
        onSliderChange={onSliderChange}
        onChangeNumber={onChangeNumber}
      />
      <TotalTariffContainer t={t} usersCount={usersCount} price={price} />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo } = payments;

  return { tariffsInfo };
})(observer(PriceCalculation));
