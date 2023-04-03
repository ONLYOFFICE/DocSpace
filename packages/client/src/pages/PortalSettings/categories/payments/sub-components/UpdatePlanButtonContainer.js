import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled from "styled-components";
import toastr from "@docspace/components/toast/toastr";
import DowngradePlanButtonContainer from "./DowngradePlanButtonContainer";
import api from "@docspace/common/api";
import { Trans } from "react-i18next";
import { updatePayment } from "@docspace/common/api/portal";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;
const MANAGER = "manager";
let timerId = null,
  intervalId = null,
  isWaitRequest = false,
  previousManagersCount = null;
const UpdatePlanButtonContainer = ({
  setIsLoading,
  paymentLink,
  isAlreadyPaid,
  managersCount,
  isDisabled,
  isLoading,
  maxCountManagersByQuota,
  setPortalQuotaValue,
  isLessCountThanAcceptable,
  currentTariffPlanTitle,
  t,
}) => {
  const updateMethod = async () => {
    try {
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

      const res = await updatePayment(managersCount);

      if (res === false) {
        toastr.error(t("ErrorNotification"));

        setIsLoading(false);
        clearTimeout(timerId);
        timerId = null;

        return;
      }

      previousManagersCount = maxCountManagersByQuota;
      waitingForQuota();
    } catch (e) {
      toastr.error(t("ErrorNotification"));
      setIsLoading(false);
      clearTimeout(timerId);
      timerId = null;
    }
  };

  const successToastr = () =>
    toastr.success(
      <Trans t={t} i18nKey="BusinessUpdated" ns="Payments">
        {{ planName: currentTariffPlanTitle }}
      </Trans>
    );

  const resetIntervalSuccess = () => {
    intervalId && successToastr();

    setIsLoading(false);
    clearInterval(intervalId);
    intervalId = null;
  };
  useEffect(() => {
    if (intervalId && maxCountManagersByQuota !== previousManagersCount) {
      resetIntervalSuccess();
      return;
    }
  }, [maxCountManagersByQuota]);
  const waitingForQuota = () => {
    isWaitRequest = false;

    intervalId = setInterval(async () => {
      try {
        if (isWaitRequest) {
          return;
        }

        isWaitRequest = true;
        const res = await api.portal.getPortalQuota();

        const managersObject = res.features.find((obj) => obj.id === MANAGER);

        if (managersObject && managersObject.value !== previousManagersCount) {
          setPortalQuotaValue(res);

          resetIntervalSuccess();
        }
      } catch (e) {
        setIsLoading(false);

        intervalId && toastr.error(e);
        clearInterval(intervalId);
        intervalId = null;
      }

      isWaitRequest = false;
    }, 2000);
  };

  const onUpdateTariff = () => {
    if (isAlreadyPaid) {
      updateMethod();
      return;
    }

    paymentLink
      ? window.open(paymentLink, "_blank")
      : toastr.error(t("ErrorNotification"));
  };

  useEffect(() => {
    return () => {
      timerId && clearTimeout(timerId);
      timerId = null;

      intervalId && clearInterval(intervalId);
      intervalId = null;
    };
  }, []);

  const switchPlanButton = (
    <Button
      label={t("UpgradeNow")}
      size={"medium"}
      primary
      isDisabled={isLessCountThanAcceptable || isLoading || isDisabled}
      onClick={onUpdateTariff}
      isLoading={isLoading}
    />
  );

  const updatingPlanButton = () => {
    const isDowngradePlan = managersCount < maxCountManagersByQuota;
    const isTheSameCount = managersCount === maxCountManagersByQuota;

    return isDowngradePlan ? (
      <DowngradePlanButtonContainer
        onUpdateTariff={onUpdateTariff}
        t={t}
        isDisabled={isDisabled}
      />
    ) : (
      <Button
        label={t("UpgradeNow")}
        size={"medium"}
        primary
        isDisabled={
          isLessCountThanAcceptable || isTheSameCount || isLoading || isDisabled
        }
        onClick={onUpdateTariff}
        isLoading={isLoading}
      />
    );
  };

  return (
    <StyledBody>
      {isAlreadyPaid ? updatingPlanButton() : switchPlanButton}
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { currentTariffStatusStore, currentQuotaStore } = auth;
  const {
    maxCountManagersByQuota,
    setPortalQuotaValue,
    currentTariffPlanTitle,
  } = currentQuotaStore;

  const { isNotPaidPeriod, isGracePeriod } = currentTariffStatusStore;

  const {
    setIsLoading,
    paymentLink,
    isNeedRequest,
    isLoading,
    managersCount,
    isLessCountThanAcceptable,
    accountLink,
    isAlreadyPaid,
  } = payments;

  return {
    isAlreadyPaid,
    setIsLoading,
    paymentLink,
    isNeedRequest,
    isLoading,
    managersCount,
    maxCountManagersByQuota,
    isLessCountThanAcceptable,
    isNotPaidPeriod,
    isGracePeriod,
    accountLink,
    setPortalQuotaValue,
    currentTariffPlanTitle,
  };
})(observer(UpdatePlanButtonContainer));
