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


const notify = (type, text, title, autoClosed = true) => {

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
      closeOnClick: autoClosed,
      closeButton: !autoClosed,
      autoClose: autoClosed
    }
  );
};

function success(text, title, autoClosed) {
  return notify('success', text, title, autoClosed);
}

function error(text, title, autoClosed) {
  return notify('error', text, title, autoClosed);
}

function warning(text, title, autoClosed) {
  return notify('warning', text, title, autoClosed);
}

function info(text, title, autoClosed) {
  return notify('info', text, title, autoClosed);
}

function clear() {
  return toast.dismiss();
}

export default toastr;