import React from "react";
import PropTypes from "prop-types";

import {
  ModalBackdrop,
  ModalContentWrapper,
  ModalLoader,
} from "./modal-aside-components";
import Heading from "../heading";
import { getModalType } from "../utils/device";
import throttle from "lodash/throttle";
import Box from "../box";
import {
  StyledModal,
  CloseButton,
  StyledHeader,
  Content,
  BodyBox,
  StyledFooter,
} from "./styled-modal-dialog";
import Portal from "../portal";

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
    return getModalType();
  }

  resize() {
    if (this.props.displayType !== "auto") return;
    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;
    this.setState({ displayType: type });
    this.props.onResize && this.props.onResize(type);
  }

  popstate() {
    window.removeEventListener("popstate", this.popstate, false);
    this.props.onClose();
    window.history.go(1);
  }

  componentDidUpdate(prevProps) {
    // if (this.props.displayType !== prevProps.displayType) {
    //   this.setState({ displayType: this.getTypeByWidth() });
    // }
    // if (this.props.visible && this.state.displayType === "aside") {
    //   window.addEventListener("popstate", this.popstate, false);
    // }
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
      contentHeight,
      contentWidth,
      className,
      id,
      style,
      children,
      isLoading,
      contentPaddingBottom,
      removeScroll,
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
      return (
        <StyledModal>
          <ModalBackdrop
            displayType={this.state.displayType}
            visible={visible}
            zIndex={zIndex}
          >
            <ModalContentWrapper
              displayType={this.state.displayType}
              visible={visible}
              scale={scale}
              zIndex={zIndex}
              contentPaddingBottom={contentPaddingBottom}
              withoutBodyScroll={removeScroll}
              className={className}
              id={id}
              style={style}
            >
              <Content
                contentHeight={contentHeight}
                contentWidth={contentWidth}
                displayType={this.state.displayType}
              >
                {isLoading ? (
                  <ModalLoader
                    displayType={this.state.displayType}
                    bodyHeight={modalLoaderBodyHeight}
                  />
                ) : (
                  <>
                    <StyledHeader>
                      <Heading
                        level={1}
                        className={
                          this.state.displayType === "modal"
                            ? "heading"
                            : "heading heading-aside"
                        }
                        size="medium"
                        truncate={true}
                      >
                        {header ? header.props.children : null}
                        <CloseButton
                          displayType={this.state.displayType}
                          className="modal-dialog-button_close"
                          onClick={() => onClose()}
                        />
                      </Heading>
                    </StyledHeader>
                    <BodyBox
                      className={
                        this.state.displayType === "aside" &&
                        "modal-dialog-aside-body bodybox-aside"
                      }
                      paddingProp={"0 16px"}
                      removeScroll={removeScroll}
                    >
                      {body ? body.props.children : null}
                    </BodyBox>
                    <Box
                      className={
                        this.state.displayType === "aside" &&
                        "modal-dialog-aside-footer footer-aside"
                      }
                    >
                      <StyledFooter>
                        {footer ? footer.props.children : null}
                      </StyledFooter>
                    </Box>
                  </>
                )}
              </Content>
            </ModalContentWrapper>
          </ModalBackdrop>
        </StyledModal>
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
  onResize: PropTypes.func,
  /** CSS z-index */
  zIndex: PropTypes.number,
  contentHeight: PropTypes.string,
  isLoading: PropTypes.bool,
  removeScroll: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  contentPaddingBottom: PropTypes.string,
  modalLoaderBodyHeight: PropTypes.string,
};

ModalDialog.defaultProps = {
  displayType: "auto",
  zIndex: 310,
};

ModalDialog.Header = Header;
ModalDialog.Body = Body;
ModalDialog.Footer = Footer;

export default ModalDialog;
