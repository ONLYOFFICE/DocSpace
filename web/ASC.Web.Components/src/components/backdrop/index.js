import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

const StyledBackdrop = styled.div`
  background-color: ${(props) =>
    props.withBackdrop && !props.backdropExist
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
      backdropExist: false,
    };
  }

  componentDidUpdate(prevProps) {
    if (prevProps.visible !== this.props.visible) {
      const isExist = document.querySelectorAll("#backdrop-active").length > 1;
      this.setState({ backdropExist: isExist });
    }
  }

  render() {
    const { backdropExist } = this.state;

    return this.props.visible ? (
      <StyledBackdrop
        id="backdrop-active"
        {...this.props}
        backdropExist={backdropExist}
      />
    ) : (
      <StyledBackdrop
        id="backdrop-inactive"
        {...this.props}
        backdropExist={backdropExist}
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
