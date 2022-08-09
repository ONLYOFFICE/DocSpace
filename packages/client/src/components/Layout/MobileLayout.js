import React, { Component, createRef } from "react";
import { isTouchDevice } from "@docspace/components/utils/device";
import Scrollbar from "@docspace/components/scrollbar";
import { LayoutContextProvider } from "./context";
import { getBannerAttribute } from "@docspace/components/utils/banner";
import PropTypes from "prop-types";
import {
  isTablet,
  isMobile,
  isSafari,
  isIOS,
  isChrome,
} from "react-device-detect";
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
    const { headerHeight } = getBannerAttribute();

    const currentScrollPosition =
      this.customScrollElm.scrollTop > 0 ? this.customScrollElm.scrollTop : 0;

    if (
      isTablet &&
      document.getElementsByClassName("backdrop-active").length > 0 &&
      !this.props.isArticleVisibleOnUnpin
    ) {
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
        headerHeight
    ) {
      if (!this.state.visibleContent)
        this.setState({
          visibleContent: true,
        });
      return;
    }

    if (
      prevScrollPosition - currentScrollPosition > 0 &&
      currentScrollPosition < headerHeight
    ) {
      if (!this.state.visibleContent)
        this.setState({
          visibleContent: true,
        });
      return;
    }

    if (
      (isSafari || isIOS) &&
      Math.abs(currentScrollPosition - prevScrollPosition) <= headerHeight &&
      currentScrollPosition === 0
    ) {
      if (!this.state.visibleContent)
        this.setState({
          visibleContent: true,
        });
      return;
    }

    if (Math.abs(currentScrollPosition - prevScrollPosition) <= headerHeight) {
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
      <Scrollbar
        id="customScrollBar"
        {...scrollProp}
        stype="mediumBlack"
        scrollclass="mobile-scroll"
      >
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
