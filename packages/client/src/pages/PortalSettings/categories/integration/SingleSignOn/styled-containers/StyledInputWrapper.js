import styled from "styled-components";

const StyledInputWrapper = styled.div`
  width: 100%;
  max-width: ${(props) => props.maxWidth || "350px"};
`;

export default StyledInputWrapper;
