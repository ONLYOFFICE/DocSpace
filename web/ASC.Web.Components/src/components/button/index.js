import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import Loader from '../loader';

const activeCss = css`
  background-color: ${props => (props.primary ? '#1F97CA' : '#ECEEF1')};
  color: ${props => (props.primary ? '#ffffff' : '#333333')};

  ${props =>
    !props.primary &&
    css`
      border: 1px solid #2DA7DB;
      box-sizing: border-box;
    `}

`;

const hoverCss = css`
  background-color: ${props => (props.primary ? '#3DB8EC' : '#FFFFFF')};
  color: ${props => (props.primary ? '#ffffff' : '#333333')};

  ${props =>
    !props.primary &&
    css`
      border: 1px solid #2DA7DB;
      box-sizing: border-box;
    `}
`;

const ButtonWrapper = ({primary, isHovered, isClicked, isDisabled, isLoading, ...props}) => <button type="button" {...props}></button>;

const StyledButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled || props.isLoading ? 'disabled' : '',
  tabIndex: props.tabIndex
}))`
  height: ${props =>
    (props.size === 'big' && '36px') ||
    (props.size === 'medium' && '32px') ||
    (props.size === 'base' && '24px')
  };

  line-height: ${props =>
    (props.size === 'big' && '19px') ||
    (props.size === 'medium' && '18px') ||
    (props.size === 'base' && '16px')
  };

  font-size: ${props =>
    (props.size === 'big' && '14px') ||
    (props.size === 'medium' && '13px') ||
    (props.size === 'base' && '12px')
  };

  color: ${props => (props.primary && '#FFFFFF') || (!props.isDisabled ? '#333333' : '#ECEEF1')};

  background-color: ${props => 
    (!props.isDisabled || props.isLoading 
      ? (props.primary ? '#2DA7DB' : '#FFFFFF') 
      : (props.primary ? '#A6DCF2' : '#FFFFFF'))
  };

  padding: ${props =>
    (props.size === 'big' && (props.primary 
      ? (props.icon 
          ? (props.label ? '8px 24px 9px 24px' : '8px 10px 9px 10px')
          : (props.label ? '8px 28px 9px 28px' : '8px 10px 9px 10px')
        ) 
      : (props.icon 
          ? (props.label ? '8px 24px 9px 24px' : '8px 10px 9px 10px')
          : (props.label ? '8px 27px 9px 28px' : '8px 10px 9px 10px')
        ))
    ) ||
    (props.size === 'medium' && (props.primary 
      ? (props.icon 
          ? (props.label ? '7px 24px 7px 24px' : '7px 10px 7px 10px')
          : (props.label ? '7px 24px 7px 24px' : '7px 10px 7px 10px')
        ) 
      : (props.icon 
          ? (props.label ? '7px 24px 7px 24px' : '7px 10px 7px 10px')
          : (props.label ? '7px 24px 7px 24px' : '7px 10px 7px 10px')
        ))
    ) ||
    (props.size === 'base' && (props.primary 
      ? (props.icon 
          ? (props.label ? '3px 20px 5px 20px' : '3px 5px 5px 5px') 
          : (props.label ? '3px 24px 5px 24px' : '3px 5px 5px 5px')
        )
      : (props.icon 
          ? (props.label ? '3px 20px 5px 20px' : '3px 5px 5px 5px') 
          : (props.label ? '3px 24px 5px 24px' : '3px 5px 5px 5px')
        ))
    )
  };

  cursor: ${props => props.isDisabled || props.isLoading ? 'default !important' : 'pointer'};

  font-family: 'Open Sans', sans-serif;
  border: none;
  margin: 0;
  display: inline-block;
  font-weight: 600;
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
      border: 1px solid;
      box-sizing: border-box;
      border-color: ${props => (!props.isDisabled && !props.isLoading) ? '#D0D5DA' : '#ECEEF1'};
    `}

  ${props => (!props.isDisabled && !props.isLoading) && (props.isHovered ? hoverCss : css`
    &:hover {
      ${hoverCss}
    }`)
  }

  ${props => (!props.isDisabled && !props.isLoading) && (props.isClicked ? activeCss : css`
    &:active {
      ${activeCss}
    }`)
  }

  &:focus {
    outline: none
  }

  .btnIcon,
  .loader {
    display: inline-block;
    vertical-align: text-top;
  }

  ${props => props.label && css`
    .btnIcon,
    .loader {
      padding-right: 4px;
    }
  `}
`;

const Icon = ({size, primary, icon}) => (
  <div className="btnIcon">
    { icon && React.cloneElement(icon, 
      { 
        isfill: true, 
        size: size === "big" ? "medium" : "small", 
        color: primary ? "#FFFFFF" : '#333333'
    })}
  </div>
);

Icon.propTypes = {
  icon: PropTypes.node
};

Icon.defaultProps = {
  icon: null
};

const Button = props => {
  //console.log("Button render");
  const { isLoading, label, primary, size, icon } = props;
  return (
    <StyledButton {...props}>
        {(isLoading || icon) && 
          isLoading 
            ? <Loader type="oval" size={size === "big" ? 16 : 14} color={primary ? "#FFFFFF" : '#333333'} className="loader" />
            : <Icon {...props} />
        }
        {label}
    </StyledButton>
  );
};

Button.propTypes = {
  label: PropTypes.string,
  primary: PropTypes.bool,
  size: PropTypes.oneOf(['base', 'medium', 'big']),
  icon: PropTypes.node,

  tabIndex: PropTypes.number,

  isHovered: PropTypes.bool,
  isClicked: PropTypes.bool,
  isDisabled: PropTypes.bool,
  isLoading: PropTypes.bool,

  onClick: PropTypes.func.isRequired,
};

Button.defaultProps = {
  label: '',
  primary: false,
  size: 'base',
  icon: null,

  tabIndex: -1,

  isHovered: false,
  isClicked: false,
  isDisabled: false,
  isLoading: false
};

export default Button;
