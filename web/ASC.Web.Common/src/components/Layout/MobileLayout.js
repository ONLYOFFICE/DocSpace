import React, { Component, createRef } from "react";
import { Scrollbar } from "asc-web-components";
import { LayoutContextProvider } from "./context";

class MobileLayout extends Component {
  constructor(props) {
    super(props);

    this.state = {
      prevScrollPosition: window.pageYOffset,
      visibleContent: true,
    };

    this.scrollRefPage = createRef();
  }

  componentDidMount() {
    this.documentElement = document.getElementById("customScrollBar");
    this.documentElement.addEventListener(
      "scroll",
      this.scrolledTheVerticalAxis
    );
  }

  componentWillUnmount() {
    this.documentElement.removeEventListener(
      "scroll",
      this.scrolledTheVerticalAxis
    );
  }

  scrolledTheVerticalAxis = () => {
    const { prevScrollPosition } = this.state;
    const currentScrollPosition =
      this.documentElement.scrollTop > 0
        ? this.documentElement.scrollTop
        : window.pageYOffset;

    let visibleContent = prevScrollPosition >= currentScrollPosition;

    if (
      currentScrollPosition >=
        this.documentElement.scrollHeight - this.documentElement.clientHeight &&
      this.documentElement.scrollHeight !== this.documentElement.clientHeight
    ) {
      visibleContent = false;
    }

    if (
      !visibleContent &&
      this.documentElement.scrollHeight - this.documentElement.clientHeight < 57
    ) {
      visibleContent = true;
    }

    this.setState({
      prevScrollPosition: currentScrollPosition,
      visibleContent,
    });
  };

  render() {
    const scrollProp = { ref: this.scrollRefPage };
    const { children } = this.props;

    return (
      <Scrollbar {...scrollProp} stype="mediumBlack">
        <LayoutContextProvider
          value={{
            scrollRefLayout: this.scrollRefPage,
            isVisible: this.state.visibleContent,
          }}
        >
          {children}
        </LayoutContextProvider>
      </Scrollbar>
    );
  }
}

export default MobileLayout;
