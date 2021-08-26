import React from "react";
import PropTypes from "prop-types";

import Scrollbar from "../scrollbar";
import StyledAside from "./styled-aside";

const Aside = React.memo((props) => {
  //console.log("Aside render");
  const {
    visible,
    children,
    scale,
    zIndex,
    className,
    contentPaddingBottom,
  } = props;

  return (
    <StyledAside
      visible={visible}
      scale={scale}
      zIndex={zIndex}
      contentPaddingBottom={contentPaddingBottom}
      className={`${className} not-selectable aside`}
    >
      <Scrollbar>{children}</Scrollbar>
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
};
Aside.defaultProps = {
  scale: false,
  zIndex: 400,
};

export default Aside;
