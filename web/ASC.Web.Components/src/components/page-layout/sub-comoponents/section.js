import React from 'react'
import styled from 'styled-components'

const StyledSection = styled.section`
  padding: 0 16px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden auto;
`;

class Section extends React.Component {
  /*shouldComponentUpdate() {
    return false;
  }*/
  render() {
    console.log("PageLayout Section render");
    return (<StyledSection {...this.props} />);
  };
};

export default Section;