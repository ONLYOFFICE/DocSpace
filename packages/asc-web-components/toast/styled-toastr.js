import styled from "styled-components";
import IconButton from "../icon-button";
import Base from "../themes/base";

const IconWrapper = styled.div`
  align-self: start;
  display: flex;
  svg {
    width: ${(props) => props.theme.toastr.svg.width};
    min-width: ${(props) => props.theme.toastr.svg.minWidth};
    height: ${(props) => props.theme.toastr.svg.height};
    min-height: ${(props) => props.theme.toastr.svg.minHeight};
  }

  .toastr_success {
    path {
      fill: ${(props) => props.theme.toastr.svg.color.success};
    }
  }
  .toastr_error {
    path {
      fill: ${(props) => props.theme.toastr.svg.color.error};
    }
  }
  .toastr_warning {
    path {
      fill: ${(props) => props.theme.toastr.svg.color.warning};
    }
  }
  .toastr_info {
    path {
      fill: ${(props) => props.theme.toastr.svg.color.info};
    }
  }
`;
IconWrapper.defaultProps = { theme: Base };

const StyledDiv = styled.div`
  margin: 0 15px;

  .toast-title {
    font-weight: ${(props) => props.theme.toastr.title.lineHeight};
    margin: ${(props) => props.theme.toastr.title.margin};
    margin-bottom: ${(props) => props.theme.toastr.title.marginBottom};
    line-height: ${(props) => props.theme.toastr.title.lineHeight};
    color: ${(props) => props.theme.toastr.title.color[props.type]};
    font-size: ${(props) => props.theme.toastr.title.fontSize};
  }

  .toast-text {
    line-height: ${(props) => props.theme.toastr.text.lineHeight};
    align-self: center;
    font-size: ${(props) => props.theme.toastr.text.fontSize};
    color: ${(props) => props.theme.toastr.text.color};
    word-break: break-word;
  }
`;
StyledDiv.defaultProps = { theme: Base };

const StyledCloseWrapper = styled.div`
  .closeButton {
    opacity: 0.5;
    padding-top: 2px;
    &:hover {
      opacity: 1;
    }
  }
`;

const StyledIconButton = styled(IconButton)`
  svg {
    path {
      fill: ${(props) => props.theme.toastr.closeButtonColor};
    }
  }
`;

StyledIconButton.defaultProps = { theme: Base };

export { StyledCloseWrapper, StyledDiv, IconWrapper, StyledIconButton };
