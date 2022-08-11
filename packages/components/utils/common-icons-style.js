import { css } from "styled-components";

const iconSizes = {
  small: 12,
  medium: 16,
  big: 24,
};
const getSizeStyle = (size) => {
  switch (size) {
    case "scale":
      return `
          &:not(:root) {
            width: 100%;
            height: 100%;
          }
        `;
    case "small":
    case "medium":
    case "big":
      return `
          width: ${iconSizes[size]}px;
          min-width: ${iconSizes[size]}px;
          height: ${iconSizes[size]}px;
          min-height: ${iconSizes[size]}px;
        `;
    default:
      return `
          width: ${iconSizes.big}px;
          min-width: ${iconSizes.big}px;
          height: ${iconSizes.big}px;
          min-height: ${iconSizes.big}px;
        `;
  }
};

const commonIconsStyles = css`
  overflow: hidden;
  vertical-align: middle;
  ${(props) => getSizeStyle(props.size)};
`;

export default commonIconsStyles;
