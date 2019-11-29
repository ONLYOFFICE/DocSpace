import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Text } from '../text';
import isEqual from "lodash/isEqual";
import { Icons } from "../icons";

const partOfButton = css`
  .part-of-button {
    height: 100%;
    padding: 0;
    display: flex;
    text-align: left; 
    stroke: none;
  }

  ${props =>
    !props.isDisabled ?
      css`
        .part-of-button {
          background: #FFFFFF;
          border-color: #D0D5DA;
        }
        
        :hover, :active {
          .part-of-button {
            cursor: pointer;
            border-color: #2DA7DB;
          }

          .toggle svg rect {
            fill: #2DA7DB;
          }

          .toggle svg path {
            fill: white;
          }
        }
      
        :hover {
          .part-of-button {
            background: #FFFFFF;    
          }
        } 
      
        :active {
          .part-of-button {
            background: #ECEEF1;
          }
        }
      `
      :
      css`
        .part-of-button {
          background: #F8F9F9;
          border-color: #ECEEF1;
        }
      `
  };
      
  ${props =>
    props.isChecked && !props.isDisabled &&
      css`
        .part-of-button {
          border-color: rgba(0, 0, 0, 0);
          background: #2DA7DB;
        }
        
        :hover, :active {
          .part-of-button {
            cursor: pointer;
          }
        }
      
        :hover {
          .part-of-button {
            border-color: rgba(0, 0, 0, 0);
            background: #3DB8EC;    
          }

          .toggle svg rect {
            fill: white;
          }

          .toggle svg path {
            fill: #3DB8EC;
          }
        } 
      
        :active {
          .part-of-button {
            border-color: rgba(0, 0, 0, 0);
            background: #1F97CA;
          }

          .toggle svg rect {
            fill: white;
          }

          .toggle svg path {
            fill: #1F97CA;
          }
        }
      `
  };
`;

const ButtonContainer = styled.button.attrs((props) => ({
  disabled: props.isDisabled ? 'disabled' : '',
  tabIndex: props.tabIndex
}))`
  border-width: 1px 0 1px 1px;
  border-style: solid;
  background-clip: padding-box !important;
  border-radius: 3px 0 0 3px;
  width: 112px;

  font-family: 'Open Sans', sans-serif;
  font-weight: 600;
  text-decoration: none; 

  &:focus {
    outline: none;
  }

  .social_button_text {  
    display: inline-block;
    width: 67px;
    height: 19px;
    margin: 8px 10px 9px 8px;
    font-style: normal;
    font-weight: 600;
    line-height: 19px;
    user-select: none;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  } 

  svg {
    display: inline-block;
    margin: 10px 0 10px 10px;
    width: 16px;
    height: 16px;
    min-width: 16px;
    min-height: 16px;
  }

  ${props =>
    !props.isDisabled ?
      css`
        .social_button_text {  
          color: #333333;
        }

        svg path {
          fill: #333333;  
        }
      `
    :
      css`
        .social_button_text {  
          color: #D0D5DA;
        }

        svg path {
          fill: #D0D5DA; 
        }
      `
  };

  ${props =>
    props.isChecked && !props.isDisabled &&
      css`
        .social_button_text {  
          color: #FFFFFF;
        }

        svg path {
          fill: #FFFFFF;  
        }
      `
  };
`; 

const ToggleContainer = styled.button.attrs((props) => ({
  disabled: props.isDisabled ? 'disabled' : '',
  tabIndex: props.tabIndex
}))`
  width: 50px;
  border: 1px solid rgba(0, 0, 0, 0);
  background-clip: padding-box !important;
  border-radius: 0 3px 3px 0;

  &:focus {
    outline: none;
  }

  svg {
    margin: 6px 10px;
    width:  28px;
    height: 16px;
  }

  ${props =>
    !props.isDisabled ?
      css`
        svg rect {
          fill: #A3A9AE;
        }

        svg path {
          fill: white;
        }
      `
    :
      css`
        svg rect {
          fill: #D0D5DA;
        }

        svg path {
          fill: white;
        }
      `
  };

  ${props =>
    props.isChecked && !props.isDisabled &&
      css`
        svg rect {
          fill: #FFFFFF;
        }

        svg path {
          fill: #2DA7DB;
        }
      `
  };  
`; 

const StyledSocialButtonWithToggle = styled.div`
  display: flex;
  width: 162px;
  height: 38px;
  margin: 20px 0 0 20px;
  ${partOfButton}
`;

const Toggle = ({ isChecked }) => {
  const iconName = isChecked ? "ToggleButtonCheckedIcon" : "ToggleButtonIcon";
  return <>{React.createElement(Icons[iconName])}</>;
};

class SocialButtonWithToggle extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      isChecked: this.props.isChecked
    };
  }  

  handleClick = (isChecked) => this.setState({ isChecked: isChecked });

  componentDidUpdate(prevProps) {
    if (this.props.isChecked !== prevProps.isChecked) {
      this.setState({ isChecked: this.props.isChecked });
    }
  }

  render() {
    var {label, iconName, isDisabled, onClick, onChange} = this.props;

    return (
      <StyledSocialButtonWithToggle isDisabled={isDisabled} isChecked={this.state.isChecked}>
        <ButtonContainer className="part-of-button button" isDisabled={isDisabled} isChecked={this.state.isChecked} onClick={onClick}>
          {React.createElement(Icons[iconName], {})}

          {label && (
            <Text.Body as="span" className="social_button_text" fontSize={14}>{label}</Text.Body>
          )} 
        </ButtonContainer>

        <ToggleContainer className="part-of-button toggle" isDisabled={isDisabled} isChecked={this.state.isChecked} 
          onClick={() => {
            this.handleClick(!this.state.isChecked);
            onChange(!this.state.isChecked);
          }}
        >
          <Toggle isChecked={this.state.isChecked}/>
        </ToggleContainer>  
      </StyledSocialButtonWithToggle>
    );
  }
}

SocialButtonWithToggle.propTypes = {
  label: PropTypes.string,
  iconName: PropTypes.string,
  tabIndex: PropTypes.number,
  isDisabled: PropTypes.bool,
  isChecked: PropTypes.bool,
  onClick: PropTypes.func,
  onChange: PropTypes.func
};

SocialButtonWithToggle.defaultProps = {
  label: '',
  iconName: 'ShareFacebookIcon',
  tabIndex: -1,
  isDisabled: false,
  isChecked: false
};

export default SocialButtonWithToggle;
