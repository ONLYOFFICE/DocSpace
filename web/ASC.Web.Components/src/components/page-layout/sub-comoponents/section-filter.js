import React from 'react'
import styled from 'styled-components'

const StyledSectionFilter = styled.div`
  margin: 16px 0 0;
`;

const SectionFilter = (props) => { 
  //console.log("SectionFilter render");
  return (<StyledSectionFilter {...props}/>); 
}

export default SectionFilter;