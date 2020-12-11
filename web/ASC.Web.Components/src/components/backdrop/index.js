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
      />
    ) : null;
  }
}

Backdrop.propTypes = {
  visible: PropTypes.bool,
  zIndex: PropTypes.number,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
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
