import React from "react";
import { ToastContainer, cssTransition } from "react-toastify";
import styled from "styled-components";
import PropTypes from "prop-types";
import { tablet } from "../../utils/device";

const Slide = cssTransition({
  enter: "SlideIn",
  exit: "SlideOut",
});

const toastColors = {
  active: {
    success: "#cae796",
    error: "#ffbfaa",
    info: "#f1da92",
    warning: "#f1ca92",
  },
  hover: {
    success: "#bcdf7e",
    error: "#ffa98d",
    info: "#eed27b",
    warning: "#eeb97b",
  },
};

const StyledToastContainer = styled(ToastContainer)`
  width: 365px;
  z-index: 9999;
  -webkit-transform: translateZ(9999px);
  position: fixed;
  padding: 4px;
  width: 320px;
  box-sizing: border-box;
  color: #fff;
  top: 16px;
  right: 24px;
  margin-top: 0px;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .Toastify__progress-bar--animated {
    animation: Toastify__trackProgress linear 1 forwards;
  }
  .Toastify__toast-body {
    margin: auto 0;
    -ms-flex: 1;
    flex: 1;
  }

  .Toastify__close-button {
    color: #fff;
    font-weight: 700;
    font-size: 14px;
    background: transparent;
    outline: none;
    border: none;
    padding: 0;
    cursor: pointer;
    opacity: 0.7;
    transition: 0.3s ease;
    -ms-flex-item-align: start;
    align-self: flex-start;
  }
  .Toastify__close-button:focus,
  .Toastify__close-button:hover {
    opacity: 1;
  }

  @keyframes SlideIn {
    from {
      transform: translate3d(150%, 0, 0);
    }

    50% {
      transform: translate3d(0, 0, 0);
    }
  }

  .SlideIn {
    animation-name: SlideIn;
  }

  @keyframes SlideOut {
    from {
      opacity: 1;
    }

    to {
      opacity: 0;
    }
  }

  .SlideOut {
    animation-name: SlideOut;
  }

  @keyframes Toastify__trackProgress {
    0% {
      transform: scaleX(1);
    }
    to {
      transform: scaleX(0);
    }
  }

  .Toastify__toast--success {
    background-color: ${toastColors.active.success};

    &:hover {
      background-color: ${toastColors.hover.success};
    }
  }

  .Toastify__toast--error {
    background-color: ${toastColors.active.error};

    &:hover {
      background-color: ${toastColors.hover.error};
    }
  }

  .Toastify__toast--info {
    background-color: ${toastColors.active.info};

    &:hover {
      background-color: ${toastColors.hover.info};
    }
  }

  .Toastify__toast--warning {
    background-color: ${toastColors.active.warning};

    &:hover {
      background-color: ${toastColors.hover.warning};
    }
  }

  .Toastify__toast {
    box-sizing: border-box;
    margin-bottom: 1rem;
    box-shadow: 0px 10px 16px -12px rgba(0, 0, 0, 0.3);
    display: flex;
    justify-content: space-between;
    max-height: 800px;
    overflow: hidden;
    cursor: pointer;

    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    color: #000;
    margin: 0 0 12px;
    padding: 12px;
    min-height: 32px;
    font: normal 12px "Open Sans", sans-serif;
    width: 100%;
    right: 0;
    transition: 0.3s;

    @media ${tablet} {
      // TODO: Discuss the behavior of notifications on mobile devices
      position: absolute;

      &:nth-child(1) {
        z-index: 3;
        top: 0px;
      }
      &:nth-child(2) {
        z-index: 2;
        top: 8px;
      }
      &:nth-child(3) {
        z-index: 1;
        top: 16px;
      }
    }
  }

  .Toastify__toast-body {
    display: flex;
    align-items: center;
  }

  @media ${tablet} {
    right: 16px;
  }

  @media only screen and (max-width: 480px) {
    left: 0;
    margin: auto;
    right: 0;
    width: 100%;
    max-width: calc(100% - 32px);

    @keyframes SlideIn {
      from {
        transform: translate3d(0, -150%, 0);
      }

      50% {
        transform: translate3d(0, 0, 0);
      }
    }
  }
`;

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
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  text: PropTypes.string,
  title: PropTypes.string,
  type: PropTypes.oneOf(["success", "error", "warning", "info"]).isRequired,
};

Toast.defaultProps = {
  autoClosed: true,
  text: "Demo text for example",
  title: "Demo title",
  type: "success",
};

export default Toast;
