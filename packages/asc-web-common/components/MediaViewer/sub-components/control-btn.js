import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

const StyledVideoControlBtn = styled.div`
  display: inline-block;
  height: 30px;
  line-height: 25px;
  margin: 5px;
  width: 40px;
  border-radius: 2px;
  cursor: pointer;
  text-align: center;

  &:hover {
    background-color: rgba(200, 200, 200, 0.2);
  }
`;
const ControlBtn = (props) => {
  return (
    <StyledVideoControlBtn {...props}>{props.children}</StyledVideoControlBtn>
  );
};

ControlBtn.propTypes = {
  children: PropTypes.any,
};

export default ControlBtn;
