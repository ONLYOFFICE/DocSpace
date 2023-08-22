import React from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled from "styled-components";
import RequestButtonContainer from "./RequestButtonContainer";
import UpdatePlanButtonContainer from "./UpdatePlanButtonContainer";
import toastr from "@docspace/components/toast/toastr";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

const ButtonContainer = ({
  isNeedRequest,
  isDisabled,
  isLoading,
  t,
  isNotPaidPeriod,
  isGracePeriod,
  accountLink,
  isFreeAfterPaidPeriod,
}) => {
  const goToStripeAccount = () => {
    accountLink
      ? window.open(accountLink, "_blank")
      : toastr.error(t("ErrorNotification"));
  };

  return (
    <StyledBody>
      {isNotPaidPeriod || isGracePeriod || isFreeAfterPaidPeriod ? (
        <Button
          className="pay-button"
          label={t("Pay")}
          size={"medium"}
          primary
          isDisabled={isLoading || isDisabled}
          onClick={goToStripeAccount}
          isLoading={isLoading}
        />
      ) : isNeedRequest ? (
        <RequestButtonContainer isDisabled={isDisabled} t={t} />
      ) : (
        <UpdatePlanButtonContainer t={t} isDisabled={isDisabled} />
      )}
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { currentTariffStatusStore } = auth;
  const { isNeedRequest, isLoading, accountLink } = payments;
  const { isNotPaidPeriod, isGracePeriod } = currentTariffStatusStore;

  return {
    isNeedRequest,
    isLoading,
    isNotPaidPeriod,
    isGracePeriod,
    accountLink,
  };
})(observer(ButtonContainer));
