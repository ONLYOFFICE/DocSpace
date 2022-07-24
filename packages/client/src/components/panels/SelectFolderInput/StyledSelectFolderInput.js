import styled from "styled-components";

const StyledComponent = styled.div`
  .select-folder_file-input {
    margin-bottom: 16px;
    margin-top: 3px;
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
