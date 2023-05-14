import React from "react";
import styled from "styled-components";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

export const CurrentDateItem = styled(({ ...props }) => (
  <ColorTheme themeId={ThemeType.DateItem} {...props} />
))`
  border-radius: 50%;
  color: white !important;

  :hover {
    color: white !important;
  }

  :focus {
    color: white !important;
  }
`;
