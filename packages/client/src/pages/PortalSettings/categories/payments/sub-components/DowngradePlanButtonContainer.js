import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";
import styled from "styled-components";
import ChangePricingPlanDialog from "../../../../../components/dialogs/ChangePricingPlanDialog";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

const DowngradePlanButtonContainer = ({
  isDisabled,
  isLoading,
  isLessCountThanAcceptable,
  isAlreadyPaid,
  onUpdateTariff,
  canDowngradeTariff,
  canPayTariff,
  buttonLabel,
}) => {
  const [
    isVisibleDowngradePlanDialog,
    setIsVisibleDowngradePlanDialog,
  ] = useState(false);

  const isPassedByQuota = () => {
    return isAlreadyPaid ? canDowngradeTariff : canPayTariff;
  };
  const onDowngradeTariff = () => {
    if (isPassedByQuota()) {
      onUpdateTariff && onUpdateTariff();
      return;
    }

    setIsVisibleDowngradePlanDialog(true);
  };

  const onClose = () => {
    isVisibleDowngradePlanDialog && setIsVisibleDowngradePlanDialog(false);
  };

  return (
    <StyledBody>
      {isVisibleDowngradePlanDialog && (
        <ChangePricingPlanDialog
          visible={isVisibleDowngradePlanDialog}
          onClose={onClose}
        />
      )}
      <Button
        label={buttonLabel}
        size={"medium"}
        primary
        isDisabled={isLessCountThanAcceptable || isLoading || isDisabled}
        onClick={onDowngradeTariff}
        isLoading={isLoading}
      />
    </StyledBody>
  );
};

export default inject(({ payments }) => {
  const {
    isLoading,
    isLessCountThanAcceptable,
    canDowngradeTariff,
    canPayTariff,

    isAlreadyPaid,
  } = payments;

  return {
    isLoading,
    isLessCountThanAcceptable,
    canDowngradeTariff,
    canPayTariff,
    isAlreadyPaid,
  };
})(observer(DowngradePlanButtonContainer));
