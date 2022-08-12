import styled, { css } from "styled-components";
import { Label } from "@docspace/components/tabs-container/styled-tabs-container";

const getDefaultStyles = ({ currentColorScheme, selected }) => css`
  background-color: ${selected && currentColorScheme.accentColor};
`;

export default styled(Label)(getDefaultStyles);
