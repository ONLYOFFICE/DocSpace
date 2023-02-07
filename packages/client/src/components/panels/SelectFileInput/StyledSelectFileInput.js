import styled from "styled-components";

const StyledComponent = styled.div`
  .select-file_file-input {
    margin-bottom: 16px;
    width: 100%;
    max-width: ${(props) => (props.maxWidth ? props.maxWidth : "350px")};
  }
`;
export default StyledComponent;
