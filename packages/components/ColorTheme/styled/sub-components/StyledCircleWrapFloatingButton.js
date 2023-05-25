import styled from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledCircleWrapFloatingButton = styled.div`
  position: relative;
  z-index: 500;
  width: 48px;
  height: 48px;
  background: ${(props) =>
    props.color ? props.color : props.theme.floatingButton.backgroundColor};
  border-radius: 50%;
  cursor: pointer;
  box-shadow: ${(props) => props.theme.floatingButton.boxShadow};
`;

StyledCircleWrapFloatingButton.defaultProps = { theme: Base };

export default StyledCircleWrapFloatingButton;
