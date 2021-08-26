import React from "react";
import PropTypes from "prop-types";

import Backdrop from "../backdrop";
import Aside from "../aside";
import Heading from "../heading";
import { desktop } from "../utils/device";
import throttle from "lodash/throttle";
import Box from "../box";
import {
  CloseButton,
  StyledHeader,
  Content,
  Dialog,
  BodyBox,
} from "./styled-modal-dialog";
import Portal from "../portal";
import Loaders from "@appserver/common/components/Loaders";

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
  constructor(props) {
    super(props);

    this.state = { displayType: this.getTypeByWidth() };

    this.getTypeByWidth = this.getTypeByWidth.bind(this);
    this.resize = this.resize.bind(this);
    this.popstate = this.popstate.bind(this);
    this.throttledResize = throttle(this.resize, 300);
  }

  getTypeByWidth() {
    if (this.props.displayType !== "auto") return this.props.displayType;

    return window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "modal";
  }

  resize() {
    if (this.props.displayType !== "auto") return;

    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;

    this.setState({ displayType: type });
  }

  popstate() {
    window.removeEventListener("popstate", this.popstate, false);
    this.props.onClose();
    window.history.go(1);
  }

  componentDidUpdate(prevProps) {
    if (this.props.displayType !== prevProps.displayType) {
      this.setState({ displayType: this.getTypeByWidth() });
    }
    if (this.props.visible && this.state.displayType === "aside") {
      window.addEventListener("popstate", this.popstate, false);
    }
  }

  componentDidMount() {
    window.addEventListener("resize", this.throttledResize);
    window.addEventListener("keyup", this.onKeyPress);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
    window.removeEventListener("keyup", this.onKeyPress);
  }

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.props.onClose();
    }
  };

  render() {
    const {
      visible,
      scale,
      onClose,
      zIndex,
      bodyPadding,
      contentHeight,
      contentWidth,
      className,
      id,
      style,
      children,
      isLoading,
      contentPaddingBottom,
      removeScroll,
    } = this.props;

    let header = null;
    let body = null;
    let footer = null;

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
        <Backdrop
          visible={visible}
          zIndex={zIndex}
          withBackground={true}
          isModalDialog
        >
          <Dialog
            className={`${className} not-selectable`}
            id={id}
            style={style}
          >
            <Content contentHeight={contentHeight} contentWidth={contentWidth}>
              {isLoading ? (
                <Loaders.DialogLoader />
              ) : (
                <>
                  <StyledHeader>
                    <Heading className="heading" size="medium" truncate={true}>
                      {header ? header.props.children : null}
                    </Heading>
                    <CloseButton onClick={onClose}></CloseButton>
                  </StyledHeader>
                  <BodyBox paddingProp={bodyPadding}>
                    {body ? body.props.children : null}
                  </BodyBox>
                  <Box>{footer ? footer.props.children : null}</Box>
                </>
              )}
            </Content>
          </Dialog>
        </Backdrop>
      ) : (
        <Box className={className} id={id} style={style}>
          <Backdrop
            visible={visible}
            onClick={onClose}
            zIndex={zIndex}
            isAside={true}
          />
          <Aside
            visible={visible}
            scale={scale}
            zIndex={zIndex}
            contentPaddingBottom={contentPaddingBottom}
            className="modal-dialog-aside not-selectable"
          >
            <Content
              contentHeight={contentHeight}
              contentWidth={contentWidth}
              removeScroll={removeScroll}
            >
              {isLoading ? (
                <Loaders.DialogAsideLoader withoutAside />
              ) : (
                <>
                  <StyledHeader className="modal-dialog-aside-header">
                    <Heading className="heading" size="medium" truncate={true}>
                      {header ? header.props.children : null}
                    </Heading>
                    {scale ? <CloseButton onClick={onClose}></CloseButton> : ""}
                  </StyledHeader>
                  <BodyBox
                    className="modal-dialog-aside-body"
                    paddingProp={bodyPadding}
                    removeScroll={removeScroll}
                  >
                    {body ? body.props.children : null}
                  </BodyBox>
                  <Box className="modal-dialog-aside-footer">
                    {footer ? footer.props.children : null}
                  </Box>
                </>
              )}
            </Content>
          </Aside>
        </Box>
      );
    };

    const modalDialog = renderModal();

    return <Portal element={modalDialog} />;
  }
}

ModalDialog.propTypes = {
  children: PropTypes.any,
  /** Display dialog or not */
  visible: PropTypes.bool,
  /** Display type */
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  /** Indicates the side panel has scale */
  scale: PropTypes.bool,
  /** Will be triggered when a close button is clicked */
  onClose: PropTypes.func,
  /** CSS z-index */
  zIndex: PropTypes.number,
  /** CSS padding props for body section */
  bodyPadding: PropTypes.string,
  contentHeight: PropTypes.string,
  contentWidth: PropTypes.string,
  isLoading: PropTypes.bool,
  removeScroll: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  contentPaddingBottom: PropTypes.string,
};

ModalDialog.defaultProps = {
  displayType: "auto",
  zIndex: 310,
  bodyPadding: "16px 0",
  contentWidth: "100%",
};

ModalDialog.Header = Header;
ModalDialog.Body = Body;
ModalDialog.Footer = Footer;

export default ModalDialog;
