import styled from "styled-components";
import Base from "../themes/base";

const StyledProgressBar = styled.div`
  position: relative;
  height: ${(props) => props.theme.progressBar.height};
  background-color: ${(props) => props.theme.progressBar.backgroundColor};

  .progress-bar_full-text {
    display: block;
    position: absolute;
    margin-top: 8px;
  }

  .progress-bar_percent {
    width: ${(props) => props.percent}%;
    float: ${(props) => props.theme.progressBar.percent.float};
    overflow: ${(props) => props.theme.progressBar.percent.overflow};
    max-height: ${(props) => props.theme.progressBar.percent.maxHeight};
    min-height: ${(props) => props.theme.progressBar.percent.minHeight};
    transition: ${(props) => props.theme.progressBar.percent.transition};
    background: ${(props) => props.theme.progressBar.percent.background};
  }
`;

StyledProgressBar.defaultProps = { theme: Base };

export default StyledProgressBar;
