import React from "react";
import styled, { css } from "styled-components";
import equal from "fast-deep-equal/react";
import { isMobile } from "react-device-detect";
import { tablet, desktop } from "@docspace/components/utils/device";

const StyledSectionFilter = styled.div`
  margin-right: ${isMobile ? "16px" : "20px"};

  @media ${tablet} {
    margin-right: 16px;
  }
`;

class SectionFilter extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout SectionFilter render");
    return <StyledSectionFilter className="section-filter" {...this.props} />;
  }
}

SectionFilter.displayName = "SectionFilter";

export default SectionFilter;
