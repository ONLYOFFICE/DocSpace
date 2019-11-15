import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Text } from '../text';
import isEqual from "lodash/isEqual";
import { Icons } from "../icons";

const StyledSocialButton = styled.div`
  position: absolute;
  margin: 20px 0 0 20px;
  border-radius: 2px;
  width: 201px;
  height: 40px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;  

  ${props =>
    !props.isDisabled &&
    css`
      background: #FFFFFF;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.24), 0px 0px 1px rgba(0, 0, 0, 0.12);
      color: rgba(0, 0, 0, 0.54);

      :hover, :active {
        cursor: pointer;
        box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.24), 0px 0px 2px rgba(0, 0, 0, 0.12);
      }

      :hover {
        background: #FFFFFF;    
      }

      :active {
        background: #EEEEEE;  
        border: none;  
      }
    `
  };

  ${props =>
    props.isDisabled &&
    css`
      box-shadow: none;
      background: rgba(0, 0, 0, 0.08);
      color: rgba(0, 0, 0, 0.4);

      svg path {
        fill: rgba(0, 0, 0, 0.4);        
      } 
  `}

  .text-on-button {  
    position: absolute;
    width: 142px;
    height: 16px;
    left: 50px;
    right: 9px;
    top: 12px;
    font-family: Roboto, 'Open Sans', sans-serif, Arial;
    font-style: normal;
    font-weight: 500;
    font-size: 14px;
    line-height: 16px;
    letter-spacing: 0.21875px;
    user-select: none;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .picture-of-button svg {
    position: absolute;
    width: 18px;
    height: 18px;
    margin-top: 11px;
    margin-left: 11px;
    min-width: 18px;
    min-height: 18px;
  }
`;

const SocialButtonIcon = ({ iconName }) => {
  return <div className="picture-of-button">{React.createElement(Icons[iconName], {})}</div>;
};

class SocialButton extends React.Component {

  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    const {label, iconName} = this.props;  
    return (
      <StyledSocialButton {...this.props}>
        <SocialButtonIcon iconName={iconName}/>

        {label && (
          <Text.Body as="span" className="text-on-button">{label}</Text.Body>
        )}
      </StyledSocialButton>
    );
  }
}

SocialButton.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool
};

SocialButton.defaultProps = {
  label: '',
  iconName: 'SocialButtonGoogleIcon',
  tabIndex: -1,
  isDisabled: false
};

export default SocialButton;
