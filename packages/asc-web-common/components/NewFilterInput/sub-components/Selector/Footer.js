import React from 'react';

import Button from '@appserver/components/button';

import { StyledSelectorFooter } from './StyledSelector';

const Footer = ({ count, onClick }) => {
  return (
    <StyledSelectorFooter>
      <Button
        size="large"
        primary={true}
        label={`Add users ${count > 0 ? `(${count})` : ''}`}
        scale={true}
        onClick={onClick}
      />
    </StyledSelectorFooter>
  );
};

export default React.memo(Footer);
