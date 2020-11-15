
import React, { Component, createRef, useRef, useEffect} from "react"
import styled from "styled-components";
import {  Scrollbar} from "asc-web-components";

import {LayoutContextProvider} from "./context"


const StyledContainer = styled.div`
width:100%;
height:100vh;
`
class ExpandLayout extends Component{
  constructor(props) {
    super(props);

    this.windowWidth = window.matchMedia( "(max-width: 1024px)" );

    this.state = {
      isTablet: window.innerWidth < 1024,
      prevScrollPosition:  window.pageYOffset,
      visibleContent: true,
    };

    this.scrollRefPage = createRef();
  }


  componentDidMount() {
  this.getElementById = document.getElementById("scroll");

  this.getElementById.addEventListener("scroll", this.scrolledTheVerticalAxis);
  }

  componentWillUnmount() {

     this.getElementById.removeEventListener("scroll", this.scrolledTheVerticalAxis);
  }

  scrolledTheVerticalAxis = () => {

    const { prevScrollPosition } = this.state;
    const currentScrollPosition = this.getElementById.scrollTop || window.pageYOffset ;
    let visibleContent = prevScrollPosition >= currentScrollPosition;

    if (!visibleContent && (this.getElementById.scrollHeight - this.getElementById.clientHeight < 57)) {
      visibleContent = true
    }

     this.setState({
      prevScrollPosition: currentScrollPosition,
      visibleContent
    });
  };

  render() {  
  const scrollProp =  { ref: this.scrollRefPage } ;
  const { children, windowWidth } = this.props
    return(
            <StyledContainer className="Layout">
              {windowWidth && windowWidth.matches 
                ? <Scrollbar {...scrollProp} stype="mediumBlack">
                    <LayoutContextProvider value={{scrollRefLayout: this.scrollRefPage, isVisible:this.state.visibleContent}}>
                          { children }
                    </LayoutContextProvider>
                  </Scrollbar>
          
              :  children
                }
              </StyledContainer>
          
         
    )
  }
}

const Layout = (props) => {
  // const scrollRefPage = useRef(null)
  // const scrollProp =  { ref: scrollRefPage } ;

  const isTablet = window.innerWidth < 1024;

  const [windowWidth, setWindowWidth] = React.useState({
    matches: isTablet,
  });

  useEffect(() => {
    let mediaQuery = window.matchMedia("(max-width: 1024px)");
    mediaQuery.addListener(setWindowWidth);

    return () => mediaQuery.removeListener(setWindowWidth);
  }, []);

  return <ExpandLayout windowWidth={windowWidth} {...props}/>;
}
export default Layout;