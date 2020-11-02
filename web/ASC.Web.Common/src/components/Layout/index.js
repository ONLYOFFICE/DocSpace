
import React, { Component, createRef} from "react"
import styled from "styled-components";
import {  Scrollbar } from "asc-web-components";
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
  
  }
  
  componentDidMount() {
    isMobile && document.getElementById("scroll").addEventListener("scroll", this.scrolledTheVerticalAxis,false);
  }


  componentWillUnmount() {
    isMobile && document.getElementById("scroll").removeEventListener("scroll", this.scrolledTheVerticalAxis,false);
  }

  scrolledTheVerticalAxis = () => {
    const { prevScrollPosition } = this.state;
    const currentScrollPosition =  document.getElementById("scroll").scrollTop || window.pageYOffset ;
    const visibleContent = prevScrollPosition > currentScrollPosition;
    this.setState({
      prevScrollPosition: currentScrollPosition,
      visibleContent
    });
  };
  render() {  
  const scrollProp =  { ref: this.scrollRefPage } ;
  const { children } = this.props

    return(
      <StyledContainer className="Layout">
           { isMobile  
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