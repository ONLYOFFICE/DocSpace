
import React from "react"
import styled from "styled-components";
import {  Scrollbar } from "asc-web-components";
import { isMobile } from "react-device-detect";
const StyledContainer = styled.div`
width:100%;
height:100vh;

`

const Layout = ({children}) => (
    
    <StyledContainer>
      {isMobile  
      ? <Scrollbar  stype="mediumBlack">
        {children}
        </Scrollbar>

      :  {children}
        }
    </StyledContainer>
)

export default Layout;