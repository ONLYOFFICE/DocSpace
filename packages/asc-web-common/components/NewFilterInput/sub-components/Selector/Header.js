import React from 'react';

import IconButton from '@appserver/components/icon-button';
import Heading from '@appserver/components/heading';

import { StyledSelectorHeader } from './StyledSelector';

const Header = ({ label, onClick }) => {
  return (
    <StyledSelectorHeader>
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
        color="#A3A9AE"
        hoverColor="#657077"
        isFill={true}
        className="arrow-button"
        onClick={onClick}
      />
      <Heading size="medium">{label}</Heading>
    </StyledSelectorHeader>
  );
};

export default React.memo(Header);
