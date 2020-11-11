
import React, { Component, createRef} from "react"
import styled from "styled-components";
import {  Scrollbar, utils} from "asc-web-components";
import { isMobile } from "react-device-detect";
import {RefContextProvider, IsVisibleContextProvider} from "./context"

const StyledContainer = styled.div`
width:100%;
height:100vh;
`
class Layout extends Component{
  constructor(props) {
    super(props);
    this.state = {

      prevScrollPosition:  window.pageYOffset ,
      visibleContent: true
    };
    this.scrollRefPage = createRef();
    this.windowWidth = window.matchMedia( "(max-width: 1024px)" );
  }

  componentDidMount() {

    (isMobile || this.windowWidth.matches ) && document.getElementById("scroll").addEventListener("scroll", this.scrolledTheVerticalAxis,false);
  }

  componentWillUnmount() {
    (isMobile || this.windowWidth.matches ) && document.getElementById("scroll").removeEventListener("scroll", this.scrolledTheVerticalAxis,false);
  }


  scrolledTheVerticalAxis = () => {

    const { prevScrollPosition } = this.state;
    const currentScrollPosition =  document.getElementById("scroll").scrollTop || window.pageYOffset ;
    let visibleContent = prevScrollPosition >= currentScrollPosition;

    if (!visibleContent && (document.getElementById("scroll").scrollHeight - document.getElementById("scroll").clientHeight < 57)) {
      visibleContent = true
    }

     this.setState({
      prevScrollPosition: currentScrollPosition,
      visibleContent
    });
  };

  render() {  
  const scrollProp =  { ref: this.scrollRefPage } ;
  const { children } = this.props

    return(
        
          <StyledContainer className="Layout" >
  
              {(isMobile || (!isMobile && this.windowWidth.matches) ) 
                ? <Scrollbar {...scrollProp} stype="mediumBlack">
                    <RefContextProvider value={this.scrollRefPage}>
                      <IsVisibleContextProvider value={this.state.visibleContent}>
                        { children }
                        </IsVisibleContextProvider>
                    </RefContextProvider>
                  </Scrollbar>
          
              :  children
                }
          </StyledContainer>
    )
  }
}


export default Layout;