import React from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";
import RequestButtonContainer from "./RequestButtonContainer";
import UpdatePlanButtonContainer from "./UpdatePlanButtonContainer";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

const ButtonContainer = ({
  isNeedRequest,
  isAlreadyPaid,
  isDisabled,
  isLoading,
  t,
  isNotPaid,
  isGracePeriod,
  accountLink,
}) => {
  const goToStripeAccount = () => {
    if (accountLink) window.open(accountLink, "_blank");
  };

  return (
    <StyledBody>
      {isNotPaid || isGracePeriod ? (
        <Button
          label={t("Pay")}
          size={"medium"}
          primary
          isDisabled={isLoading || isDisabled}
          onClick={goToStripeAccount}
          isLoading={isLoading}
        />
      ) : isNeedRequest ? (
        <RequestButtonContainer
          isLoading={isLoading}
          isDisabled={isDisabled}
          t={t}
        />
      ) : (
        <UpdatePlanButtonContainer isAlreadyPaid={isAlreadyPaid} t={t} />
      )}
    </StyledBody>
  );
};

export default inject(({ auth, payments }) => {
  const { isNotPaid, isGracePeriod } = auth;
  const { isNeedRequest, isLoading, accountLink } = payments;

  return {
    isNeedRequest,
    isLoading,
    isNotPaid,
    isGracePeriod,
    accountLink,
  };
})(observer(ButtonContainer));
