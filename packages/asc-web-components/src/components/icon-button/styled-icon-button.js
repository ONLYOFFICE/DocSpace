import styled from "styled-components";

const StyledOuter = styled.div`
width: ${(props) =>
  props.size ? Math.abs(parseInt(props.size)) + "px" : "20px"};
cursor: ${(props) =>
  props.isDisabled || !props.isClickable ? "default" : "pointer"};
line-height: 0;
-webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

export default StyledOuter;