import React from 'react'
import { toast } from 'react-toastify'
import styled from 'styled-components'
import { Icons } from '../icons'

const Icon = ({ type }) => (
  type === "success"
    ? <Icons.CheckIcon color="#ffffff" isfill={true} />
    : type === "error" || type === "warning"
      ? <Icons.DangerIcon color="#ffffff" isfill={true} />
      : <Icons.InfoIcon color="#ffffff" isfill={true} />
);

const StyledDiv = styled.div`
  margin-left: 15px;
`;

const ToastTitle = styled.p`
  font-weight: bold;
  margin: 0;
`;

const toastr = {
  clear: clear,
  error: error,
  info: info,
  success: success,
  warning: warning
};


const notify = (type, text, title, timeout = 5000, withCross = false,  centerPosition = false) => {
  return toast(
    <>
      <div>
        <Icon type={type} />
      </div>
      <StyledDiv>
        <ToastTitle>{title}</ToastTitle>
        {text}
      </StyledDiv>
    </>,
    {
      type: type,
      closeOnClick: !withCross,
      closeButton: withCross,
      autoClose: timeout === 0 ? false : timeout < 750 ? 5000 : (timeout || 5000),
      position: centerPosition && toast.POSITION.TOP_CENTER
    }
  );
};

function success(text, title, timeout, withCross, centerPosition) {
  return notify('success', text, title, timeout, withCross, centerPosition);
}

function error(text, title, timeout, withCross, centerPosition) {
  return notify('error', text, title, timeout, withCross, centerPosition);
}

function warning(text, title, timeout, withCross, centerPosition) {
  return notify('warning', text, title, timeout, withCross, centerPosition);
}

function info(text, title, timeout, withCross, centerPosition) {
  return notify('info', text, title, timeout, withCross, centerPosition);
}

function clear() {
  return toast.dismiss();
}

export default toastr;