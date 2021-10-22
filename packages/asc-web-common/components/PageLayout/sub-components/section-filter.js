import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import { tablet } from "@appserver/components/utils/device";

const StyledSectionFilter = styled.div`
  margin: 11px 24px 9px 0;

  @media ${tablet} {
    margin-left: -4px;
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
