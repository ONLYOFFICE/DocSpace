import React from "react";
import { toast } from "react-toastify";
import styled from "styled-components";

import { CheckToastIcon, DangerToastIcon, InfoToastIcon } from "./svg";
import IconButton from "../icon-button";
import Text from "../text";
import {
  StyledCloseWrapper,
  StyledDiv,
  IconWrapper,
  StyledIconButton,
} from "./styled-toastr";
import commonIconsStyles from "../utils/common-icons-style";

const StyledCheckToastIcon = styled(CheckToastIcon)`
  ${commonIconsStyles}
`;
const StyledDangerToastIcon = styled(DangerToastIcon)`
  ${commonIconsStyles}
`;
const StyledInfoToastIcon = styled(InfoToastIcon)`
  ${commonIconsStyles}
`;

// eslint-disable-next-line react/prop-types
const Icon = ({ type, theme }) =>
  type === "success" ? (
    <StyledCheckToastIcon className="toastr_icon toastr_success" />
  ) : type === "error" ? (
    <StyledDangerToastIcon className="toastr_icon toastr_error" />
  ) : type === "warning" ? (
    <StyledDangerToastIcon className="toastr_icon toastr_warning" />
  ) : (
    <StyledInfoToastIcon className="toastr_icon toastr_info" />
  );

const toastr = {
  clear: clear,
  error: error,
  info: info,
  success: success,
  warning: warning,
};

const CloseButton = ({ closeToast, theme }) => (
  <StyledCloseWrapper>
    <StyledIconButton
      className="closeButton"
      onClick={closeToast}
      iconName="/static/images/cross.react.svg"
      size={12}
    />
  </StyledCloseWrapper>
);

const notify = (
  type,
  data,
  title,
  timeout = 5000,
  withCross = false,
  centerPosition = false,
  theme
) => {
  return toast(
    <>
      <IconWrapper>
        <Icon size="medium" type={type} />
      </IconWrapper>
      <StyledDiv type={type}>
        {typeof data === "string" ? (
          <>
            {title && <Text className="toast-title">{title}</Text>}
            {data && <Text className="toast-text">{data}</Text>}
          </>
        ) : (
          data
        )}
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
      : dataType === "object" && data.message
      ? data.message
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
