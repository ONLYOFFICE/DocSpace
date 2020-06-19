import React from 'react';

import StyledSettingsWrapper from './StyledSettingsWrapper';

const SettingsWrapper = ({
  children
}) => (
  <StyledSettingsWrapper>
    { children }
  </StyledSettingsWrapper>
);

export default SettingsWrapper;