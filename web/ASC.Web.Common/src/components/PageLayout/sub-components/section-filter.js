import React from "react";
import styled from "styled-components";
import isEqual from "lodash/isEqual";

const StyledSectionFilter = styled.div`
  margin: 16px 24px 9px 0;
`;

class SectionFilter extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    //console.log("PageLayout SectionFilter render");
    return <StyledSectionFilter className="section-filter" {...this.props} />;
  }
}

SectionFilter.displayName = "SectionFilter";

export default SectionFilter;
