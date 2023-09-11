import React from "react";
import styled, { css } from "styled-components";
import equal from "fast-deep-equal/react";
import { tablet } from "@docspace/components/utils/device";

const StyledSectionPaging = styled.div`
  margin: 16px 0 0;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          padding-left: 3px;
        `
      : css`
          padding-right: 3px;
        `}

  @media ${tablet} {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-left: 0px;
          `
        : css`
            padding-right: 0px;
          `}
  }
`;

class SectionPaging extends React.Component {
  render() {
    return <StyledSectionPaging {...this.props} />;
  }
}

SectionPaging.displayName = "SectionPaging";

export default SectionPaging;
