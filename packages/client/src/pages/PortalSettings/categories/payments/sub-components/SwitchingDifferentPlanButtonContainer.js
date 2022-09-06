import React from "react";
import Button from "@docspace/components/button";
import styled, { css } from "styled-components";

const StyledBody = styled.div`
  button {
    width: 100%;
  }
`;

const SwitchingDifferentPlanButtonContainer = ({
  isDisabled,
  isLoading,
  isLessCountThanAcceptable,
  t,
  onUpdateTariff,
}) => {
  return (
    <StyledBody>
      <Button
        label={t("UpgradeNow")}
        size={"medium"}
        primary
        isDisabled={isLessCountThanAcceptable || isLoading || isDisabled}
        onClick={onUpdateTariff}
        isLoading={isLoading}
      />
    </StyledBody>
  );
};

export default SwitchingDifferentPlanButtonContainer;
