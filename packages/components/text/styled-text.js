import styled, { css } from "styled-components";
import commonTextStyles from "./common-text-styles";
import Base from "../themes/base";
import NoUserSelect from "@docspace/components/utils/commonStyles";
const styleCss = css`
  font-size: ${(props) => props.fontSizeProp};
  outline: 0 !important;
  margin: 0;
  font-weight: ${(props) =>
    props.fontWeightProp
      ? props.fontWeightProp
      : props.isBold == true
      ? 700
      : props.theme.text.fontWeight};

  ${(props) =>
    props.isItalic == true &&
    css`
      font-style: italic;
    `}
  ${(props) =>
    props.backgroundColor &&
    css`
      background-color: ${(props) => props.backgroundColor};
    `}
  ${(props) =>
    props.isInline
      ? css`
          display: inline-block;
        `
      : props.display &&
        css`
          display: ${(props) => props.display};
        `}
`;
const StyledText = styled.p`
  ${styleCss};

  ${commonTextStyles};
  ${(props) => props.noSelect && NoUserSelect}
`;

StyledText.defaultProps = { theme: Base };

export default StyledText;
