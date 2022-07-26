import styled, { css } from "styled-components";

import { StyledSubmenuItemLabel } from "@appserver/components/submenu/styled-submenu";

const getDefaultStyles = ({ currentColorScheme, isActive }) => css`
  background-color: ${isActive ? currentColorScheme.accentColor : "none"};

  &:hover {
    background-color: ${isActive && currentColorScheme.accentColor};
  }
`;

export default styled(StyledSubmenuItemLabel)(getDefaultStyles);
