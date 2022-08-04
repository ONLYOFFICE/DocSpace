import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";

import throttle from "lodash/throttle";

import Portal from "../portal";
import ModalAside from "./views/modal-aside";
import { handleTouchMove, handleTouchStart } from "./handlers/swipeHandler";
import { getCurrentDisplayType } from "./handlers/resizeHandler";
import { parseChildren } from "./handlers/childrenParseHandler";

const Header = () => null;
Header.displayName = "DialogHeader";

const Body = () => null;
Body.displayName = "DialogBody";

const Footer = () => null;
Footer.displayName = "DialogFooter";

const ModalDialog = ({
  id,
  style,
  children,
  visible,
  onClose,
  isLarge,
  zIndex,
  className,
  displayType,
  displayTypeDetailed,
  isLoading,
  autoMaxHeight,
  autoMaxWidth,
  withBodyScroll,
  modalLoaderBodyHeight,
  withFooterBorder,
}) => {
  const [currentDisplayType, setCurrentDisplayType] = useState(
    getCurrentDisplayType(displayType, displayTypeDetailed)
  );
  const [modalSwipeOffset, setModalSwipeOffset] = useState(0);

  useEffect(() => {
    const onResize = throttle(() => {
      setCurrentDisplayType(
        getCurrentDisplayType(displayType, displayTypeDetailed)
      );
    }, 300);
    const onSwipe = (e) => setModalSwipeOffset(handleTouchMove(e, onClose));
    const onSwipeEnd = () => setModalSwipeOffset(0);
    const onKeyPress = (e) => {
      if ((e.key === "Esc" || e.key === "Escape") && visible) onClose();
    };

    window.addEventListener("resize", onResize);
    window.addEventListener("keyup", onKeyPress);
    window.addEventListener("touchstart", handleTouchStart);
    window.addEventListener("touchmove", onSwipe);
    window.addEventListener("touchend", onSwipeEnd);
    return () => {
      window.removeEventListener("resize", onResize);
      window.removeEventListener("keyup", onKeyPress);
      window.removeEventListener("touchstart", handleTouchStart);
      window.removeEventListener("touchmove", onSwipe);
      window.addEventListener("touchend", onSwipeEnd);
    };
  }, []);

  const [header, body, footer] = parseChildren(
    children,
    Header.displayName,
    Body.displayName,
    Footer.displayName
  );

  return (
    <Portal
      element={
        <ModalAside
          id={id}
          style={style}
          className={className}
          currentDisplayType={currentDisplayType}
          withBodyScroll={withBodyScroll}
          isLarge={isLarge}
          zIndex={zIndex}
          autoMaxHeight={autoMaxHeight}
          autoMaxWidth={autoMaxWidth}
          withFooterBorder={withFooterBorder}
          onClose={onClose}
          isLoading={isLoading}
          header={header}
          body={body}
          footer={footer}
          visible={visible}
          modalSwipeOffset={modalSwipeOffset}
        />
      }
    />
  );
};

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

  /** Display type */
  displayType: PropTypes.oneOf(["modal", "aside"]),
  /** Detailed display type for each dimension */
  displayTypeDetailed: PropTypes.object,

  /** Show loader in body */
  isLoading: PropTypes.bool,

  /** **`MODAL-ONLY`**  

  Sets `width: 520px` and `max-hight: 400px`*/
  isLarge: PropTypes.bool,

  /** **`MODAL-ONLY`**  

  Sets `max-width: auto`*/
  autoMaxWidth: PropTypes.bool,

  /** **`MODAL-ONLY`**  

  Sets `max-height: auto`*/
  autoMaxHeight: PropTypes.bool,

  /** **`MODAL-ONLY`**  

  Displays border betweeen body and footer`*/
  withFooterBorder: PropTypes.bool,

  /** **`ASIDE-ONLY`**  

  Enables Body scroll */
  withBodyScroll: PropTypes.bool,
  /** **`ASIDE-ONLY`**  

  Sets modal dialog size equal to window */
  scale: PropTypes.bool,
};

ModalDialog.defaultProps = {
  displayType: "modal",
  zIndex: 310,
  isLarge: false,
  isLoading: false,
  withoutCloseButton: false,
  withBodyScroll: false,
  withFooterBorder: false,
};

ModalDialog.Header = Header;
ModalDialog.Body = Body;
ModalDialog.Footer = Footer;

export default ModalDialog;
