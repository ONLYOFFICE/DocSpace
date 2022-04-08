import React from "react";
import PropTypes from "prop-types";

import { getModalType, isSmallTablet, isTouchDevice } from "../utils/device";
import throttle from "lodash/throttle";

import Portal from "../portal";
import Modal from "./views/modal";
import ModalAside from "./views/modalAside";
import { handleTouchMove, handleTouchStart } from "./handlers/swipeHandler";

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
      displayType: this.getTypeByWidth(),
      modalSwipeOffset: 0,
    };

    this.getTypeByWidth = this.getTypeByWidth.bind(this);
    this.resize = this.resize.bind(this);
    this.throttledResize = throttle(this.resize, 300);
    this.popstate = this.popstate.bind(this);
  }

  getTypeByWidth() {
    if (this.props.displayType !== "auto") return this.props.displayType;
    return getModalType();
  }

  resize() {
    const newType = this.getTypeByWidth();
    if (newType === this.state.displayType) return;

    this.setState({ ...this.state, displayType: newType });
    this.props.onResize && this.props.onResize(newType);
  }

  popstate() {
    window.removeEventListener("popstate", this.popstate, false);
    this.props.onClose();
    window.history.go(1);
  }

  componentDidUpdate(prevProps) {
    if (this.props.displayType !== prevProps.displayType)
      this.setState({ ...this.state, displayType: this.getTypeByWidth() });
    if (this.props.visible && this.state.displayType === "aside")
      window.addEventListener("popstate", this.popstate, false);

    if (!this.props.visible && this.state.modalSwipeOffset) {
      this.setState({
        ...this.state,
        modalSwipeOffset: 0,
      });
    }
  }

  componentDidMount() {
    if (this.props.displayType === "auto")
      window.addEventListener("resize", this.throttledResize);
    window.addEventListener("keyup", this.onKeyPress);

    window.addEventListener("touchstart", handleTouchStart, false);
    window.addEventListener(
      "touchmove",
      (e) => {
        this.setState({
          ...this.state,
          modalSwipeOffset: handleTouchMove(e, this.props.onClose),
        });
      },
      false
    );
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
    window.removeEventListener("keyup", this.onKeyPress);
    window.removeEventListener("touchstart", handleTouchMove);
    window.removeEventListener("touchmove", handleTouchMove);
  }

  onKeyPress = (e) => {
    if (e.key === "Esc" || e.key === "Escape") this.props.onClose();
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
      if (!visible) return null;
      return this.state.displayType === "modal" ? (
        <Modal
          id={id}
          style={style}
          className={className}
          isLarge={isLarge}
          zIndex={zIndex}
          onClose={onClose}
          modalLoaderBodyHeight={modalLoaderBodyHeight}
          isLoading={isLoading}
          header={header}
          body={body}
          footer={footer}
          modalSwipeOffset={this.state.modalSwipeOffset}
        />
      ) : (
        <ModalAside
          id={id}
          style={style}
          className={className}
          isLarge={isLarge}
          zIndex={zIndex}
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
  /** If true sets width to 520px && maxHeight to 400px*/
  isLarge: PropTypes.bool,

  isLoading: PropTypes.bool,
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
