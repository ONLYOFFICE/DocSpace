import React from 'react'
import styled from 'styled-components'
import Scrollbar from '../../scrollbar'

const StyledSectionBody = styled.div`
  margin: 16px 0;
  outline: 1px dotted;
  flex-grow: 1;
`;

const SectionBody = React.memo(props => { 
  console.log("PageLayout SectionBody render");
  const { children } = props;

  return (
    <StyledSectionBody>
      <Scrollbar stype="mediumBlack">
        {children}
      </Scrollbar>
    </StyledSectionBody>
  );
});

export default SectionBody;