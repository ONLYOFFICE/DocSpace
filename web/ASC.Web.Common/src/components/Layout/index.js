
import React, { Component, createRef} from "react"
import styled from "styled-components";
import {  Scrollbar } from "asc-web-components";
import { isMobile } from "react-device-detect";
import {ThemeContextProvider} from "./context"

const StyledContainer = styled.div`
width:100%;
height:100vh;
`
class Layout extends Component{
  constructor(props) {
    super(props);

    this.scrollRefPage = createRef();
  
  }
  
  render() {  
  const scrollProp =  { ref: this.scrollRefPage } ;
  const { children } = this.props

    return(
      <StyledContainer className="Layout">
           { isMobile  
            ? <Scrollbar {...scrollProp} stype="mediumBlack">
                <ThemeContextProvider value={this.scrollRefPage}>
                  { children }
                </ThemeContextProvider>
              </Scrollbar>
      
           :  children
            }
          </StyledContainer>

    )
  }
}



export default Layout;