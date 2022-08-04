import React from "react";
import PropTypes from "prop-types";

import Loaders from "@docspace/common/components/Loaders";

import Heading from "../../heading";
import {
  StyledModal,
  StyledHeader,
  Content,
  Dialog,
  StyledBody,
  StyledFooter,
} from "../styled-modal-dialog";
import CloseButton from "../components/CloseButton";
import ModalBackdrop from "../components/ModalBackdrop";
import Scrollbar from "../../scrollbar";

const Modal = ({
  id,
  style,
  className,
  currentDisplayType,
  withBodyScroll,
  isLarge,
  zIndex,
  autoMaxHeight,
  autoMaxWidth,
  onClose,
  isLoading,
  header,
  body,
  footer,
  visible,
  withFooterBorder,
  modalSwipeOffset,
}) => {
  const headerComponent = header ? header.props.children : null;
  const bodyComponent = body ? body.props.children : null;
  const footerComponent = footer ? footer.props.children : null;

  return (
    <StyledModal
      className={visible ? "modal-active" : ""}
      modalSwipeOffset={modalSwipeOffset}
    >
      <ModalBackdrop
        className={visible ? "modal-backdrop-active" : ""}
        visible={true}
        zIndex={zIndex}
        modalSwipeOffset={modalSwipeOffset}
      >
        <Dialog
          className={`${className} dialog not-selectable`}
          currentDisplayType={currentDisplayType}
          id={id}
          style={style}
          onClick={onClose}
        >
          <Content
            id="modal-dialog"
            visible={visible}
            isLarge={isLarge}
            currentDisplayType={currentDisplayType}
            autoMaxHeight={autoMaxHeight}
            autoMaxWidth={autoMaxWidth}
            modalSwipeOffset={modalSwipeOffset}
            onClick={(e) => e.stopPropagation()}
          >
            <CloseButton
              currentDisplayType={currentDisplayType}
              onClick={onClose}
            />
            {isLoading ? (
              currentDisplayType === "modal" ? (
                <Loaders.DialogLoader
                  isLarge={isLarge}
                  withFooterBorder={withFooterBorder}
                />
              ) : (
                <Loaders.DialogAsideLoader
                  withoutAside
                  withFooterBorder={withFooterBorder}
                />
              )
            ) : (
              <>
                {header && (
                  <StyledHeader
                    id="modal-header-swipe"
                    currentDisplayType={currentDisplayType}
                    {...header.props}
                  >
                    <Heading
                      level={1}
                      className={"heading"}
                      size="medium"
                      truncate={true}
                    >
                      {headerComponent}
                    </Heading>
                  </StyledHeader>
                )}
                {body && (
                  <StyledBody
                    withBodyScroll={withBodyScroll}
                    hasFooter={1 && footer}
                    currentDisplayType={currentDisplayType}
                    {...body.props}
                  >
                    {currentDisplayType === "aside" && withBodyScroll ? (
                      <Scrollbar stype="mediumBlack">{bodyComponent}</Scrollbar>
                    ) : (
                      bodyComponent
                    )}
                  </StyledBody>
                )}
                {footer && (
                  <StyledFooter
                    withFooterBorder={withFooterBorder}
                    currentDisplayType={currentDisplayType}
                    {...footer.props}
                  >
                    {footerComponent}
                  </StyledFooter>
                )}
              </>
            )}
          </Content>
        </Dialog>
      </ModalBackdrop>
    </StyledModal>
  );
};

Modal.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  zIndex: PropTypes.number,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  onClose: PropTypes.func,
  visible: PropTypes.bool,
  modalSwipeOffset: PropTypes.number,

  isLoading: PropTypes.bool,
  modalLoaderBodyHeight: PropTypes.string,

  currentDisplayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  isLarge: PropTypes.bool,

  header: PropTypes.object,
  body: PropTypes.object,
  footer: PropTypes.object,
};

export default Modal;
