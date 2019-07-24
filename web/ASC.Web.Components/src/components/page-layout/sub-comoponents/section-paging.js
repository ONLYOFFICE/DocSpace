import React from 'react'
import styled from 'styled-components'

const StyledSectionPaging = styled.div`
  margin: 0 0 16px;
`;

const SectionPaging = (props) => {
  //console.log("SectionPaging render");
  return (<StyledSectionPaging {...props} />);
}

export default SectionPaging;