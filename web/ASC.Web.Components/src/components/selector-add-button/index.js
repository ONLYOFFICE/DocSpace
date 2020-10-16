import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import IconButton from "../icon-button";

const StyledButton = styled.div`
  background: #f8f9f9;
  border: 1px solid #eceef1;
  box-sizing: border-box;
  border-radius: 3px;
  height: 34px;
  width: 34px;
  padding: 9px;
  display: inline-block;
  cursor: ${(props) => (!props.isDisabled ? "pointer" : "default")};

  &:hover {
    path {
      ${(props) => !props.isDisabled && "fill: #333;"}
    }
  }

  &:active {
    ${(props) => !props.isDisabled && "background-color: #ECEEF1;"}
  }
`;

const SelectorAddButton = (props) => {
  const { isDisabled, title, onClick, className, id, style } = props;

  return (
    <StyledButton
      isDisabled={isDisabled}
      title={title}
      onClick={onClick}
      className={className}
      id={id}
      style={style}
    >
      <IconButton
        color="#979797"
        size={14}
        iconName="PlusIcon"
        isFill={true}
        isDisabled={isDisabled}
        isClickable={!isDisabled}
      />
    </StyledButton>
  );
};

SelectorAddButton.propTypes = {
  title: PropTypes.string,
  onClick: PropTypes.func,
  isDisabled: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

SelectorAddButton.defaultProps = {
  isDisabled: false,
};

export default SelectorAddButton;
