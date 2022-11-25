import styled, { css } from "styled-components";
import {
  StyledCircle,
  StyledLoadingButton,
  StyledCircleWrap,
} from "@docspace/client/src/components/panels/UploadPanel/SubComponents/StyledLoadingButton";
import { Base } from "@docspace/components/themes";

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    ${StyledCircle} {
      .circle__mask .circle__fill {
        background-color: ${$currentColorScheme.main.accent};
      }
    }

    ${StyledLoadingButton} {
      color: ${$currentColorScheme.main.accent};
    }
  `;

StyledCircleWrap.defaultProps = {
  theme: Base,
};

export default styled(StyledCircleWrap)(getDefaultStyles);
