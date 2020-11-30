import React from "react";
import styled, { css } from "styled-components";
import { utils } from "asc-web-components";
const { tablet } = utils.device;

const tabletProps = css`
  .section-header_filter {
    display: none;
  }

  .section-body_filter {
    display: block;
    margin: 0 0 25px;
  }
`;

const StyledSection = styled.section`
  padding: 0 0 0 24px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  /*width: ${(props) => `${props.widthProp}px`};*/
  .layout-progress-bar {
    bottom: 0;
    position: sticky;
    margin-left: -24px;

    @media ${tablet} {
      margin-left: -16px;
    }
  }

  .section-header_filter {
    display: block;
  }

  .section-body_filter {
    display: none;
  }
  @media ${tablet} {
    padding: 0 0 0 16px;
    ${tabletProps};
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
