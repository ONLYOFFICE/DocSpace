import React from "react";
import PropTypes from "prop-types";

import StyledBackdrop from "./styled-backdrop";

class Backdrop extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      needBackdrop: false,
      needBackground: false,
    };

    this.backdropRef = React.createRef();
  }

  componentDidUpdate(prevProps) {
    if (
      prevProps.visible !== this.props.visible ||
      prevProps.isAside !== this.props.isAside ||
      prevProps.withBackground !== this.props.withBackground
    ) {
      this.checkingExistBackdrop();
    }
  }

  componentDidMount() {
    this.checkingExistBackdrop();
  }

  checkingExistBackdrop = () => {
    const { visible, isAside, withBackground } = this.props;
    if (visible) {
      const isTablet = window.innerWidth < 1024;
      const backdrops = document.querySelectorAll(".backdrop-active");

      const needBackdrop =
        backdrops.length < 1 || (isAside && backdrops.length <= 1);

      let needBackground = needBackdrop && (isTablet || withBackground);

      if (isAside && needBackdrop) needBackground = true;

      this.setState({
        needBackdrop: needBackdrop,
        needBackground: needBackground,
      });
    } else {
      this.setState({
        needBackground: false,
        needBackdrop: false,
      });
    }
  };

  modifyClassName = () => {
    const { className } = this.props;
    let modifiedClass = "backdrop-active";

    if (className) {
      if (typeof className !== "string") {
        if (!className.includes(modifiedClass)) {
          modifiedClass = className.push(modifiedClass);
        } else {
          modifiedClass = className;
        }
      } else {
        modifiedClass += ` ${className}`;
      }
    }

    return modifiedClass;
  };

  onTouchHandler = (e) => {
    const { isModalDialog } = this.props;
    !isModalDialog && e.preventDefault();
    this.backdropRef.current.click();
  };

  render() {
    const { needBackdrop, needBackground } = this.state;
    const { isAside, visible, theme } = this.props;

    const modifiedClassName = this.modifyClassName();

    return visible && (needBackdrop || isAside) ? (
      <StyledBackdrop
        theme={theme}
        {...this.props}
        ref={this.backdropRef}
        className={modifiedClassName}
        needBackground={needBackground}
        visible={visible}
        onTouchMove={this.onTouchHandler}
        onTouchEnd={this.onTouchHandler}
      />
    ) : null;
  }
}

Backdrop.propTypes = {
  /** Display or not */
  visible: PropTypes.bool,
  /** CSS z-index */
  zIndex: PropTypes.number,
  /** Accepts class */
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** The background is not displayed if the viewport width is less than 1024,
   * set it to true for display */
  withBackground: PropTypes.bool,
  /** Must be true if used with Aside component */
  isAside: PropTypes.bool,
};

Backdrop.defaultProps = {
  visible: false,
  zIndex: 200,
  withBackground: false,
  isAside: false,
  isModalDialog: false,
};

export default Backdrop;
