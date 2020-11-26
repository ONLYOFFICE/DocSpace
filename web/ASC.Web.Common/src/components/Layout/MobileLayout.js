import React, { Component, createRef } from "react";
import { Scrollbar, utils } from "asc-web-components";
import { LayoutContextProvider } from "./context";
import { isMobile, isSafari, isIOS } from "react-device-detect";

const { isTouchDevice } = utils.device;
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

    this.documentElement.scrollTo(0, 0);

    this.documentElement.addEventListener(
      "scroll",
      this.scrolledTheVerticalAxis
    );

    // this.setState({ visibleContent: true });
  }

  componentWillUnmount() {
    this.documentElement.removeEventListener(
      "scroll",
      this.scrolledTheVerticalAxis
    );
  }

  scrolledTheVerticalAxis = () => {
    const { prevScrollPosition, visibleContent } = this.state;

    if (visibleContent && isMobile && !isTouchDevice) {
      return;
    }

    const currentScrollPosition =
      this.documentElement.scrollTop > 0
        ? this.documentElement.scrollTop
        : window.pageYOffset;

    if (Math.abs(currentScrollPosition - prevScrollPosition) <= 104) return;

    let isVisible = prevScrollPosition >= currentScrollPosition;

    if (
      (isSafari || isIOS) &&
      currentScrollPosition >=
        this.documentElement.scrollHeight - this.documentElement.clientHeight &&
      this.documentElement.scrollHeight !== this.documentElement.clientHeight
    ) {
      isVisible = false;
    }

    if (
      !visibleContent &&
      this.documentElement.scrollHeight - this.documentElement.clientHeight < 57
    ) {
      isVisible = true;
    }

    this.setState({
      prevScrollPosition: currentScrollPosition,
      visibleContent: isVisible,
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
