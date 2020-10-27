
import React, { forwardRef, useEffect, Component, useState } from "react"
import styled from "styled-components";
import {  Scrollbar } from "asc-web-components";
import { isMobile } from "react-device-detect";
import { scrolledTheVerticalAxis } from "../../utils";
const StyledContainer = styled.div`
width:100%;
height:100vh;
`



const Layout = (({children}) => {
  
      return(
        <StyledContainer className="Layout">
          {isMobile  
          ? <Scrollbar  stype="mediumBlack">
            {/* { console.log("(IN APP REF", scrollRefPage)} */}
    
             {children}
             </Scrollbar>
    
           :  children
             }
         </StyledContainer>)
})




export default Layout;