import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Icons } from "../icons";
import { tablet } from "../../utils/device";

const itemTruncate = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const fontStyle = css`
  font-family: "Open Sans", sans-serif, Arial;
  font-style: normal;
`;

const disabledAndHeaderStyle = css`
  color: #a3a9ae;

  &:hover {
    cursor: default;
    background-color: white;
  }
`;

const StyledDropdownItem = styled.div`
  display: ${(props) => (props.textOverflow ? "block" : "flex")};
  width: 100%;
  max-width: 500px;
  border: 0px;
  cursor: pointer;
  margin: 0px;
  padding: 0px 16px;
  line-height: 32px;
  box-sizing: border-box;
  text-align: left;
  background: none;
  text-decoration: none;
  user-select: none;
  outline: 0 !important;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${fontStyle}

  font-weight: 600;
  font-size: 13px;
  color: #333333;
  text-transform: none;

  ${itemTruncate}

  &:hover {
    background-color: ${(props) => (props.noHover ? "white" : "#F8F9F9")};
    text-align: left;
  }

  ${(props) =>
    props.isSeparator &&
    `   
        padding: 0px 16px;
        border-bottom: 1px solid #ECEEF1;
        cursor: default;
        margin: 4px 16px 4px;
        line-height: 1px;
        height: 1px;
        width: calc(100% - 32px);
  
        &:hover {
          cursor: default;
        }
      `}

  ${(props) =>
    props.isHeader &&
    `
        ${disabledAndHeaderStyle}

        text-transform: uppercase;
        break-before: column;
      `}

    @media ${tablet} {
    line-height: 36px;
  }

  ${(props) => props.disabled && disabledAndHeaderStyle}
`;

const IconWrapper = styled.div`
  display: flex;
  width: 16px;
  margin-right: 8px;
  line-height: 14px;
`;

const DropDownItem = (props) => {
  //console.log("DropDownItem render");
  const {
    isSeparator,
    label,
    icon,
    children,
    disabled,
    onClick,
    className,
  } = props;
  const color = disabled ? "#A3A9AE" : "#333333";

  const onClickAction = (e) => {
    onClick && !disabled && onClick(e);
  };

  return (
    <StyledDropdownItem
      {...props}
      className={className}
      onClick={onClickAction}
    >
      {icon && (
        <IconWrapper>
          {React.createElement(Icons[icon], {
            size: "scale",
            color: color,
            isfill: true,
          })}
        </IconWrapper>
      )}
      {isSeparator ? "\u00A0" : label ? label : children && children}
    </StyledDropdownItem>
  );
};

DropDownItem.propTypes = {
  isSeparator: PropTypes.bool,
  isHeader: PropTypes.bool,
  tabIndex: PropTypes.number,
  label: PropTypes.string,
  disabled: PropTypes.bool,
  icon: PropTypes.string,
  noHover: PropTypes.bool,
  onClick: PropTypes.func,
  children: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  textOverflow: PropTypes.bool,
};

DropDownItem.defaultProps = {
  isSeparator: false,
  isHeader: false,
  tabIndex: -1,
  label: "",
  disabled: false,
  noHover: false,
  textOverflow: false,
};

export default DropDownItem;
