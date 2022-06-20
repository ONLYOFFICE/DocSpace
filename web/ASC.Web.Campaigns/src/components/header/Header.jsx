import React from 'react';
import styled from "styled-components";
import logo from './nav.svg'



const StyledHeader = styled.div`
  border-bottom: 1px solid rgba(0, 0, 0, 0.1);
  padding: 8px 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: ${props => props.theme.headerColor};
`;


export const Header = () => (
    <StyledHeader>
        <img src={logo} />
    </StyledHeader>
);
