import styled, { css } from "styled-components";
import StyledIconWrapper from "@appserver/common/components/Navigation/sub-components/StyledIconWrapper";

const getDefaultStyles = ({ currentColorScheme }) => css`
  svg {
    path:nth-child(2) {
        fill: ${currentColorScheme.accentColor};
    }
`;

export default styled(StyledIconWrapper)(getDefaultStyles);
