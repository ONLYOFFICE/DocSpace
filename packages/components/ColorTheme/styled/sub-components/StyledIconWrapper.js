import styled from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledIconWrapper = styled.div`
  width: 17px;
  display: flex;
  align-items: ${(props) => (props.isRoot ? "center" : "flex-end")};
  justify-content: center;

  svg {
    path {
      fill: ${(props) => props.theme.navigation.icon.fill};
    }

    circle {
      stroke: ${(props) => props.theme.navigation.icon.fill};
    }

    path:first-child {
      fill: none !important;
      stroke: ${(props) => props.theme.navigation.icon.stroke};
    }
  }
`;

StyledIconWrapper.defaultProps = { theme: Base };

export default StyledIconWrapper;
