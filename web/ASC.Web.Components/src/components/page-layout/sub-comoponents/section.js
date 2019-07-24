import React from 'react'
import styled from 'styled-components'

const StyledSection = styled.section`
  padding: 0 16px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden auto;
`;

const Section = (props) => {
  //console.log("Section render");
  return  (<StyledSection {...props} />);
};

export default Section;