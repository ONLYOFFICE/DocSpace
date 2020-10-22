import { Box } from "asc-web-components";
import React from "react"
import styled from "styled-components";

const StyledContainer = styled.div`
width:100%;
height:100%;
`

const Layout = ({children}) => (
    
    <StyledContainer>
        {children}
    </StyledContainer>
)

export default Layout;