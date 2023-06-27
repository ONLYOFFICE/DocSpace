import styled from "styled-components";

const StyledContainer = styled.div`
  width: 100%;
  height: 100%;

  display: flex;

  flex-direction: column;

  .plugins__upload-button {
    width: 110px;

    margin-bottom: 24px;
  }

  .custom-plugin-input {
    display: none;
  }
`;

export { StyledContainer };
