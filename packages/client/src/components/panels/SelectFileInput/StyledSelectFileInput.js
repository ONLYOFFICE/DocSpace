import styled from "styled-components";

const StyledComponent = styled.div`
  .select-file_file-input {
    margin: 16px 0;
    width: 100%;
    max-width: ${(props) => (props.maxWidth ? props.maxWidth : "350px")};
  }
`;
export default StyledComponent;
