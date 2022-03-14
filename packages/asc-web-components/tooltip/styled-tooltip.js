import styled from "styled-components";
import Base from "../themes/base";

const StyledTooltip = styled.div`
  .__react_component_tooltip {
    background-color: ${(props) => props.color} !important;
    border-radius: ${(props) => props.theme.tooltip.borderRadius};
    -moz-border-radius: ${(props) => props.theme.tooltip.borderRadius};
    -webkit-border-radius: ${(props) => props.theme.tooltip.borderRadius};
    box-shadow: ${(props) => props.theme.tooltip.boxShadow};
    -moz-box-shadow: ${(props) => props.theme.tooltip.boxShadow};
    -webkit-box-shadow: ${(props) => props.theme.tooltip.boxShadow};
    opacity: ${(props) => props.theme.tooltip.opacity};
    padding: ${(props) => props.theme.tooltip.padding};
    pointer-events: ${(props) => props.theme.tooltip.pointerEvents};
    max-width: ${(props) =>
      props.maxWidth ? props.maxWidth : props.theme.tooltip.maxWidth};

    &:before {
      border: ${(props) => props.theme.tooltip.before.border};
    }
    &:after {
      border: ${(props) => props.theme.tooltip.after.border};
    }
  }

  .__react_component_tooltip.place-left::after {
    border-left: 6px solid ${(props) => props.color} !important;
  }

  .__react_component_tooltip.place-right::after {
    border-right: 6px solid ${(props) => props.color} !important;
  }

  .__react_component_tooltip.place-top::after {
    border-top: 6px solid ${(props) => props.color} !important;
  }

  .__react_component_tooltip.place-bottom::after {
    border-bottom: 6px solid ${(props) => props.color} !important;
  }
`;

StyledTooltip.defaultProps = {
  theme: Base,
};

export default StyledTooltip;
