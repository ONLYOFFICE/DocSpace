import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import { tablet } from "@docspace/components/utils/device";

const StyledSectionPaging = styled.div`
  margin: 16px 0 0;
  padding-right: 3px;

  @media ${tablet} {
    padding-right: 0px;
  }
`;

class SectionPaging extends React.Component {
  render() {
    return <StyledSectionPaging {...this.props} />;
  }
}

SectionPaging.displayName = "SectionPaging";

export default SectionPaging;
