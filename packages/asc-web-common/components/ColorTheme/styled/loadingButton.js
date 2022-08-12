import styled, { css } from "styled-components";
import {
  StyledCircle,
  StyledLoadingButton,
  StyledCircleWrap,
} from "@appserver/files/src/components/panels/UploadPanel/SubComponents/StyledLoadingButton";
import { Base } from "@appserver/components/themes";

const getDefaultStyles = ({ currentColorScheme, theme }) => css`
  ${StyledCircle} {
    .circle__mask .circle__fill {
      background-color: ${theme.isBase === true
        ? currentColorScheme.accentColor
        : theme.filesPanels.upload.loadingButton.color};
    }
  }

  ${StyledLoadingButton} {
    color: ${theme.isBase === true
      ? currentColorScheme.accentColor
      : theme.filesPanels.upload.loadingButton.color};
  }
`;

StyledCircleWrap.defaultProps = {
  theme: Base,
};

export default styled(StyledCircleWrap)(getDefaultStyles);
