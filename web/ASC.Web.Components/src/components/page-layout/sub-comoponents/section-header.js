import React from 'react'
import styled from 'styled-components'

const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #ECEEF1;
  height: 56px;
`;

const SectionHeader = React.memo(props => {
  //console.log("PageLayout SectionHeader render");
  return (<StyledSectionHeader {...props} />);
});

export default SectionHeader;