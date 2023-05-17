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
    const { visible, isAside, withBackground, withoutBlur } = this.props;
    if (visible) {
      const isTablet = window.innerWidth < 1024;
      const backdrops = document.querySelectorAll(".backdrop-active");

      const needBackdrop =
        backdrops.length < 1 || (isAside && backdrops.length <= 2);

      let needBackground =
        needBackdrop && ((isTablet && !withoutBlur) || withBackground);

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
    let modifiedClass = "backdrop-active not-selectable";

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
    const { isAside, visible } = this.props;

    const modifiedClassName = this.modifyClassName();

    return visible && (needBackdrop || isAside) ? (
      <StyledBackdrop
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
  /** Sets visible or hidden */
  visible: PropTypes.bool,
  /** CSS z-index */
  zIndex: PropTypes.number,
  /** Accepts class */
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Displays the background. *The background is not displayed if the viewport width is more than 1024 */
  withBackground: PropTypes.bool,
  /** Must be true if used with Aside component */
  isAside: PropTypes.bool,
  /** Must be true if used with Context menu */
  withoutBlur: PropTypes.bool,
};

Backdrop.defaultProps = {
  visible: false,
  zIndex: 203,
  withBackground: false,
  isAside: false,
  isModalDialog: false,
  withoutBlur: false,
};

export default Backdrop;
