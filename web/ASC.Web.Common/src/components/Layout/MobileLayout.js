import React, { Component, createRef } from "react";
import { Scrollbar, utils } from "asc-web-components";
import { LayoutContextProvider } from "./context";
import PropTypes from "prop-types";
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
      this.customScrollElm.scrollTop > 0 ? this.customScrollElm.scrollTop : 0;

    if (document.getElementsByClassName("backdrop-active").length > 0) {
      const elements = document.getElementsByClassName("backdrop-active");
      elements[0].click();
      return;
    }

    if (visibleContent && isMobile && !isTouchDevice) {
      return;
    }
    if (
      (isSafari || isIOS) &&
      this.customScrollElm.scrollHeight - this.customScrollElm.clientHeight <
        112
    ) {
      if (!this.state.visibleContent)
        this.setState({
          visibleContent: true,
        });
      return;
    }

    if (
      (isSafari || isIOS) &&
      Math.abs(currentScrollPosition - prevScrollPosition) <= 112 &&
      currentScrollPosition === 0
    ) {
      if (!this.state.visibleContent)
        this.setState({
          visibleContent: true,
        });
      return;
    }

    if (Math.abs(currentScrollPosition - prevScrollPosition) <= 112) {
      return;
    }

    if (prevScrollPosition === 0 && currentScrollPosition > 100) {
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

MobileLayout.propTypes = {
  children: PropTypes.any,
};

export default MobileLayout;
