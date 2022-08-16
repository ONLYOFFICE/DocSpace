import React, { useState } from "react";
import styled from "styled-components";
import { useTranslation, Trans } from "react-i18next";
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

const step = "1",
  minUsersCount = 1,
  maxUsersCount = 1000;

const PriceCalculation = ({ t, price }) => {
  const [usersCount, setUsersCount] = useState(minUsersCount);

  const onSliderChange = (e) => {
    const count = parseFloat(e.target.value);
    count > minUsersCount ? setUsersCount(count) : setUsersCount(minUsersCount);
  };

  const onPlusClick = () => {
    usersCount < maxUsersCount && setUsersCount(usersCount + 1);
  };

  const onMinusClick = () => {
    usersCount > minUsersCount && setUsersCount(usersCount - 1);
  };

  return (
    <StyledBody>
      <Text fontSize="16px" fontWeight={600} noSelect>
        {t("PriceCalculation")}
      </Text>
      <SelectUsersCountContainer
        maxUsersCount={maxUsersCount}
        step={step}
        usersCount={usersCount}
        onMinusClick={onMinusClick}
        onPlusClick={onPlusClick}
        onSliderChange={onSliderChange}
      />
      <TotalTariffContainer t={t} usersCount={usersCount} price={price} />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { tariffsInfo } = payments;

  return { tariffsInfo };
})(observer(PriceCalculation));
