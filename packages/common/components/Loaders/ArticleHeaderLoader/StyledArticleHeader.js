import { isMobile } from "react-device-detect";
import styled from "styled-components";
import { tablet } from "@docspace/components/utils/device";
const StyledContainer = styled.div`
  max-width: 211px;

  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `margin-right: 1px;`
      : `margin-left: 1px;`}

  ${isMobile} {
    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-right: 0;`
        : `margin-left: 0;`}
  }

  @media ${tablet} {
    ${(props) => {
      const value = props.showText ? "10px" : "0";

      return props.theme.interfaceDirection === "rtl"
        ? `margin-right: ${value};`
        : `margin-left: ${value};`;
    }}
  }
`;

export default StyledContainer;
