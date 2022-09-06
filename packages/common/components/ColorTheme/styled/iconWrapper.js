import styled, { css } from "styled-components";
import StyledIconWrapper from "@docspace/common/components/Navigation/sub-components/StyledIconWrapper";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
  svg {
    path:nth-child(2) {
        fill: ${$currentColorScheme.accentColor};
    }
`;

export default styled(StyledIconWrapper)(getDefaultStyles);
