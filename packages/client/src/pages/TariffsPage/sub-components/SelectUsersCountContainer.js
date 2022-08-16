import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Slider from "@docspace/components/slider";
import PlusIcon from "../../../../public/images/plus.react.svg";
import MinusIcon from "../../../../public/images/minus.react.svg";
import { smallTablet } from "@docspace/components/utils/device";
const StyledBody = styled.div`
  max-width: 272px;
  margin: 0 auto;

  @media ${smallTablet} {
    max-width: 520px;
  }

  .tariff-users {
    display: flex;
    align-items: center;
    margin: 0 auto;
    width: max-content;
    .tariff-score,
    .circle {
      cursor: pointer;
    }
    .circle {
      background: #f3f4f4;
      display: flex;
      border: 1px solid #f3f4f4;
      border-radius: 50%;
      width: 40px;
      height: 40px;
      justify-content: center;
      -ms-align-items: center;
      align-items: center;
    }
  }
  .tariff-users_count {
    margin-left: 20px;
    margin-right: 20px;
    text-align: center;
    width: 102px;
  }

  .tariff-users_text {
    margin-bottom: 12px;
    text-align: center;
  }
`;

const SelectUsersCountContainer = ({
  maxUsersCount,
  step,
  usersCount,
  onMinusClick,
  onPlusClick,
  onSliderChange,
}) => {
  const { t } = useTranslation("Payments");

  return (
    <StyledBody>
      <Text noSelect fontWeight={600} className="tariff-users_text">
        {t("ManagersNumber")}
      </Text>
      <div className="tariff-users">
        <div className="circle" onClick={onMinusClick}>
          <MinusIcon onClick={onMinusClick} className="tariff-score" />
        </div>
        <Text noSelect fontSize={"44px"} className="tariff-users_count" isBold>
          {usersCount}
        </Text>
        <div className="circle" onClick={onPlusClick}>
          <PlusIcon onClick={onPlusClick} className="tariff-score" />
        </div>
      </div>

      <Slider
        type="range"
        min={"0"}
        max={maxUsersCount.toString()}
        step={step}
        withPouring
        value={usersCount}
        onChange={onSliderChange}
        colorPouring={"#20D21F"}
      />
    </StyledBody>
  );
};

export default SelectUsersCountContainer;
