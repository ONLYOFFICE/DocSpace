import React from "react";
import styled from "styled-components";
import { utils } from "asc-web-components";
const { tablet } = utils.device;

const StyledSection = styled.section`
  padding: 0 0 0 24px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;

  .layout-progress-bar {
    position: sticky;
    margin-left: -16px;
  }

  .section-header_filter {
      display: block;
    }

    .section-body_filter {
      display: none;
    }

  @media ${tablet} {
    padding: 0 0 0 16px;

    .section-header_filter {
      display: none;
    }

    .section-body_filter {
      display: block;
      margin: 0 0 16px;
    }
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
