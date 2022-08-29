import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";
import toastr from "client/toastr";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

let timerId = null;
const ButtonContainer = ({
  updatePayment,
  setIsLoading,
  paymentLink,
  isNeedRequest,
  isAlreadyPaid,
  managersCount,
  isDisabled,
  isLoading,
  countAdmin,
  t,
}) => {
  const updateMethod = async () => {
    try {
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

      await updatePayment(managersCount);
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
  useEffect(() => {
    return () => {
      timerId && clearTimeout(timerId);
      timerId = null;
    };
  }, []);

  const isTheSameCount = managersCount === countAdmin;

  return (
    <StyledBody>
      <Button
        label={isNeedRequest ? t("SendRequest") : t("UpgradeNow")}
        size={"medium"}
        primary
        isDisabled={
          (!isNeedRequest && isAlreadyPaid && isTheSameCount) ||
          isLoading ||
          isDisabled
        }
        onClick={onUpdateTariff}
        isLoading={isLoading}
      />
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { portalQuota } = auth;
  const { countAdmin } = portalQuota;
  const {
    updatePayment,
    setIsLoading,
    paymentLink,
    isNeedRequest,
    isLoading,
    managersCount,
  } = payments;

  return {
    updatePayment,
    setIsLoading,
    paymentLink,
    isNeedRequest,
    isLoading,
    managersCount,
    countAdmin,
  };
})(observer(ButtonContainer));
