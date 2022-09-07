import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";
import toastr from "client/toastr";
import DowngradePlanButtonContainer from "./DowngradePlanButtonContainer";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

let timerId = null;
const UpdatePlanButtonContainer = ({
  updatePayment,
  setIsLoading,
  paymentLink,
  isAlreadyPaid,
  managersCount,
  isDisabled,
  isLoading,
  maxTariffManagers,
  setPortalPaymentsQuotas,
  setPortalQuota,
  isLessCountThanAcceptable,
  t,
}) => {
  const updateMethod = async () => {
    try {
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

      await updatePayment(managersCount);

      await Promise.all([setPortalQuota(), setPortalPaymentsQuotas()]);
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

  useEffect(() => {
    return () => {
      timerId && clearTimeout(timerId);
      timerId = null;
    };
  }, []);

  const switchPlanButton = () => {
    return (
      <Button
        label={t("UpgradeNow")}
        size={"medium"}
        primary
        isDisabled={isLessCountThanAcceptable || isLoading || isDisabled}
        onClick={onUpdateTariff}
        isLoading={isLoading}
      />
    );
  };

  const updatingPlanButton = () => {
    const isDowngradePlan = managersCount < maxTariffManagers;
    const isTheSameCount = managersCount === maxTariffManagers;

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
        isDisabled={isTheSameCount || isLoading || isDisabled}
        onClick={onUpdateTariff}
        isLoading={isLoading}
      />
    );
  };

  return (
    <StyledBody>
      {isAlreadyPaid ? updatingPlanButton() : switchPlanButton()}
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const {
    portalQuota,
    setPortalPaymentsQuotas,
    setPortalQuota,
    isNotPaid,
    isGracePeriod,
  } = auth;
  const { countAdmin: maxTariffManagers } = portalQuota;
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
    maxTariffManagers,
    setPortalPaymentsQuotas,
    setPortalQuota,
    isLessCountThanAcceptable,
    isNotPaid,
    isGracePeriod,
    accountLink,
  };
})(observer(UpdatePlanButtonContainer));
