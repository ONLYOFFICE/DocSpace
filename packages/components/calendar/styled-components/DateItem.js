import styled from "styled-components";

export const DateItem = styled.button`
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 16px;
  color: #333333;

  border: 2px solid transparent;
  background-color: transparent;

  width: ${props => props.big ? '60px' : '40px'};
  height: ${props => props.big ? '60px' : '40px'};

  display: inline-flex;
  justify-content: center;
  align-items: center;

  :hover {
    cursor: pointer;
    background: #f3f4f4;
    border-radius: 50%;
    color: #333333;
  }

  :focus{
    color: #4781D1;
    border-radius: 50%;
    border: 2px solid #4781D1;
  }
`;
