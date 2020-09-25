import React from 'react'
import { ToastContainer, cssTransition } from 'react-toastify'
import styled from 'styled-components'
import PropTypes from 'prop-types'

const Fade = cssTransition({
  enter: 'fadeIn',
  exit: 'fadeOut'
});

const toastColors = {
  active: {
    success: '#cae796',
    error: '#ffbfaa',
    info: '#f1da92',
    warning: '#f1ca92'
  },
  hover: {
    success: '#bcdf7e',
    error: '#ffa98d',
    info: '#eed27b',
    warning: '#eeb97b'
  }
};

const StyledToastContainer = styled(ToastContainer)`
  width: 365px !important;
  z-index: 9999;
  -webkit-transform: translateZ(9999px);
  position: fixed;
  padding: 4px;
  width: 320px;
  box-sizing: border-box;
  color: #fff;
  top: 1em;
  right: 1em;
  margin-top: 40px;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  @media only screen and (max-width: 480px) {
    width: 100vw;
    padding: 0;
    left: 0;
    margin: 0;
}

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

@keyframes Toastify__trackProgress {
  0% {
    transform: scaleX(1);
  }
  to {
    transform: scaleX(0);
  }
}

.Toastify__toast--success{
  background-color: ${toastColors.active.success};

  &:hover {
      background-color: ${toastColors.hover.success};
  }
}

.Toastify__toast--error{
  background-color: ${toastColors.active.error};

  &:hover {
    background-color: ${toastColors.hover.error};
  }
}

.Toastify__toast--info{
  background-color: ${toastColors.active.info};

  &:hover {
    background-color: ${toastColors.hover.info};
  }
}

.Toastify__toast--warning{
  background-color: ${toastColors.active.warning};

  &:hover {
    background-color: ${toastColors.hover.warning};
  }
}

@-webkit-keyframes fadeout {
  0% {
      opacity: 1;
      }

  100% {
      opacity: 0;
      }
}

@keyframes fadeout {
  0% {
    opacity: 1;
    }
  100% {
    opacity: 0;
    }
}

.fadeOut {
  opacity: 0;
  -moz-animation: fadeout 1s linear;
  -webkit-animation: fadeout 1s linear;
  animation: fadeout 1s linear;
}

@-webkit-keyframes fadein {
  0% {
    opacity: 0;
    }
  100% {
    opacity: 1;
    }
}

@keyframes fadein {
  0% {
    opacity: 0;
    }
  100% {
    opacity: 1;
    }
}

.fadeIn {
  opacity: 1;
  -moz-animation: fadein 0.3s linear;
  -webkit-animation: fadein 0.3s linear;
  animation: fadein 0.3s linear;
}

/* .Toastily__toast or & > div (less productive) */
.Toastify__toast 
{
      box-sizing: border-box;
      margin-bottom: 1rem;
      box-shadow: 0 1px 10px 0 rgba(0, 0, 0, 0.1), 0 2px 15px 0 rgba(0, 0, 0, 0.05);
      display: flex;
      justify-content: space-between;
      max-height: 800px;
      overflow: hidden;
      cursor: pointer;

      border-radius: 3px;
      -moz-border-radius: 3px;
      -webkit-border-radius: 3px;
      color: #000;
      margin: 0 0 6px;
      padding: 12px;
      min-height: 32px;
      font: normal 12px 'Open Sans', sans-serif;
      width: 100%;
  }

/* .Toastify__toast-body or & > div > div (less productive) */
.Toastify__toast-body {
    display: flex;
    align-items: center;
  }
`;

const Toast = props => {
  //console.log("Toast render");
  return (
    <StyledToastContainer
      className={props.className}
      draggable={false}
      hideProgressBar={true}
      id={props.id}
      newestOnTop={true}
      pauseOnFocusLoss={false}
      style={props.style}
      transition={Fade}
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
  type: PropTypes.oneOf(['success', 'error', 'warning', 'info']).isRequired
};

Toast.defaultProps = {
  autoClosed: true,
  text: 'Demo text for example',
  title: 'Demo title',
  type: 'success',
}

export default Toast;
