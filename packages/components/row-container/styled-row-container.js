import styled from "styled-components";

const StyledRowContainer = styled.div`
  height: ${(props) =>
    props.useReactWindow
      ? props.manualHeight
        ? props.manualHeight
        : "100%"
      : "auto"};
  position: relative;
`;

export default StyledRowContainer;
