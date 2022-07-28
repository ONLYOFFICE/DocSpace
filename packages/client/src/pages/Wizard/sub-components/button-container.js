import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import Box from "@docspace/components/box";
import Button from "@docspace/components/button";
import { tablet } from "@docspace/components/utils/device";

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 0 auto;
  margin-top: 1px;

  @media ${tablet} {
    width: 100%;
  }
`;

const ButtonContainer = ({ t, sending, onContinueHandler }) => {
  return (
    <StyledContainer>
      <Button
        size="medium"
        scale={true}
        primary
        isDisabled={sending}
        isLoading={sending ? true : false}
        label={t("Common:ContinueButton")}
        onClick={onContinueHandler}
      />
    </StyledContainer>
  );
};

ButtonContainer.propTypes = {
  t: PropTypes.func.isRequired,
  sending: PropTypes.bool.isRequired,
  onContinueHandler: PropTypes.func.isRequired,
};

export default ButtonContainer;
