import React from 'react'
import { ToastContainer, cssTransition } from 'react-toastify'
import styled from 'styled-components'
import PropTypes from 'prop-types'

const Fade = cssTransition({
  enter: 'fadeIn',
  exit: 'fadeOut'
});

const StyledToastContainer = styled(ToastContainer)`
width: 365px !important;

.Toastify__toast--success{
  background-color: #cae796;

  &:hover {
      background-color: #bcdf7e;
  }
}

.Toastify__toast--error{
  background-color: #ffbfaa;

  &:hover {
    background-color: #ffa98d;
  }
}

.Toastify__toast--info{
  background-color: #f1da92;

  &:hover {
    background-color: #eed27b;
  }
}

.Toastify__toast--warning{
  background-color: #f1ca92;

  &:hover {
    background-color: #eeb97b;
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
      border-radius: 3px;
      -moz-border-radius: 3px;
      -webkit-border-radius: 3px;
      color: #000;
      margin: 0 0 6px;
      padding: 13px 11px 13px 11px;
      min-height: 32px;
      font: normal 12px 'Open Sans', sans-serif;
      width: 100%;
  }

/* .Toastify__toast-body or & > div > div (less productive) */
.Toastify__toast-body {
    display: flex;
    align-items: center;
  }

svg {
    width: 20px;
    min-width: 20px;
    height: 20px;
    min-height: 20px;
  }

`;

const Toast = props => {
  //console.log("Toast render");
  return (
    <StyledToastContainer
      draggable={false}
      hideProgressBar={true}
      newestOnTop={true}
      pauseOnFocusLoss={false}
      transition={Fade}
    />
  );
};


Toast.propTypes = {
  autoClosed: PropTypes.bool,
  text: PropTypes.string,
  title: PropTypes.string,
  type: PropTypes.oneOf(['success', 'error', 'warning', 'info']).isRequired,
};

Toast.defaultProps = {
  text: 'Demo text for example',
  title: 'Demo title',
  autoClosed: true,
  type: 'success',
}

export default Toast;
