import styled from "styled-components";

const StyledComponent = styled.div`
  .select-folder_file-input {
    margin: 16px 0;
    width: 100%;
    max-width: ${(props) => (props.maxWidth ? props.maxWidth : "350px")};
  }

  .panel-loader-wrapper {
    margin-top: 8px;
    padding-left: 32px;
  }
  .panel-loader {
    display: inline;
    margin-right: 10px;
  }
`;

export default StyledComponent;
