import React from 'react';

import { Button } from 'asc-web-components';

import StyledButtonBox from './StyledButtonBox';

const ButtonBox = ({ label }) => (
  <StyledButtonBox>
    <Button
      id="continue-button"
      className="wizard-button"
      primary
      label={label}
      tabIndex={3}
      size="big"
      onClick={() => alert('click')}
    />
  </StyledButtonBox>
);

export default ButtonBox;