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
import SelectTotalSizeContainer from "./SelectTotalSizeContainer";

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
    margin-bottom: 4px;
    text-align: center;
  }
`;

const SelectUsersCountContainer = ({
  managersCount,
  setShoppingLink,
  theme,
  isDisabled,
  isLoading,
  minAvailableManagersValue,
  isAlreadyPaid,
  maxAvailableManagersCount,
  setManagersCount,
  setTotalPrice,
  isLessCountThanAcceptable,
  step,
}) => {
  const { t } = useTranslation("Payments");

  const onSliderChange = (e) => {
    const count = parseFloat(e.target.value);
    if (count > minAvailableManagersValue) {
      setShoppingLink(count);
      setManagersCount(count);
      setTotalPrice(count);
    } else {
      setShoppingLink(minAvailableManagersValue);
      setManagersCount(minAvailableManagersValue);
      setTotalPrice(minAvailableManagersValue);
    }
  };

  const onClickOperations = (e) => {
    const operation = e.currentTarget.dataset.operation;

    let value = +managersCount;

    if (operation === "plus") {
      if (managersCount <= maxAvailableManagersCount) {
        value += step;
      }
    }
    if (operation === "minus") {
      if (managersCount > maxAvailableManagersCount) {
        value = maxAvailableManagersCount;
      } else {
        if (managersCount > minAvailableManagersValue) {
          value -= step;
        }
      }
    }

    if (value !== +managersCount) {
      setShoppingLink(value);
      setManagersCount(value);
      setTotalPrice(value);
    }
  };
  const onChangeNumber = (e) => {
    const { target } = e;
    let value = target.value;

    if (managersCount > maxAvailableManagersCount) {
      value = value.slice(0, -1);
    }

    const numberValue = +value;

    if (isNaN(numberValue)) return;

    if (numberValue === 0) {
      setManagersCount(minAvailableManagersValue);
      return;
    }

    setShoppingLink(numberValue);
    setManagersCount(numberValue);
    setTotalPrice(numberValue);
  };

  const isNeedPlusSign = managersCount > maxAvailableManagersCount;

  const value = isNeedPlusSign
    ? maxAvailableManagersCount + "+"
    : managersCount + "";

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
      <SelectTotalSizeContainer isNeedPlusSign={isNeedPlusSign} />
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
        min={minAvailableManagersValue}
        max={(maxAvailableManagersCount + 1).toString()}
        step={step}
        withPouring
        value={
          isLessCountThanAcceptable ? minAvailableManagersValue : managersCount
        }
        {...onChangeSlideProp}
      />
      <div className="slider-track">
        <Text className="slider-track-value_min" noSelect>
          {minAvailableManagersValue}
        </Text>
        <Text className="slider-track-value_max" noSelect>
          {maxAvailableManagersCount + "+"}
        </Text>
      </div>
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { theme } = auth.settingsStore;
  const {
    isLoading,
    minAvailableManagersValue,
    managersCount,
    maxAvailableManagersCount,
    setManagersCount,
    setTotalPrice,
    isLessCountThanAcceptable,
    stepByQuotaForManager,
  } = payments;

  const step = stepByQuotaForManager;

  return {
    theme,
    isLoading,
    minAvailableManagersValue,
    managersCount,
    maxAvailableManagersCount,
    setManagersCount,
    setTotalPrice,
    isLessCountThanAcceptable,
    step,
  };
})(observer(SelectUsersCountContainer));
