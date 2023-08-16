import styled from "styled-components";

const StyledInputWrapper = styled.div`
  width: 100%;
  max-width: ${(props) => props.maxWidth || "350px"};

  .field-input {
    ::placeholder {
      font-size: 13px;
      font-weight: 400;
    }
  }

  .field-label-icon {
    align-items: center;
    margin-bottom: 4px;
    max-width: 350px;
  }

  .field-label {
    display: flex;
    align-items: center;
    height: auto;
    font-weight: 600;
    line-height: 20px;
    overflow: visible;
    white-space: normal;
  }
`;

export default StyledInputWrapper;
