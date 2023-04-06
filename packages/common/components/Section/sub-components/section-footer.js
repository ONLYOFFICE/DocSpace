import React from "react";
import styled from "styled-components";
import { smallTablet } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";

const StyledSectionFooter = styled.div`
  margin-top: 40px;

  @media ${smallTablet}, ${isMobileOnly} {
    margin-top: 32px;
  }
`;

const SectionFooter = ({ children }) => {
  return <StyledSectionFooter>{children}</StyledSectionFooter>;
};

SectionFooter.displayName = "SectionFooter";

export default SectionFooter;
