import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { Box, Button, utils } from "asc-web-components";

const { tablet } = utils.device;

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
        size="large"
        scale={true}
        primary
        isDisabled={sending}
        isLoading={sending ? true : false}
        label={t("ButtonContinue")}
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
