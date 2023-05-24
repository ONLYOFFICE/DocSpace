import styled, { css } from "styled-components";
import Base from "@docspace/components/themes/base";
import { isMobileOnly } from "react-device-detect";

const StyledWrapper = styled.div`
  #ipl-progress-indicator {
    position: fixed;
    z-index: 390;
    top: 0;
    left: -6px;
    width: 0%;
    height: 3px;
    -moz-border-radius: 1px;
    -webkit-border-radius: 1px;
    border-radius: 1px;

    ${isMobileOnly &&
    css`
      top: 48px;
    `}
  }
`;

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    #ipl-progress-indicator {
      background-color: ${theme.isBase
        ? $currentColorScheme.main.accent
        : "#FFFFFF"};

      &:hover {
        background-color: ${theme.isBase
          ? $currentColorScheme.main.accent
          : "#FFFFFF"};
      }
    }
  `;
StyledWrapper.defaultProps = { theme: Base };
export default styled(StyledWrapper)(getDefaultStyles);
