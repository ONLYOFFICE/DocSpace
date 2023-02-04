import styled from "styled-components";

export const DateItem = styled.button`
  font-family: "Open Sans";
  font-weight: 600;
  font-size: 16px;
  border-radius: 50%;
  color: ${props => props.focused ? '#4781D1' : '#333'};

  border: 2px solid;
  border-color: ${props => props.focused ? '#4781D1' : 'transparent'};
  background-color: transparent;

  width: ${props => props.big ? '60px' : '40px'};
  height: ${props => props.big ? '60px' : '40px'};

  display: inline-flex;
  justify-content: center;
  align-items: center;

  :hover {
    cursor: pointer;
    background: #f3f4f4;
    color: #333333;
  }

  :focus{
    color: #4781D1;
    border: 2px solid #4781D1;
  }
`;
