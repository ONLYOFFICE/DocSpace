import React from "react";
import styled, { css } from "styled-components";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import Slider from "@docspace/components/slider";
import PlusIcon from "../../../../../../public/images/plus.react.svg";
import MinusIcon from "../../../../../../public/images/minus.react.svg";
import { smallTablet } from "@docspace/components/utils/device";
import TextInput from "@docspace/components/text-input";
import { inject, observer } from "mobx-react";

const StyledBody = styled.div`
  max-width: 272px;
  margin: 0 auto;

  @media ${smallTablet} {
    max-width: 520px;
  }

  .slider-track {
    display: flex;
    position: relative;
    margin-top: -10px;
    height: 16px;

    .slider-track-value_min,
    .slider-track-value_max {
      color: ${(props) =>
        props.theme.avatarEditorBody.slider.trackNumber.color};
    }

    .slider-track-value_max {
      position: absolute;
      right: 0;
    }
    .slider-track-value_min {
      position: absolute;
      left: 0;
    }
  }

  .payment-operations_input {
    width: 111px;
    font-size: 44px;
    text-align: center;
    margin-left: 20px;
    margin-right: 20px;
    ${(props) =>
      props.isDisabled &&
      css`
        color: ${props.theme.text.disableColor};
      `}
  }

  .payment-users {
    display: flex;
    align-items: center;
    margin: 0 auto;
    width: max-content;
    .payment-score {
      path {
        ${(props) =>
          props.isDisabled &&
          css`
            fill: ${props.theme.text.disableColor};
          `}
      }
    }

    .payment-score,
    .circle {
      cursor: ${(props) => (props.isDisabled ? "default" : "pointer")};
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
  .payment-users_count {
    margin-left: 20px;
    margin-right: 20px;
    text-align: center;
    width: 102px;
  }

  .payment-users_text {
    margin-bottom: 12px;
    text-align: center;
  }
`;

const min = 0;
const SelectUsersCountContainer = ({
  maxUsersCount,
  step,
  maxSliderNumber,
  usersCount,
  onSliderChange,
  onClickOperations,
  onChangeNumber,
  theme,
  isDisabled,
  isAlreadyPaid,
  isLoading,
}) => {
  const { t } = useTranslation("Payments");

  const value =
    usersCount >= maxUsersCount ? maxSliderNumber + "+" : usersCount + "";

  const isUpdatingTariff = isLoading && isAlreadyPaid;

  const onClickProp =
    isDisabled || isUpdatingTariff ? {} : { onClick: onClickOperations };
  const onChangeSlideProp =
    isDisabled || isUpdatingTariff ? {} : { onChange: onSliderChange };
  const onchangeNumberProp =
    isDisabled || isUpdatingTariff ? {} : { onChange: onChangeNumber };

  const color = isDisabled ? { color: theme.text.disableColor } : {};

  return (
    <StyledBody theme={theme} isDisabled={isDisabled || isUpdatingTariff}>
      <Text noSelect fontWeight={600} className="payment-users_text" {...color}>
        {t("ManagersNumber")}
      </Text>
      <div className="payment-users">
        <div className="circle" {...onClickProp} data-operation={"minus"}>
          <MinusIcon {...onClickProp} className="payment-score" />
        </div>

        <TextInput
          isReadOnly={isDisabled}
          withBorder={false}
          className="payment-operations_input"
          value={value}
          {...onchangeNumberProp}
        />
        <div className="circle" {...onClickProp} data-operation={"plus"}>
          <PlusIcon {...onClickProp} className="payment-score" />
        </div>
      </div>

      <Slider
        isDisabled={isDisabled || isUpdatingTariff}
        isReadOnly={isDisabled || isUpdatingTariff}
        type="range"
        min={min}
        max={maxUsersCount.toString()}
        step={step}
        withPouring
        value={usersCount}
        {...onChangeSlideProp}
      />
      <div className="slider-track">
        <Text className="slider-track-value_min">{min}</Text>
        <Text className="slider-track-value_max">{maxSliderNumber + "+"}</Text>
      </div>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { theme } = auth.settingsStore;
  const { isLoading } = payments;
  return { theme, isLoading };
})(observer(SelectUsersCountContainer));
