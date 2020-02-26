import React from "react";
import styled from "styled-components";

const StyledSectionFilter = styled.div`
  margin: 16px 16px 8px 0;
`;

const SectionFilter = React.memo(props => {
  //console.log("PageLayout SectionFilter render");
  return <StyledSectionFilter className='section-filter' {...props} />;
});

SectionFilter.displayName = "SectionFilter";

export default SectionFilter;
