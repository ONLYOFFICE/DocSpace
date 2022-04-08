import React from "react";
import PropTypes from "prop-types";

import Box from "../../box";
import Loaders from "@appserver/common/components/Loaders";

import Heading from "../../heading";
import {
  StyledModal,
  StyledHeader,
  Content,
  Dialog,
  BodyBox,
  StyledFooter,
} from "../styled-modal-dialog";
import CloseButton from "../components/CloseButton";
import ModalBackdrop from "../components/ModalBackdrop";

const Modal = ({
  id,
  style,
  className,

  zIndex,
  isLarge,

  visible,
  onClose,

  modalSwipeOffset,

  isLoading,
  modalLoaderBodyHeight,

  header,
  body,
  footer,
}) => {
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
          id={id}
          style={style}
        >
          <Content
            displayType="modal"
            id="modal-dialog"
            isLarge={isLarge}
            modalSwipeOffset={modalSwipeOffset}
          >
            {isLoading ? (
              <Loaders.DialogLoader bodyHeight={modalLoaderBodyHeight} />
            ) : (
              <>
                <StyledHeader id="modal-header-swipe">
                  <Heading
                    level={1}
                    className={"heading"}
                    size="medium"
                    truncate={true}
                  >
                    {header ? header.props.children : null}
                    <CloseButton displayType={"modal"} onClick={onClose} />
                  </Heading>
                </StyledHeader>
                <BodyBox paddingProp={"0 16px"} withoutBodyScroll={true}>
                  {body ? body.props.children : null}
                </BodyBox>
                <Box>
                  <StyledFooter displayType="modal">
                    {footer ? footer.props.children : null}
                  </StyledFooter>
                </Box>
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

  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  isLarge: PropTypes.bool,

  header: PropTypes.object,
  body: PropTypes.object,
  footer: PropTypes.object,
};

export default Modal;
