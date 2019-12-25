import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
const { tablet } = utils.device;

const StyledSection = styled.section`
  padding: 0 0 0 24px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;

  @media ${tablet} {
    padding: 0 0 0 16px;
  }
`;

class Section extends React.Component {
  /*shouldComponentUpdate() {
    return false;
  }*/
  render() {
    //console.log("PageLayout Section render");
    return <StyledSection {...this.props} />;
  }
}

export default Section;
