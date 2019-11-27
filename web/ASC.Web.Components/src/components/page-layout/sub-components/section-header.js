import React from "react";
import styled from "styled-components";

const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #eceef1;
  height: 56px;
  margin-right: 16px;
`;

const SectionHeader = React.memo(props => {
  //console.log("PageLayout SectionHeader render");
  return <StyledSectionHeader {...props} />;
});

SectionHeader.displayName = "SectionHeader";

export default SectionHeader;
