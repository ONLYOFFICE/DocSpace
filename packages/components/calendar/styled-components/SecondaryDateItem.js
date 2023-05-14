import React from "react";
import styled from "styled-components";
import Base from "../../themes/base";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

export const SecondaryDateItem = styled(({ ...props }) => (
  <ColorTheme themeId={ThemeType.DateItem} {...props} />
))`
  color: ${(props) =>
    props.disabled
      ? props.theme.calendar.disabledColor
      : props.theme.calendar.pastColor} !important;

  :hover {
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
    color: ${(props) =>
      props.disabled
        ? props.theme.calendar.disabledColor
        : props.theme.calendar.pastColor} !important;
  }
`;
SecondaryDateItem.defaultProps = { theme: Base };
