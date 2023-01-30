import styled from "styled-components";

export const RoundButton = styled.button`
  width: 32px;
  height: 32px;
  border-radius: 50%;
  outline: 1px solid #eceef1;
  border: none;
  background-color: transparent;
  position: relative;

  transition: all ease-in-out 0.05s;

  span {
    border-color: ${(props) => (props.disabled ? "#A3A9AE" : "#555f65")};
  }

  :hover {
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
    outline: ${(props) =>
      props.disabled ? "1px solid #eceef1" : "2px solid #4781d1"};
    span {
      border-color: ${(props) => (props.disabled ? "#A3A9AE" : "#4781d1")};
    }
  }
`;
