import React from "react";
import styled from "styled-components";

const StyledSectionFilter = styled.div`
  margin: 0 0 16px;
`;

const SectionFilter = React.memo(props => {
  //console.log("PageLayout SectionFilter render");
  return <StyledSectionFilter {...props} />;
});

SectionFilter.displayName = "SectionFilter";

export default SectionFilter;
