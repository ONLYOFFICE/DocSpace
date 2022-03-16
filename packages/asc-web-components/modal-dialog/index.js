import React from "react";
import PropTypes from "prop-types";

import {
  ModalBackdrop,
  ModalContentWrapper,
  ModalLoader,
} from "./modal-aside-components";

import Heading from "../heading";
import {
  StyledModal,
  CloseButton,
  StyledHeader,
  Content,
  BodyBox,
  StyledFooter,
} from "./styled-modal-dialog";

import Portal from "../portal";
import Box from "../box";
import { getModalType, isDesktop } from "../utils/device";

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
    this.state = { displayType: this.props.displayType };
    this.handleResize = this.handleResize.bind(this);
  }

  componentDidMount() {
    window.addEventListener("keyup", this.onKeyPress);
    window.addEventListener("resize", this.handleResize);
  }

  componentWillUnmount() {
    window.removeEventListener("keyup", this.onKeyPress);
    window.removeEventListener("resize", this.handleResize);
  }

  handleResize() {
    if (this.displayType === "modal" && isDesktop()) return;
    if (this.displayType === "aside" && !isDesktop()) return;
    const newDisplayType = getModalType();
    if (this.state.displayType.toString() !== newDisplayType) {
      //this.setState({ displayType: newDisplayType });
      console.log("rerender");
    }
  }

  onKeyPress = (event) => {
    if (event.key === "Esc" || event.key === "Escape") {
      this.props.onClose();
    }
  };

  render() {
    const {
      displayType,
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

    let currentDisplayType;
    if (this.props.displayType === "auto" && isDesktop())
      currentDisplayType = "modal";
    else currentDisplayType = "aside";

    console.log(visible, currentDisplayType);

    const renderModal = () => {
      if (visible !== true) return null;
      else
        return (
          <StyledModal>
            <ModalBackdrop displayType={currentDisplayType} zIndex={zIndex}>
              {displayType === "aside" && !scale && (
                <CloseButton
                  displayType={currentDisplayType}
                  className="modal-dialog-button_close"
                  onClick={onClose}
                />
              )}
              <ModalContentWrapper
                displayType={currentDisplayType}
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
                  displayType={currentDisplayType}
                >
                  {isLoading ? (
                    <ModalLoader
                      displayType={currentDisplayType}
                      bodyHeight={modalLoaderBodyHeight}
                    />
                  ) : (
                    <>
                      <StyledHeader>
                        <Heading
                          level={1}
                          className={
                            displayType === "modal"
                              ? "heading"
                              : "heading heading-aside"
                          }
                          size="medium"
                          truncate={true}
                        >
                          {header ? header.props.children : null}
                          <CloseButton
                            displayType={currentDisplayType}
                            className="modal-dialog-button_close"
                            onClick={onClose}
                          />
                        </Heading>
                      </StyledHeader>
                      <BodyBox
                        className={
                          displayType === "aside" &&
                          "modal-dialog-aside-body bodybox-aside"
                        }
                        paddingProp={"0 16px"}
                        removeScroll={removeScroll}
                      >
                        {body ? body.props.children : null}
                      </BodyBox>
                      <Box
                        className={
                          displayType === "aside" &&
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
