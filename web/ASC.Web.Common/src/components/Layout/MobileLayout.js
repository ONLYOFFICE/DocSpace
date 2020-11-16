import React, { Component, createRef} from "react"

import {  Scrollbar} from "asc-web-components";

import {LayoutContextProvider} from "./context"

class MobileLayout extends Component{
    constructor(props) {
      super(props);
  
      this.state = {
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
    const { children} = this.props
   
      return(
                <Scrollbar {...scrollProp} stype="mediumBlack">
                      <LayoutContextProvider value={{scrollRefLayout: this.scrollRefPage, isVisible:this.state.visibleContent}}>
                            { children }
                      </LayoutContextProvider>
                </Scrollbar>
      )
    }
  }

  export default MobileLayout;