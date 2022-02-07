import styled from "styled-components";

const InputWrapper = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;

  input {
    height: 120px;
    width: 100px;
    display: block;
    background: #ffffff;
    border: 1px solid #d0d5da;
    box-sizing: border-box;
    border-radius: 8px;
    margin: 0 4px;
    text-align: center;
    font-size: 72px;

    @media (max-width: 768px) {
      height: 76px;
      width: 64px;
      font-size: 48px;
    }

    @media (max-width: 414px) {
      height: 48px;
      width: 40px;
      font-size: 32px;
    }

    &:last-child {
      margin: 0;
    }
  }

  input:focus {
    border: 1px solid #3b72a7;
  }

  input:disabled {
    color: #a3a9ae;
    background: #f8f9f9;
    border: 1px solid #eceef1;
    outline: none;
  }

  hr {
    width: 24px;
    height: 1px;
    background: #c4c4c4;
    border: none;
    margin: 0 16px;

    @media (max-width: 768px) {
      margin: 0 4px;
    }
  }
`;

export default InputWrapper;
