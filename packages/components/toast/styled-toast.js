import styled from "styled-components";
import { ToastContainer } from "react-toastify";
import { tablet } from "../utils/device";
import Base from "../themes/base";

const StyledToastContainer = styled(ToastContainer)`
  z-index: ${(props) => props.theme.toast.zIndex};
  -webkit-transform: translateZ(9999px);
  position: fixed;
  padding: ${(props) => props.theme.toast.padding};
  width: ${(props) => props.theme.toast.width};
  box-sizing: border-box;
  color: ${(props) => props.theme.toast.color};
  top: ${(props) => props.theme.toast.top};
  right: ${(props) => props.theme.toast.right};
  margin-top: ${(props) => props.theme.toast.marginTop};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .Toastify__progress-bar--animated {
    animation: Toastify__trackProgress linear 1 forwards;
  }
  .Toastify__toast-body {
    overflow-wrap: anywhere;
    margin: auto 0;
    -ms-flex: 1;
    flex: 1;
  }

  .Toastify__close-button {
    color: ${(props) => props.theme.toast.closeButton.color};
    font-weight: ${(props) => props.theme.toast.closeButton.fontWeight};
    font-size: ${(props) => props.theme.toast.closeButton.fontSize};
    background: ${(props) => props.theme.toast.closeButton.background};
    outline: none;
    border: none;
    padding: ${(props) => props.theme.toast.closeButton.padding};
    cursor: pointer;
    opacity: ${(props) => props.theme.toast.closeButton.opacity};
    transition: ${(props) => props.theme.toast.closeButton.transition};
    -ms-flex-item-align: start;
    align-self: flex-start;
  }
  .Toastify__close-button:focus,
  .Toastify__close-button:hover {
    opacity: ${(props) => props.theme.toast.closeButton.hoverOpacity};
  }

  @keyframes SlideIn {
    from {
      transform: translate3d(150%, 0, 0);
    }

    50% {
      visibility: hidden;
      transform: translate3d(0, 0, 0);
    }
  }

  .SlideIn {
    animation: SlideIn 0.7s ease-out;
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
    animation: SlideOut 0.3s ease-out both;
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
    background-color: ${(props) => props.theme.toast.active.success};
    border: ${(props) => props.theme.toast.border.success};

    &:hover {
      background-color: ${(props) => props.theme.toast.hover.success};
    }
  }

  .Toastify__toast--error {
    background-color: ${(props) => props.theme.toast.active.error};
    border: ${(props) => props.theme.toast.border.error};

    &:hover {
      background-color: ${(props) => props.theme.toast.hover.error};
    }
  }

  .Toastify__toast--info {
    background-color: ${(props) => props.theme.toast.active.info};
    border: ${(props) => props.theme.toast.border.info};

    &:hover {
      background-color: ${(props) => props.theme.toast.hover.info};
    }
  }

  .Toastify__toast--warning {
    background-color: ${(props) => props.theme.toast.active.warning};
    border: ${(props) => props.theme.toast.border.warning};

    &:hover {
      background-color: ${(props) => props.theme.toast.hover.warning};
    }
  }

  .Toastify__toast {
    box-sizing: border-box;
    margin-bottom: ${(props) => props.theme.toast.main.marginBottom};
    box-shadow: ${(props) => props.theme.toast.main.boxShadow};
    display: flex;
    justify-content: space-between;
    max-height: ${(props) => props.theme.toast.main.maxHeight};
    overflow: ${(props) => props.theme.toast.main.overflow};
    cursor: pointer;

    border-radius: ${(props) => props.theme.toast.main.borderRadius};
    -moz-border-radius: ${(props) => props.theme.toast.main.borderRadius};
    -webkit-border-radius: ${(props) => props.theme.toast.main.borderRadius};
    color: ${(props) => props.theme.toast.main.color};
    margin: ${(props) => props.theme.toast.main.margin};
    padding: ${(props) => props.theme.toast.main.padding};
    min-height: ${(props) => props.theme.toast.main.minHeight};
    font: normal 12px "Open Sans", sans-serif;
    width: ${(props) => props.theme.toast.main.width};
    right: ${(props) => props.theme.toast.main.right};
    transition: ${(props) => props.theme.toast.main.transition};

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
StyledToastContainer.defaultProps = { theme: Base };

export default StyledToastContainer;
