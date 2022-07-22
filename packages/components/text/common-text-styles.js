import { css } from "styled-components";

const commonTextStyles = css`
  font-family: ${(props) => props.theme.fontFamily};
  text-align: ${(props) => props.textAlign};
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
