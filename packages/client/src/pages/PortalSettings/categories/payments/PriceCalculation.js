import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";

import SelectUsersCountContainer from "./sub-components/SelectUsersCountContainer";
import TotalTariffContainer from "./sub-components/TotalTariffContainer";
import { smallTablet } from "@docspace/components/utils/device";
import toastr from "client/toastr";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import axios from "axios";
import { combineUrl } from "@docspace/common/utils";
import api from "@docspace/common/api";

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

let timeout = null,
  timerId = null,
  CancelToken,
  source;

const PriceCalculation = ({
  t,
  price,
  rights,
  theme,
  setPaymentLink,
  portalQuota,
  paymentLink,
  setIsLoading,
  updatePayment,
}) => {
  const { trial, free, countAdmin } = portalQuota;

  const isAlreadyPaid = !trial && !free;
  const initialUsersCount = isAlreadyPaid ? countAdmin : minUsersCount;

  const [usersCount, setUsersCount] = useState(initialUsersCount);

  const setStartLink = async () => {
    const link = await api.portal.getPaymentLink(initialUsersCount);
    setPaymentLink(link);
  };

  useEffect(() => {
    setStartLink();
    return () => {
      timerId && clearTimeout(timerId);
      timerId = null;

      timeout && clearTimeout(timeout);
      timeout = null;
    };
  }, []);

  const onSliderChange = (e) => {
    const count = parseFloat(e.target.value);
    if (count > minUsersCount) {
      setShoppingLink(count);
      setUsersCount(count);
    } else {
      setShoppingLink(minUsersCount);
      setUsersCount(minUsersCount);
    }
  };

  const setShoppingLink = (value) => {
    if (isAlreadyPaid || value > maxSliderNumber) {
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
          }
          return;
        });
    }, 1000);
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

    if (value !== +usersCount) {
      setShoppingLink(value);
      setUsersCount(value);
    }
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

    setShoppingLink(numberValue);
    setUsersCount(numberValue);
  };

  const updateMethod = async () => {
    try {
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

      await updatePayment(usersCount);
      toastr.success("the changes will be applied soon");
    } catch (e) {
      toastr.error(e);
    }

    setIsLoading(false);
    clearTimeout(timerId);
    timerId = null;
  };

  const onUpdateTariff = () => {
    if (isAlreadyPaid) {
      updateMethod();
      return;
    }

    if (paymentLink) window.open(paymentLink, "_blank");
  };

  const isDisabled = rights === "3" || rights === "2" ? true : false;

  const color = isDisabled ? { color: theme.text.disableColor } : {};

  return (
    <StyledBody rights={rights}>
      <Text fontSize="16px" fontWeight={600} noSelect {...color}>
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
        isDisabled={isDisabled}
        isAlreadyPaid={isAlreadyPaid}
      />
      <TotalTariffContainer
        maxUsersCount={maxUsersCount}
        maxSliderNumber={maxSliderNumber}
        t={t}
        usersCount={usersCount}
        price={price}
        isDisabled={isDisabled}
        onClick={onUpdateTariff}
        isAlreadyPaid={isAlreadyPaid}
        countAdmin={countAdmin}
      />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const {
    tariffsInfo,
    setPaymentLink,
    paymentLink,
    setIsLoading,
    updatePayment,
  } = payments;
  const { theme } = auth.settingsStore;
  const { portalQuota } = auth;
  //const rights = "2";
  //const rights = "3";
  const rights = "1";
  return {
    tariffsInfo,
    rights,
    theme,
    setPaymentLink,
    portalQuota,
    paymentLink,
    setIsLoading,
    updatePayment,
  };
})(observer(PriceCalculation));
