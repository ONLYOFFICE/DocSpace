import React from "react";
import PropTypes from "prop-types";

import Backdrop from "../backdrop";
import Aside from "../aside";
import Box from "../box";
import { Dialog } from "./styled-modal-dialog";
import Loaders from "@appserver/common/components/Loaders";

export const ModalBackdrop = ({ displayType, visible, zIndex, children }) => {
  console.log(visible);
  return (
    <>
      {displayType === "modal" ? (
        <Backdrop
          visible={visible}
          zIndex={zIndex}
          withBackground={true}
          isModalDialog
          className="backdrop"
        >
          {children}
        </Backdrop>
      ) : (
        <Backdrop
          visible={visible}
          zIndex={zIndex}
          isAside={true}
          className="backdrop"
        >
          {children}
        </Backdrop>
      )}
    </>
  );
};

export const ModalContentWrapper = ({
  displayType,
  className,
  id,
  style,
  visible,
  scale,
  zIndex,
  removeScroll,
  contentPaddingBottom,
  children,
}) => {
  return (
    <>
      {displayType === "modal" ? (
        <Dialog
          className={`${className} dialog not-selectable`}
          id={id}
          style={style}
        >
          {children}
        </Dialog>
      ) : (
        <Box className={className} id={id} style={style}>
          <Aside
            visible={visible}
            scale={scale}
            zIndex={zIndex}
            contentPaddingBottom={contentPaddingBottom}
            className="modal-dialog-aside not-selectable"
            withoutBodyScroll={removeScroll}
          >
            {children}
          </Aside>
        </Box>
      )}
    </>
  );
};

export const ModalLoader = ({ displayType, modalLoaderBodyHeight }) => {
  return (
    <>
      {displayType === "modal" ? (
        <Loaders.DialogLoader bodyHeight={modalLoaderBodyHeight} />
      ) : (
        <Loaders.DialogAsideLoader withoutAside />
      )}
    </>
  );
};

ModalBackdrop.propTypes = {
  children: PropTypes.any,
  visible: PropTypes.bool,
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  zIndex: PropTypes.number,
};

ModalContentWrapper.propTypes = {
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  children: PropTypes.any,
  visible: PropTypes.bool,
  zIndex: PropTypes.number,
  scale: PropTypes.bool,
  removeScroll: PropTypes.bool,
  contentPaddingBottom: PropTypes.string,
};

ModalLoader.propTypes = {
  displayType: PropTypes.oneOf(["auto", "modal", "aside"]),
  modalLoaderBodyHeight: PropTypes.string,
};
