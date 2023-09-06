import styled, { css } from "styled-components";
import Text from "@docspace/components/text";

const StyledText = styled(Text)`
  display: inline-block;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-left: 12px;
        `
      : css`
          margin-right: 12px;
        `}
`;

const StyledAuthorCell = styled.div`
  display: flex;
  width: 100%;
  overflow: hidden;

  .author-avatar-cell {
    width: 16px;
    min-width: 16px;
    height: 16px;
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 8px;
          `
        : css`
            margin-right: 8px;
          `}
  }
`;

export { StyledText, StyledAuthorCell };
