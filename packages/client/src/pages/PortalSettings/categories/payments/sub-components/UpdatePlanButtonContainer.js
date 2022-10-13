import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled from "styled-components";
import toastr from "@docspace/components/toast/toastr";
import DowngradePlanButtonContainer from "./DowngradePlanButtonContainer";
import api from "@docspace/common/api";
import { Trans } from "react-i18next";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;
const MANAGER = "manager";
let timerId = null,
  intervalId = null,
  isWaitRequest = false;
const UpdatePlanButtonContainer = ({
  updatePayment,
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

      await updatePayment(managersCount);
      waitingForQuota();
    } catch (e) {
      toastr.error(e);
      setIsLoading(false);
      clearTimeout(timerId);
      timerId = null;
    }
  };

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

        if (managersObject && managersObject.value === managersCount) {
          setPortalQuotaValue(res);
          intervalId &&
            toastr.success(
              <Trans t={t} i18nKey="BusinessUpdated" ns="Payments">
                {{ planName: currentTariffPlanTitle }}
              </Trans>
            );

          setIsLoading(false);
          clearInterval(intervalId);
          intervalId = null;
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

    if (paymentLink) window.open(paymentLink, "_blank");
  };

  useEffect(() => {
    return () => {
      timerId && clearTimeout(timerId);
      timerId = null;
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
    updatePayment,
    setIsLoading,
    paymentLink,
    isNeedRequest,
    isLoading,
    managersCount,
    isLessCountThanAcceptable,
    accountLink,
  } = payments;

  return {
    updatePayment,
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
