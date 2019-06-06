import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import Loader from '../loader';

const activatedCss = css`
  background-color: ${props => (props.primary ? '#1f97ca' : '#e2e2e2')};
  color: #ffffff;

  ${props =>
    !props.primary &&
    css`
      border-width: 1px;
      border-style: solid;
      border-color: #dadada;
    `}

`;

const hoveredCss = css`
  background-color: ${props => (props.primary ? '#3db8ec' : '#f5f5f5')};
  color: ${props => (props.primary ? '#ffffff' : '#666666')};
`;

const StyledButton = styled.button.attrs((props) => ({
  disabled: props.isDisabled || props.isLoading ? 'disabled' : '',
  tabIndex: props.tabIndex
}))`
  height: ${props =>
    (props.size === 'huge' && '40px') ||
    (props.size === 'big' && '32px') ||
    (props.size === 'middle' && '24px') ||
    (props.size === 'base' && '21px')
  };

  line-height: ${props => props.size === 'huge' && '15px' || props.size === 'big' && '17px' || '13px'};

  font-size: ${props =>
    ((props.size === 'huge' || props.size === 'big') && '15px') ||
    ((props.size === 'middle' || props.size === 'base') && '12px')
  };

  color: ${props => (props.primary && '#ffffff') || (!props.isDisabled ? '#666666' : '#999')};

  background-color: ${props => (!props.isDisabled || props.isLoading ? (props.primary ? '#2da7db' : '#ebebeb') : (props.primary ? '#a6dcf2' : '#f7f7f7'))};

  padding: ${props =>
    (props.size === 'huge' && (props.primary ? '12px 30px 13px' : '11px 30px 12px')) ||
    (props.size === 'big' && (props.primary ? '7px 30px 8px' : '6px 30px 7px')) ||
    (props.size === 'middle' && (props.primary ? '5px 24px 6px' : '4px 24px 5px')) ||
    (props.size === 'base' && (props.primary ? '4px 13px' : '3px 12px'))
  };

  cursor: ${props => props.isDisabled || props.isLoading ? 'default !important' : 'pointer'};

  font-family: 'Open Sans', sans-serif;
  border: none;
  margin: 0;
  display: inline-block;
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
  stroke: none;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;

  ${props =>
    !props.primary &&
    css`
      border-width: 1px;
      border-style: solid;
      border-color: ${props => (!props.isDisabled && !props.isLoading) ? '#c4c4c4' : '#ebebeb'};
    `}
  
  ${props => (!props.isDisabled && !props.isLoading) && (props.isActivated ? `${activatedCss}` : css`
    &:active {
      ${activatedCss}
    }`)
  }

  ${props => (!props.isDisabled && !props.isLoading) && (props.isHovered ? `${hoveredCss}` : css`
    &:hover {
      ${hoveredCss}
    }`)
  }

  &:focus {
    outline: none
  }
`;

const ButtonLoader = styled(Loader)`
  display: inline-block;
  margin-right: 8px;
`;

const getLoaderSize = (btnSize) => {
  switch (btnSize) {
    case 'big':
      return 16;
    case 'huge':
      return 18;
    default:
      return 14;
  }

}

const Button = props => {
  const { isLoading, label, primary, size } = props;
  return (
    <StyledButton {...props}>
      {isLoading && <ButtonLoader type="oval" size={getLoaderSize(size)} color={primary ? "white" : 'black'} />}
      {label}
    </StyledButton>
  );
};

Button.propTypes = {
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  primary: PropTypes.bool,
  label: PropTypes.string,

  tabIndex: PropTypes.number,
  isActivated: PropTypes.bool,
  isHovered: PropTypes.bool,
  isPressed: PropTypes.bool,
  isDisabled: PropTypes.bool,
  isLoading: PropTypes.bool,
  onClick: PropTypes.func.isRequired,
};

Button.defaultProps = {
  primary: false,
  isActivated: false,
  isHovered: false,
  isPressed: false,
  isDisabled: false,
  isLoading: false,
  size: 'base',
  tabIndex: -1,
  label: ''
};

export default Button;
