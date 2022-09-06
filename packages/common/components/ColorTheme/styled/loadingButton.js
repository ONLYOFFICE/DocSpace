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
        background-color: ${theme.isBase
          ? $currentColorScheme.accentColor
          : theme.filesPanels.upload.loadingButton.color};
      }
    }

    ${StyledLoadingButton} {
      color: ${theme.isBase
        ? $currentColorScheme.accentColor
        : theme.filesPanels.upload.loadingButton.color};
    }
  `;

StyledCircleWrap.defaultProps = {
  theme: Base,
};

export default styled(StyledCircleWrap)(getDefaultStyles);
