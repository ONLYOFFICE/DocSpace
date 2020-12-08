import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

const StyledBackdrop = styled.div`
  background-color: ${(props) =>
    props.withBackdrop && props.needBackground
      ? "rgba(6, 22, 38, 0.1)"
      : "unset"};
  display: ${(props) => (props.visible ? "block" : "none")};
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: ${(props) => props.zIndex};
  left: 0;
  top: 0;
  cursor: ${(props) => (props.withBackdrop ? "pointer" : "default")}; ;
`;

class Backdrop extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      needBackground: true,
    };
  }

  componentDidUpdate(prevProps) {
    if (prevProps.visible !== this.props.visible) {
      const needBackground =
        document.querySelectorAll(".backdrop-active").length <= 1;
      this.setState({ needBackground: needBackground });
    }
  }

  onClickHandler = (e) => {
    const { target, clientX, clientY } = e;
    const nearOpenDropdown = target.closest("[open]");
    const nearAside = target.closest(".aside-container");

    if ((nearOpenDropdown || nearAside) && (clientX !== 0 || clientY !== 0)) {
      let rects;
      if (nearOpenDropdown) {
        rects = nearOpenDropdown.getBoundingClientRect();
      }
      if (nearAside) {
        rects = nearAside.getBoundingClientRect();
      }
      const { x, y, width, height } = rects;

      if (
        clientX < x ||
        clientX > x + width ||
        clientY < y ||
        clientY > y + height
      ) {
        const backdrops = document.querySelectorAll(".backdrop-active");
        backdrops.forEach((item) => item.click());
      }
    }

    this.props.onClick && this.props.onClick(e);
  };

  render() {
    const { needBackground } = this.state;
    const { className } = this.props;

    const classNameStr = this.props.visible
      ? this.props.className
        ? `backdrop-active ${className}`
        : "backdrop-active"
      : this.props.className
      ? `backdrop-inactive ${className}`
      : "backdrop-inactive";

    return (
      <StyledBackdrop
        className={classNameStr}
        {...this.props}
        needBackground={needBackground}
        onClick={this.onClickHandler}
      />
    );
  }
}

Backdrop.propTypes = {
  visible: PropTypes.bool,
  zIndex: PropTypes.number,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  withBackdrop: PropTypes.bool,
};

Backdrop.defaultProps = {
  visible: false,
  zIndex: 200,
  withBackdrop: true,
};

export default Backdrop;
