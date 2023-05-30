import { css } from "styled-components";
import getTextAlign from "./utils/getTextAlign";

const commonTextStyles = css`
  font-family: ${(props) => props.theme.fontFamily};
  text-align: ${(props) =>
    getTextAlign(props.textAlign, props.theme.interfaceDirection)};
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
