import React from "react";
import PropTypes from "prop-types";

import throttle from "lodash/throttle";

import Portal from "../portal";
import Modal from "./views/modal";
import ModalAside from "./views/modalAside";
import { handleTouchMove, handleTouchStart } from "./handlers/swipeHandler";
import {
  getCurrentDisplayType,
  getTypeByWidth,
  popstate,
} from "./handlers/resizeHandler";

function Header() {
  return null;
}
Header.displayName = "DialogHeader";

function Body() {
  return null;
}
Body.displayName = "DialogBody";

function Footer() {
  return null;
}
Footer.displayName = "DialogFooter";

class ModalDialog extends React.Component {
  static Header = Header;
  static Body = Body;
  static Footer = Footer;

  constructor(props) {
    super(props);
    this.state = {
      displayType: getTypeByWidth(this.props.displayType),
      modalSwipeOffset: 0,
    };

    this.onResize = this.onResize.bind(this);
    this.onSwipe = this.onSwipe.bind(this);
    this.onSwipeEnd = this.onSwipeEnd.bind(this);
    this.onKeyPress = this.onKeyPress.bind(this);
  }

  componentDidMount() {
    window.addEventListener("resize", this.onResize);
    window.addEventListener("keyup", this.onKeyPress);
    window.addEventListener("touchstart", handleTouchStart);
    window.addEventListener("touchmove", this.onSwipe);
    window.addEventListener("touchend", this.onSwipeEnd);
  }

  componentDidUpdate(prevProps) {
    if (this.props.displayType !== prevProps.displayType)
      this.setState({
        ...this.state,
        displayType: getTypeByWidth(this.props.displayType),
      });

    if (!this.props.visible && prevProps.visible) {
      this.setState({
        ...this.state,
        modalSwipeOffset: 0,
      });
    }

    if (this.props.visible && this.state.displayType === "aside")
      window.addEventListener("popstate", popstate, false);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.onResize);
    window.removeEventListener("keyup", this.onKeyPress);
    window.removeEventListener("touchstart", handleTouchStart);
    window.removeEventListener("touchmove", this.onSwipe);
    window.addEventListener("touchend", this.onSwipeEnd);
  }

  onResize = throttle(() => {
    const newType = getCurrentDisplayType(
      this.state.displayType,
      this.props.displayType,
      this.props.onResize
    );
    if (newType) this.setState({ ...this.state, displayType: newType });
  }, 300);

  onSwipe = (e) => {
    this.setState({
      ...this.state,
      modalSwipeOffset: handleTouchMove(e, this.props.onClose),
    });
  };

  onSwipeEnd = () => {
    this.setState({
      ...this.state,
      modalSwipeOffset: 0,
    });
  };

  onKeyPress = (e) => {
    if ((e.key === "Esc" || e.key === "Escape") && this.props.visible)
      this.props.onClose();
  };

  render() {
    const {
      visible,
      onClose,
      isLarge,
      zIndex,
      className,
      id,
      style,
      children,
      isLoading,
      modalLoaderBodyHeight,
    } = this.props;

    let header = null,
      body = null,
      footer = null;

    React.Children.forEach(children, (child) => {
      const childType =
        child && child.type && (child.type.displayName || child.type.name);

      switch (childType) {
        case Header.displayName:
          header = child;
          break;
        case Body.displayName:
          body = child;
          break;
        case Footer.displayName:
          footer = child;
          break;
        default:
          break;
      }
    });

    const renderModal = () => {
      return this.state.displayType === "modal" ? (
        <Modal
          id={id}
          style={style}
          className={className}
          isLarge={isLarge}
          zIndex={zIndex}
          fadeType={this.state.fadeType}
          onClose={onClose}
          modalLoaderBodyHeight={modalLoaderBodyHeight}
          isLoading={isLoading}
          header={header}
          body={body}
          footer={footer}
          visible={visible}
          modalSwipeOffset={this.state.modalSwipeOffset}
        />
      ) : (
        <ModalAside
          id={id}
          style={style}
          className={className}
          isLarge={isLarge}
          zIndex={zIndex}
          visible={visible}
          onClose={onClose}
          isLoading={isLoading}
          header={header}
          body={body}
          footer={footer}
        />
      );
    };

    const modalDialog = renderModal();
    return <Portal element={modalDialog} />;
  }
}

ModalDialog.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  zIndex: PropTypes.number,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  children: PropTypes.any,

  /** Display dialog or not */
  visible: PropTypes.bool,
  /** Will be triggered when a close button is clicked */
  onClose: PropTypes.func,
  /** Will be triggered on resize if `displayType === auto` */
  onResize: PropTypes.func,

  /** Display type */
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  /** Sets `width: 520px` and `max-hight: 400px`*/
  isLarge: PropTypes.bool,

  /** Show loader in body */
  isLoading: PropTypes.bool,
  /** Set loader height */
  modalLoaderBodyHeight: PropTypes.string,
};

ModalDialog.defaultProps = {
  displayType: "auto",
  zIndex: 310,
  isLarge: false,
  withoutCloseButton: false,
  withoutBodyScroll: false,
};

ModalDialog.Header = Header;
ModalDialog.Body = Body;
ModalDialog.Footer = Footer;

export default ModalDialog;
