import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledChart = styled.div`
  position: relative;

  #chartjs-tooltip {
    opacity: 1;
    position: absolute;
    background: rgba(255, 255, 255, 0.9);
    box-shadow: 1px 1px 6px rgba(0, 0, 0, 0.3);
    color: #333;
    border-radius: 4px;
    border-left: 5px solid #63b963;
    padding: 50px;
    width: 140px;
    min-height: 150px;
    -webkit-transition: all 0.5s ease;
    transition: all 0.5s ease;
    pointer-events: none;
    -webkit-transform: translate(-50%, 0);
    transform: translate(-50%, 5%);
  }

  .chartjs-tooltip-key {
    display: inline-block;
    width: 10px;
    height: 10px;
    background: "pink";
    margin-right: 10px;
  }
  .tooltip-title {
    color: #666;
    font-size: 13px;
    font-weight: 600 !important;
    font-family: "Raleway";
  }
  .tooltip-value {
    color: #63b963;
    font-size: 22px;
    font-weight: 600 !important;
    font-family: "Raleway";
  }
  .tooltip-value sup {
    font-size: 12px;
  }
`;

StyledChart.defaultProps = { theme: Base };

export default StyledChart;
