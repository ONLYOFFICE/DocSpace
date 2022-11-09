import styled, { css } from "styled-components";
import { Label } from "@docspace/components/tabs-container/styled-tabs-container";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, selected, theme }) =>
  $currentColorScheme &&
  css`
    background-color: ${selected &&
    theme.isBase &&
    $currentColorScheme.accentColor} !important;

    .title_style {
      color: ${$currentColorScheme.id > 7 &&
      selected &&
      $currentColorScheme.textColor};
    }
  `;

Label.defaultProps = { theme: Base };

export default styled(Label)(getDefaultStyles);
