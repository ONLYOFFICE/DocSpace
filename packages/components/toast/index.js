import React, { useCallback, useEffect, useState } from "react";
import { cssTransition } from "react-toastify";

import PropTypes from "prop-types";
import StyledToastContainer from "./styled-toast";
import { isMobileOnly } from "react-device-detect";

const Slide = cssTransition({
  enter: "SlideIn",
  exit: "SlideOut",
});

const Toast = (props) => {
  const [offset, setOffset] = useState(0);

  const onToastClick = () => {
    let documentElement = document.getElementsByClassName("Toastify__toast");
    if (documentElement.length > 1)
      for (var i = 0; i < documentElement.length; i++) {
        documentElement &&
          documentElement[i].style.setProperty("position", "static");
      }
  };

  const onResize = useCallback((event) => {
    const topOffset = event.target.innerHeight - window.visualViewport.height;

    setOffset(topOffset);
  }, []);

  useEffect(() => {
    if (isMobileOnly) {
      window.addEventListener("resize", onResize);
    }

    return () => {
      window.removeEventListener("resize", onResize);
    };
  }, []);

  return (
    <StyledToastContainer
      theme={props.theme}
      className={props.className}
      draggable={true}
      position="top-right"
      hideProgressBar={true}
      id={props.id}
      newestOnTop={true}
      pauseOnFocusLoss={false}
      style={props.style}
      transition={Slide}
      onClick={onToastClick}
      $topOffset={offset}
    />
  );
};

Toast.propTypes = {
  /** Accepts class  */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Title inside a toast */
  title: PropTypes.string,
  /** Sets the color and icon of the toast */
  type: PropTypes.oneOf(["success", "error", "warning", "info"]).isRequired,
};

Toast.defaultProps = {
  title: "Demo title",
  type: "success",
};

export default Toast;
