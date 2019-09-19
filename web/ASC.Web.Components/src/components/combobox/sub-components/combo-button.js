import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import { Icons } from '../../icons'

const StyledComboButton = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;

  height: ${props => props.noBorder ? `18px` : `30px`};
  width: ${props =>
    (props.scaled && '100%') ||
    (props.size === 'base' && '173px') ||
    (props.size === 'middle' && '300px') ||
    (props.size === 'big' && '350px') ||
    (props.size === 'huge' && '500px') ||
    (props.size === 'content' && 'fit-content')
  };

  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;

  padding-left: 8px;

  background: ${props => !props.noBorder ? '#FFFFFF' : 'none'};

  color: ${props => props.isDisabled ? '#D0D5DA' : '#333333'};

  ${props => !props.noBorder && `
    border: 1px solid #D0D5DA;
    border-radius: 3px;
  `}
  
  ${props => props.isDisabled && !props.noBorder && `
    border-color: #ECEEF1;
    background: #F8F9F9;
  `}

  ${props => !props.noBorder && `
    height: 32px;
  `}

  :hover{
    border-color: ${props => props.isOpen ? '#2DA7DB' : '#A3A9AE'};
    cursor: ${props => (props.isDisabled || (!props.containOptions && !props.withAdvancedOptions))
      ? 'default'
      : 'pointer'};

    ${props => props.isDisabled && `
      border-color: #ECEEF1;
    `}
  }
`;

const StyledOptionalItem = styled.div`
  margin-right: 8px;
`;

const StyledIcon = styled.div`
  width: 16px;
  margin-right: 8px;
  margin-top: -2px;
`;


const StyledLabel = styled.div`
  font-family: Open Sans;
  font-style: normal;
  font-weight: 600;
  font-size: 13px;

  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  margin-right: 8px;

  ${props => props.noBorder && `
    line-height: 11px;
    border-bottom: 1px dashed transparent;

    :hover{
      border-bottom: 1px dashed;
    }
  `};
`;

const StyledArrowIcon = styled.div`
  display: flex;
  align-self: start;
  width: ${props => props.needDisplay ? '8px' : '0px'};
  flex: 0 0 ${props => props.needDisplay ? '8px' : '0px'};
  margin-top: ${props => props.noBorder ? `5px` : `12px`};
  margin-right: ${props => props.needDisplay ? '8px' : '0px'};
  margin-left: ${props => props.needDisplay ? 'auto' : '0px'};

  ${props => props.isOpen && `
    transform: scale(1, -1);
  `}
`;

class ComboButton extends React.Component {
  render() {
    const {
      noBorder,
      onClick,
      isDisabled,
      innerContainer,
      innerContainerClassName,
      selectedOption,
      optionsLength,
      withOptions,
      withAdvancedOptions,
      isOpen,
      scaled,
      size } = this.props;

    const boxIconColor = isDisabled ? '#D0D5DA' : '#333333';
    const arrowIconColor = isDisabled ? '#D0D5DA' : '#A3A9AE';

    return (
      <StyledComboButton
        isOpen={isOpen}
        isDisabled={isDisabled}
        noBorder={noBorder}
        containOptions={optionsLength}
        withAdvancedOptions={withAdvancedOptions}
        onClick={onClick}
        scaled={scaled}
        size={size}
      >
        {innerContainer &&
          <StyledOptionalItem className={innerContainerClassName}>
            {innerContainer}
          </StyledOptionalItem>
        }
        {selectedOption && selectedOption.icon &&
          <StyledIcon>
            {React.createElement(Icons[selectedOption.icon],
              {
                size: 'scale',
                color: boxIconColor,
                isfill: true
              })
            }
          </StyledIcon>
        }
        <StyledLabel noBorder={noBorder}>
          {selectedOption.label}
        </StyledLabel>
        <StyledArrowIcon
          needDisplay={withOptions || withAdvancedOptions}
          noBorder={noBorder}
          isOpen={isOpen}>
          {(withOptions || withAdvancedOptions) &&
            React.createElement(Icons['ExpanderDownIcon'],
              {
                size: 'scale',
                color: arrowIconColor,
                isfill: true
              })
          }
        </StyledArrowIcon>
      </StyledComboButton>
    );
  }
}

ComboButton.propTypes = {
  noBorder: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedOption: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.object),
    PropTypes.object
  ]),
  withOptions: PropTypes.bool,
  optionsLength: PropTypes.number,
  withAdvancedOptions: PropTypes.bool,
  innerContainer: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  innerContainerClassName: PropTypes.string,
  isOpen: PropTypes.bool,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge', 'content']),
  scaled: PropTypes.bool,
  onClick: PropTypes.func
}

ComboButton.defaultProps = {
  noBorder: false,
  isDisabled: false,
  withOptions: true,
  withAdvancedOptions: false,
  innerContainerClassName: 'innerContainer',
  isOpen: false,
  size: 'content',
  scaled: false
}

export default ComboButton;