import React from "react";
import PropTypes from "prop-types";

import Backdrop from "../../backdrop";
import Aside from "../../aside";
import Box from "../../box";
import Loaders from "@appserver/common/components/Loaders";

import Heading from "../../heading";
import {
  StyledModal,
  StyledHeader,
  Content,
  BodyBox,
  StyledFooter,
} from "../styled-modal-dialog";
import CloseButton from "../components/CloseButton";
import ModalBackdrop from "../components/ModalBackdrop";

const ModalAside = ({
  id,
  style,
  className,
  zIndex,

  visible,
  onClose,

  isLoading,

  header,
  body,
  footer,
}) => {
  if (!visible) return null;
  return (
    <StyledModal className={visible ? "modal-active" : ""}>
      <Box className={className} id={id} style={style}>
        <CloseButton displayType="aside" onClick={onClose} />
        {/* <Backdrop
          visible={true}
          zIndex={zIndex}
          withBackground={true}
          isAside={true}
        /> */}
        <ModalBackdrop
          className={"modal-backdrop-active"}
          visible={true}
          zIndex={zIndex}
          modalSwipeOffset={0}
        />
        <Aside
          scale={false}
          visible={visible}
          zIndex={zIndex}
          className="modal-dialog-aside aside-dialog not-selectable"
          withoutBodyScroll={true}
        >
          <Content displayType="aside">
            {isLoading ? (
              <Loaders.DialogAsideLoader withoutAside />
            ) : (
              <>
                <StyledHeader>
                  <Heading
                    level={1}
                    className={"heading heading-aside"}
                    size="medium"
                    truncate={true}
                  >
                    {header ? header.props.children : null}
                  </Heading>
                </StyledHeader>
                <BodyBox
                  className={"modal-dialog-aside-body bodybox-aside"}
                  paddingProp={"0 16px"}
                  withoutBodyScroll={true}
                >
                  {body ? body.props.children : null}
                </BodyBox>
                <Box className={"modal-dialog-aside-footer footer-aside"}>
                  <StyledFooter displayType="aside">
                    {footer ? footer.props.children : null}
                  </StyledFooter>
                </Box>
              </>
            )}
          </Content>
        </Aside>
      </Box>
    </StyledModal>
  );
};

ModalAside.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  zIndex: PropTypes.number,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  onClose: PropTypes.func,

  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  isLarge: PropTypes.bool,

  isLoading: PropTypes.bool,

  header: PropTypes.object,
  body: PropTypes.object,
  footer: PropTypes.object,
};

export default ModalAside;
