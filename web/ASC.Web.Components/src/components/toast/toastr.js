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


const notify = (type, text, title, autoClosed = true, centerPosition) => {
  console.log(centerPosition, ' is position')
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
      autoClose: autoClosed,
      position: centerPosition && toast.POSITION.TOP_CENTER
    }
  );
};

function success(text, title, autoClosed, centerPosition) {
  return notify('success', text, title, autoClosed, centerPosition);
}

function error(text, title, autoClosed, centerPosition) {
  return notify('error', text, title, autoClosed, centerPosition);
}

function warning(text, title, autoClosed, centerPosition) {
  return notify('warning', text, title, autoClosed, centerPosition);
}

function info(text, title, autoClosed, centerPosition) {
  return notify('info', text, title, autoClosed, centerPosition);
}

function clear() {
  return toast.dismiss();
}

export default toastr;