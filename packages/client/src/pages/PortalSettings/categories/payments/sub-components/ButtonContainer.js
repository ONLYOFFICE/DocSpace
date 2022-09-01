import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";
import toastr from "client/toastr";
import SalesDepartmentRequestDialog from "../../../../../../src/components/dialogs/SalesDepartmentRequestDialog";

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
  maxTariffManagers,
  setPortalQuota,
  isLessCountThanAcceptable,
  t,
  isNotPaid,
  isGracePeriod,
  accountLink,
}) => {
  const [isVisibleDialog, setIsVisibleDialog] = useState(false);

  const updateMethod = async () => {
    try {
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

      await updatePayment(managersCount);
      await setPortalQuota();
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

  const goToStripeAccount = () => {
    if (accountLink) window.open(accountLink, "_blank");
  };

  const toDoRequest = () => {
    setIsVisibleDialog(true);
  };

  const onClose = () => {
    setIsVisibleDialog(false);
  };
  useEffect(() => {
    return () => {
      timerId && clearTimeout(timerId);
      timerId = null;
    };
  }, []);

  const isTheSameCount = managersCount === maxTariffManagers;

  return (
    <StyledBody>
      {isVisibleDialog && (
        <SalesDepartmentRequestDialog
          visible={isVisibleDialog}
          onClose={onClose}
        />
      )}
      {isNotPaid || isGracePeriod ? (
        <Button
          label={t("Pay")}
          size={"medium"}
          primary
          isDisabled={isLoading || isDisabled}
          onClick={goToStripeAccount}
          isLoading={isLoading}
        />
      ) : (
        <Button
          label={isNeedRequest ? t("SendRequest") : t("UpgradeNow")}
          size={"medium"}
          primary
          isDisabled={
            (!isNeedRequest && isAlreadyPaid && isTheSameCount) ||
            isLessCountThanAcceptable ||
            isLoading ||
            isDisabled
          }
          onClick={isNeedRequest ? toDoRequest : onUpdateTariff}
          isLoading={isLoading}
        />
      )}
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { portalQuota, setPortalQuota, isNotPaid, isGracePeriod } = auth;
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
    setPortalQuota,
    isLessCountThanAcceptable,
    isNotPaid,
    isGracePeriod,
    accountLink,
  };
})(observer(ButtonContainer));
