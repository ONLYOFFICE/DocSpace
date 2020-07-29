import React from 'react';
import PropTypes from "prop-types";
import styled from 'styled-components';

import { Box, Button, utils } from 'asc-web-components';

const { tablet } = utils.device;

const StyledContainer = styled(Box)`
  width: 311px;
  margin: 32px auto 0 auto;

  @media ${tablet} {
    width: 100%;
  }
`;

const ButtonContainer = ({ t, sendingComplete, onContinueHandler }) => {
  return (
    <StyledContainer className="wizard-button">
      <Button
        size="large"
        scale={true}
        primary
        isLoading={sendingComplete ? true : false}
        label={t('buttonContinue')}           
        onClick={onContinueHandler}
        isDisabled={sendingComplete}
      />
    </StyledContainer>
  );
}

ButtonContainer.propTypes = {
  t: PropTypes.func.isRequired,
  sendingComplete: PropTypes.bool.isRequired,
  onContinueHandler: PropTypes.func.isRequired
}

export default ButtonContainer;