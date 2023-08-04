import React, { useEffect, useState, useCallback } from "react";
import PropTypes from "prop-types";

import throttle from "lodash/throttle";

import Portal from "../portal";
import ModalAside from "./views/modal-aside";
import { handleTouchMove, handleTouchStart } from "./handlers/swipeHandler";
import { getCurrentDisplayType } from "./handlers/resizeHandler";
import { parseChildren } from "./handlers/childrenParseHandler";
import { isSafari, isTablet } from "react-device-detect";

const Header = () => null;
Header.displayName = "DialogHeader";

const Body = () => null;
Body.displayName = "DialogBody";

const Footer = () => null;
Footer.displayName = "DialogFooter";

const Container = () => null;
Container.displayName = "DialogContainer";

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
  isScrollLocked,
  containerVisible,
  isDoubleFooterLine,
  isCloseable,
  embedded,
}) => {
  const onCloseEvent = () => {
    if (embedded) return;
    isCloseable && onClose();
  };
  const [currentDisplayType, setCurrentDisplayType] = useState(
    getCurrentDisplayType(displayType, displayTypeDetailed)
  );
  const [modalSwipeOffset, setModalSwipeOffset] = useState(0);
  const returnWindowPositionAfterKeyboard = () => {
    isSafari && isTablet && window.scrollY !== 0 && window.scrollTo(0, 0);
  };
  useEffect(() => {
    const onResize = throttle(() => {
      setCurrentDisplayType(
        getCurrentDisplayType(displayType, displayTypeDetailed)
      );
    }, 300);
    const onSwipe = (e) => setModalSwipeOffset(handleTouchMove(e, onClose));
    const onSwipeEnd = () => setModalSwipeOffset(0);
    const onKeyPress = (e) => {
      if ((e.key === "Esc" || e.key === "Escape") && visible) onCloseEvent();
    };

    window.addEventListener("resize", onResize);
    window.addEventListener("keyup", onKeyPress);
    window.addEventListener("touchstart", handleTouchStart);
    window.addEventListener("touchmove", onSwipe);
    window.addEventListener("touchend", onSwipeEnd);
    return () => {
      returnWindowPositionAfterKeyboard();

      window.removeEventListener("resize", onResize);
      window.removeEventListener("keyup", onKeyPress);
      window.removeEventListener("touchstart", handleTouchStart);
      window.removeEventListener("touchmove", onSwipe);
      window.addEventListener("touchend", onSwipeEnd);
    };
  }, []);

  const [header, body, footer, container] = parseChildren(
    children,
    Header.displayName,
    Body.displayName,
    Footer.displayName,
    Container.displayName
  );

  return (
    <Portal
      element={
        <ModalAside
          isDoubleFooterLine={isDoubleFooterLine}
          id={id}
          style={style}
          className={className}
          currentDisplayType={currentDisplayType}
          withBodyScroll={withBodyScroll}
          isScrollLocked={isScrollLocked}
          isLarge={isLarge}
          zIndex={zIndex}
          autoMaxHeight={autoMaxHeight}
          autoMaxWidth={autoMaxWidth}
          withFooterBorder={withFooterBorder}
          onClose={onCloseEvent}
          isLoading={isLoading}
          header={header}
          body={body}
          footer={footer}
          container={container}
          visible={visible}
          modalSwipeOffset={modalSwipeOffset}
          containerVisible={containerVisible}
          isCloseable={isCloseable && !embedded}
          embedded={embedded}
        />
      }
    />
  );
};

ModalDialog.propTypes = {
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** CSS z-index   */
  zIndex: PropTypes.number,
  /** Accepts css */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Displays the child elements */
  children: PropTypes.any,

  /** Sets the dialog to display */
  visible: PropTypes.bool,
  /** Sets a callback function that is triggered when the close button is clicked */
  onClose: PropTypes.func,

  /** Displays type */
  displayType: PropTypes.oneOf(["modal", "aside"]),
  /** Detailed display type for each dimension */
  displayTypeDetailed: PropTypes.object,

  /** Shows loader in body */
  isLoading: PropTypes.bool,

  /** Sets the displayed dialog to be closed or open */
  isCloseable: PropTypes.bool,

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

  Enables body scroll */
  isScrollLocked: PropTypes.bool,

  /** **`ASIDE-ONLY`**  

  Sets modal dialog size equal to window */
  scale: PropTypes.bool,
  /** **`ASIDE-ONLY`**  

  Allows you to embed a modal window as an aside dialog inside the parent container without applying a dialog layout to it */
  containerVisible: PropTypes.bool,
};

ModalDialog.defaultProps = {
  displayType: "modal",
  zIndex: 310,
  isLarge: false,
  isLoading: false,
  isCloseable: true,
  withBodyScroll: false,
  withFooterBorder: false,
  containerVisible: false,
};

ModalDialog.Header = Header;
ModalDialog.Body = Body;
ModalDialog.Footer = Footer;
ModalDialog.Container = Container;

export default ModalDialog;
