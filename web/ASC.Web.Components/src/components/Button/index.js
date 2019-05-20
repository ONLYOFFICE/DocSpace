import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';

const StyledButton = styled.button`
  /* Adapt the colors based on primary prop */
  background-color: ${props => (!props.disabled ? (props.primary ? '#2da7db' : '#ebebeb') : (props.primary ? '#a6dcf2' : '#f7f7f7')) };
  color: ${props => (props.primary ? '#ffffff' : (!props.disabled ? '#666666' : '#999'))};
  font-size: ${props =>
    props.size === 'huge' || props.size === 'big' ? '15px' : '12px'};
  line-height: ${props =>
    props.size === 'huge' ? '15px;' : props.size === 'big' ? '17px;' : '13px;'};

  height: ${props =>
    props.size === 'huge'
      ? '40px'
      : props.size === 'big'
      ? '32px'
      : props.size === 'middle'
      ? '24px'
      : '21px'};
  padding: ${props =>
    props.size === 'huge'
      ? '12px 30px 13px;'
      : props.size === 'big'
      ? '7px 30px 8px;'
      : props.size === 'middle'
      ? '5px 24px 6px;'
      : '4px 13px;'};

  border: none;
  cursor: ${props => (!props.disabled ? 'pointer' : 'default')};
  display: inline-block;
  font-family: 'Open Sans', sans-serif;
  margin: 0;
  font-weight: normal;
  text-align: center;
  text-decoration: none;
  vertical-align: middle;
  border-radius: 3px;
  -moz-border-radius: 3px;
  -webkit-border-radius: 3px;
  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;
  user-select: none;
  -o-user-select: none;
  -moz-user-select: none;
  -webkit-user-select: none;

  ${props =>
    !props.primary &&
    css`
      border-width: 1px;
      border-style: solid;
      border-color: ${props => !props.disabled ? '#c4c4c4' : '#ebebeb'};
    `}

  ${props =>
    !props.disabled &&
    css`
      &:hover {
        background-color: ${props => (props.primary ? '#3db8ec' : '#f5f5f5')};
        color: ${props => (props.primary ? '#ffffff' : '#666666')};
      }
      &:active {
        background-color: ${props => (props.primary ? '#1f97ca' : '#e2e2e2')};
        color: #ffffff;

        ${props =>
          !props.primary &&
          css`
            border-width: 1px;
            border-style: solid;
            border-color: #dadada;
          `}
      }

      

    `}
`;

const Button = props => {
  return <StyledButton {...props} />;
};

Button.propTypes = {
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  primary: PropTypes.bool,
  disabled: PropTypes.bool,
  onClick: PropTypes.func.isRequired,
};

Button.defaultProps = {
  primary: false,
  disabled: false,
  size: 'base'
};

export default Button;
