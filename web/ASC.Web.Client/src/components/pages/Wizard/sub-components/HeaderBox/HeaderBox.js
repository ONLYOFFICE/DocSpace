import React from 'react';

import StyledHeaderBox from './StyledHeaderBox';

const HeaderBox = () => (
  <StyledHeaderBox>
    <div className="wizard-header">
      <a className="wizard-logo" href="/home">
        <img
          src="images/onlyoffice_logo/light_small_general.svg"
          alt="Logo"
        />
      </a>
    </div>
  </StyledHeaderBox>
);

export default HeaderBox;