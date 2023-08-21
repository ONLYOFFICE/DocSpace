import { css } from "styled-components";
import { getCorrectTextAlign } from "../utils/rtlUtils";

const commonTextStyles = css`
  font-family: ${(props) => props.theme.fontFamily};
  text-align: ${(props) =>
    getCorrectTextAlign(props.textAlign, props.theme.interfaceDirection)};
  color: ${(props) =>
    props.colorProp ? props.colorProp : props.theme.text.color};
  ${(props) =>
    props.truncate &&
    css`
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    `}
`;

export default commonTextStyles;
