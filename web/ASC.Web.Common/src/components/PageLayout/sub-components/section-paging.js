import React from "react";
import styled from "styled-components";

const StyledSectionPaging = styled.div`
  margin: 16px 0 0;
`;

const SectionPaging = React.memo(props => {
  //console.log("PageLayout SectionPaging render");
  return <StyledSectionPaging {...props} />;
});

SectionPaging.displayName = "SectionPaging";

export default SectionPaging;
