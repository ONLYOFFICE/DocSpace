import React from "react";
import styled from "styled-components";

const StyledSectionFooter = styled.div`
  margin-top: 40px;
`;

const SectionFooter = ({ children }) => {
  return <StyledSectionFooter>{children}</StyledSectionFooter>;
};

SectionFooter.displayName = "SectionFooter";

export default SectionFooter;
