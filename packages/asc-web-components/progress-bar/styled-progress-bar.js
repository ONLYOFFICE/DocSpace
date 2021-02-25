import styled from "styled-components";
import Base from "../themes/base";

const StyledProgressBar = styled.div`
  position: relative;
  height: ${(props) => props.theme.progressBar.height};
  background-color: ${(props) => props.theme.progressBar.backgroundColor};

  .progress-bar_full-text {
    display: block;
    padding: ${(props) => props.theme.progressBar.fullText.padding};
    position: absolute;
    font-weight: ${(props) => props.theme.progressBar.fullText.fontWeight};
    margin: ${(props) => props.theme.progressBar.fullText.margin};
  }

  .progress-bar_percent {
    width: ${(props) => props.uploadedPercent}%;
    float: ${(props) => props.theme.progressBar.percent.float};
    overflow: ${(props) => props.theme.progressBar.percent.overflow};
    max-height: ${(props) => props.theme.progressBar.percent.maxHeight};
    min-height: ${(props) => props.theme.progressBar.percent.minHeight};
  }
  .progress-bar_field {
    width: ${(props) => props.remainPercent}%;
    float: left;
    overflow: hidden;
    max-height: 22px;
    min-height: 22px;
  }

  .progress-bar_percent {
    transition: ${(props) => props.theme.progressBar.percent.transition};
    background: ${(props) => props.theme.progressBar.percent.background};
  }

  .progress-bar_text {
    min-width: ${(props) => props.theme.progressBar.text.minWidth};

    .progress-bar_progress-text {
      padding: ${(props) => props.theme.progressBar.text.progressText.padding};
      position: relative;
      margin: ${(props) => props.theme.progressBar.text.progressText.margin};
      min-width: ${(props) =>
        props.theme.progressBar.text.progressText.minWidth};
      font-weight: ${(props) =>
        props.theme.progressBar.text.progressText.fontWeight};
    }
  }
  .progress-bar_field .progress-bar_text {
    margin-left: ${(props) => props.theme.progressBar.marginLeft};
  }

  .progress-bar_drop-down {
    padding: ${(props) => props.theme.progressBar.dropDown.padding};
  }
`;

StyledProgressBar.defaultProps = { theme: Base };

export default StyledProgressBar;
