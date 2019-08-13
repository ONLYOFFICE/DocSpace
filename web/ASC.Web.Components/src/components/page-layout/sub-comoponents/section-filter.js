import React from 'react'
import styled from 'styled-components'

const StyledSectionFilter = styled.div`
  margin: 16px 0 0;
`;

const SectionFilter = React.memo(props => { 
  //console.log("PageLayout SectionFilter render");
  return (<StyledSectionFilter {...props}/>); 
});

export default SectionFilter;