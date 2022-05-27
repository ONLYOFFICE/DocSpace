import React from "react";
import { cssTransition } from "react-toastify";

import PropTypes from "prop-types";
import StyledToastContainer from "./styled-toast";

const Slide = cssTransition({
  enter: "SlideIn",
  exit: "SlideOut",
});

const Toast = (props) => {
  const onToastClick = () => {
    let documentElement = document.getElementsByClassName("Toastify__toast");
    if (documentElement.length > 1)
      for (var i = 0; i < documentElement.length; i++) {
        documentElement &&
          documentElement[i].style.setProperty("position", "static");
      }
  };

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
    />
  );
};

Toast.propTypes = {
  autoClosed: PropTypes.bool,
  /** Accepts class  */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  text: PropTypes.string,
  /** Title inside a toast */
  title: PropTypes.string,
  /** Define color and icon of toast */
  type: PropTypes.oneOf(["success", "error", "warning", "info"]).isRequired,
};

Toast.defaultProps = {
  autoClosed: true,
  text: "Demo text for example",
  title: "Demo title",
  type: "success",
};

export default Toast;
