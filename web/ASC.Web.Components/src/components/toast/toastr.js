import React from "react";
import { toast } from "react-toastify";
import styled from "styled-components";
import { Icons } from "../icons";
import IconButton from "../icon-button";
import Text from "../text";

// eslint-disable-next-line react/prop-types
const Icon = ({ type }) =>
  type === "success" ? (
    <Icons.CheckToastIcon color="#333333" isfill={true} />
  ) : type === "error" || type === "warning" ? (
    <Icons.DangerToastIcon color="#333333" isfill={true} />
  ) : (
    <Icons.InfoToastIcon color="#333333" isfill={true} />
  );

const IconWrapper = styled.div`
  align-self: end;
  display: flex;
  svg {
    width: 16px;
    min-width: 16px;
    height: 16px;
    min-height: 16px;
  }
`;

const StyledDiv = styled.div`
  margin: 0 15px;

  .toast-title {
    font-weight: 600;
    margin: 0;
    margin-bottom: 5px;
    line-height: 16px;
    color: #000000;
    font-size: 12px;
  }

  .toast-text {
    line-height: 1.3;
    align-self: center;
    font-size: 12px;
    color: #333333;
  }
`;

const StyledCloseWrapper = styled.div`
  .closeButton {
    opacity: 0.5;
    padding-top: 2px;
    &:hover {
      opacity: 1;
    }
  }
`;

const toastr = {
  clear: clear,
  error: error,
  info: info,
  success: success,
  warning: warning,
};

const CloseButton = ({ closeToast }) => (
  <StyledCloseWrapper>
    <IconButton
      className="closeButton"
      onClick={closeToast}
      iconName="CrossIcon"
      size={12}
      color="#333333"
    />
  </StyledCloseWrapper>
);

const notify = (
  type,
  data,
  title,
  timeout = 5000,
  withCross = false,
  centerPosition = false
) => {
  return toast(
    <>
      <IconWrapper>
        <Icon size="medium" type={type} />
      </IconWrapper>
      <StyledDiv>
        {title && <Text className="toast-title">{title}</Text>}
        {data && <Text className="toast-text">{data}</Text>}
      </StyledDiv>
    </>,
    {
      type: type,
      closeOnClick: !withCross,
      closeButton: withCross && <CloseButton />,
      autoClose: timeout === 0 ? false : timeout < 750 ? 5000 : timeout || 5000,
      position: centerPosition && toast.POSITION.TOP_CENTER,
    }
  );
};

function success(data, title, timeout, withCross, centerPosition) {
  return notify("success", data, title, timeout, withCross, centerPosition);
}

function error(data, title, timeout, withCross, centerPosition) {
  const dataType = typeof data;
  const message =
    dataType === "string"
      ? data
      : dataType === "object" && data.statusText
      ? data.statusText
      : "";

  return notify("error", message, title, timeout, withCross, centerPosition);
}

function warning(data, title, timeout, withCross, centerPosition) {
  return notify("warning", data, title, timeout, withCross, centerPosition);
}

function info(data, title, timeout, withCross, centerPosition) {
  return notify("info", data, title, timeout, withCross, centerPosition);
}

function clear() {
  return toast.dismiss();
}

export default toastr;
