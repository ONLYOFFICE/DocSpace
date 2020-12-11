import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

const StyledBackdrop = styled.div`
  background-color: ${(props) =>
    props.needBackground ? "rgba(6, 22, 38, 0.1)" : "unset"};
  display: ${(props) => (props.visible ? "block" : "none")};
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: ${(props) => props.zIndex};
  left: 0;
  top: 0;
  cursor: ${(props) => (props.needBackground ? "pointer" : "default")}; ;
`;

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
      const backdropsAside = document.querySelectorAll(".backdrop-aside");

      const needBackdrop = backdrops.length < 1 || isAside;

      let needBackground =
        needBackdrop && (isTablet || isAside || withBackground);

      if (!isAside && backdropsAside.length > 0) needBackground = false;
      if (isAside && backdropsAside.length > 1) needBackground = false;

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

  onClickHandler = (e) => {
    if (this.backdropRef.current.classList.contains("backdrop-aside")) {
      const backdrops = document.querySelectorAll(".backdrop-active");
      if (backdrops.length > 0) {
        backdrops.forEach((item) => item.click());
      }
    }
    this.props.onClick && this.props.onClick(e);
  };

  render() {
    const { needBackdrop, needBackground } = this.state;
    const { className, isAside, visible } = this.props;

    const classNameStr = isAside
      ? className
        ? `backdrop-active backdrop-aside ${className}`
        : "backdrop-active backdrop-aside"
      : className
      ? `backdrop-active ${className}`
      : "backdrop-active";

    return visible && (needBackdrop || isAside) ? (
      <StyledBackdrop
        {...this.props}
        ref={this.backdropRef}
        className={classNameStr}
        needBackground={needBackground}
        visible={visible}
        onClick={this.onClickHandler}
      />
    ) : null;
  }
}

Backdrop.propTypes = {
  visible: PropTypes.bool,
  zIndex: PropTypes.number,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  withBackground: PropTypes.bool,
  isAside: PropTypes.bool,
};

Backdrop.defaultProps = {
  visible: false,
  zIndex: 200,
  withBackground: false,
  isAside: false,
};

export default Backdrop;
