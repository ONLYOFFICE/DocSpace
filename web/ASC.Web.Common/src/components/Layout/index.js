
import React, { useEffect, useState} from "react"
import styled from "styled-components";

import MobileLayout from "./MobileLayout"


const StyledContainer = styled.div`
width:100%;
height:100vh;
`

const Layout = (props) => {

  const { children } = props
  const isTablet = window.innerWidth < 1024;

  const [windowWidth, setWindowWidth] = useState({
    matches: isTablet,
  });

  useEffect(() => {
    let mediaQuery = window.matchMedia("(max-width: 1024px)");
    mediaQuery.addListener(setWindowWidth);

    return () => mediaQuery.removeListener(setWindowWidth);
  }, []);

  return( 

    <StyledContainer className="Layout">
              {windowWidth && windowWidth.matches 
                ? <MobileLayout  {...props}/>
                : children
              }
    </StyledContainer>
  );
}
export default Layout;