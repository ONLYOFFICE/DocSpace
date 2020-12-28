import React, { Component, createRef } from "react";
import { Scrollbar, utils } from "asc-web-components";
import { LayoutContextProvider } from "./context";
import { isMobile, isSafari, isIOS, isChrome } from "react-device-detect";

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
    this.customScrollElm = document.querySelector(
      "#customScrollBar > .scroll-body"
    );

    if (!isChrome) this.customScrollElm.scrollTo(0, 0);

    this.customScrollElm.addEventListener(
      "scroll",
      this.scrolledTheVerticalAxis
    );

    // this.setState({ visibleContent: true });
  }

  componentWillUnmount() {
    this.customScrollElm.removeEventListener(
      "scroll",
      this.scrolledTheVerticalAxis
    );
  }

  scrolledTheVerticalAxis = () => {
    const { prevScrollPosition, visibleContent } = this.state;

    const currentScrollPosition =
      this.customScrollElm.scrollTop > 0
        ? this.customScrollElm.scrollTop
        : window.pageYOffset;

    if (visibleContent && isMobile && !isTouchDevice) {
      return;
    }

    if (Math.abs(currentScrollPosition - prevScrollPosition) <= 54) {
      return;
    }

    if (prevScrollPosition === 0 && currentScrollPosition > 100) {
      this.customScrollElm.scrollTo(0, 0);
      if (Math.abs(currentScrollPosition - prevScrollPosition) <= 104) {
        return;
      }
    }

    let isVisible = prevScrollPosition >= currentScrollPosition;

    if (
      (isSafari || isIOS) &&
      currentScrollPosition >=
        this.customScrollElm.scrollHeight - this.customScrollElm.clientHeight &&
      this.customScrollElm.scrollHeight !== this.customScrollElm.clientHeight
    ) {
      isVisible = false;
    }

    if (
      !visibleContent &&
      this.customScrollElm.scrollHeight - this.customScrollElm.clientHeight < 57
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
      <Scrollbar id="customScrollBar" {...scrollProp} stype="mediumBlack">
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
