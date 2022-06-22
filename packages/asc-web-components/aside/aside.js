import React from "react";
import PropTypes from "prop-types";

import Scrollbar from "../scrollbar";
import {
  StyledAside,
  StyledControlContainer,
  StyledCrossIcon,
} from "./styled-aside";

const Aside = React.memo((props) => {
  //console.log("Aside render");
  const {
    visible,
    children,
    scale,
    zIndex,
    className,
    contentPaddingBottom,
    withoutBodyScroll,
    onClose,
  } = props;

  return (
    <StyledAside
      visible={visible}
      scale={scale}
      zIndex={zIndex}
      contentPaddingBottom={contentPaddingBottom}
      className={`${className} not-selectable aside`}
    >
      {withoutBodyScroll ? (
        children
      ) : (
        <Scrollbar stype="mediumBlack">{children}</Scrollbar>
      )}

      {visible && (
        <StyledControlContainer onClick={onClose}>
          <StyledCrossIcon />
        </StyledControlContainer>
      )}
    </StyledAside>
  );
});

Aside.displayName = "Aside";

Aside.propTypes = {
  visible: PropTypes.bool,
  scale: PropTypes.bool,
  className: PropTypes.string,
  contentPaddingBottom: PropTypes.string,
  zIndex: PropTypes.number,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  withoutBodyScroll: PropTypes.bool,
  onClose: PropTypes.func,
};
Aside.defaultProps = {
  scale: false,
  zIndex: 400,
  withoutBodyScroll: false,
};

export default Aside;
