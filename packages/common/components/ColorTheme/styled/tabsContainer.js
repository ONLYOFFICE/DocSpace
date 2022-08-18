import styled, { css } from "styled-components";
import { Label } from "@docspace/components/tabs-container/styled-tabs-container";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ $currentColorScheme, selected, theme }) =>
  $currentColorScheme &&
  css`
    background-color: ${selected &&
    theme.isBase &&
    $currentColorScheme.accentColor} !important;
  `;

Label.defaultProps = { theme: Base };

export default styled(Label)(getDefaultStyles);
