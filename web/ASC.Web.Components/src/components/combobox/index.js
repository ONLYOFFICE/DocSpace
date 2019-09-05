import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import { Icons } from '../icons'
import { handleAnyClick } from '../../utils/event';

const StyledComboBox = styled.div`
  color: ${props => props.isDisabled ? '#D0D5DA' : '#333333'};
  width: ${props =>
    (props.scaled && '100%') ||
    (props.size === 'base' && '173px') ||
    (props.size === 'middle' && '300px') ||
    (props.size === 'big' && '350px') ||
    (props.size === 'huge' && '500px') ||
    (props.size === 'content' && 'fit-content')
  };

  position: relative;
  
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;

  background: #FFFFFF;

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
    border-color: ${state => state.isOpen ? '#2DA7DB' : '#A3A9AE' };
    cursor: ${props => (props.isDisabled || !props.options.length ) ? (props.advancedOptions) ? 'pointer' : 'default' : 'pointer'};

    ${props => props.isDisabled && `
      border-color: #ECEEF1;
    `}
  }
`;

const StyledComboButton = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;

  height: ${props => props.noBorder ? `18px` : `30px`};
  margin-left: 8px;
`;

const StyledIcon = styled.div`
  width: 16px;
  margin-right: 8px;
  margin-top: -2px;
`;

const StyledOptionalItem = styled.div`
margin-right: 8px;
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

class ComboBox extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
      selectedOption: props.selectedOption
    };

    if (props.opened)
      handleAnyClick(true, this.handleClick);
  }

  handleClick = (e) =>
    this.state.isOpen
    && !this.ref.current.contains(e.target)
    && this.toggle(false);

  stopAction = (e) => e.preventDefault();

  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  comboBoxClick = (e) => {
    if (this.props.isDisabled || e.target.closest('.optionalBlock')) return;
    this.toggle(!this.state.isOpen);
  };

  optionClick = (option) => {
    this.toggle(!this.state.isOpen);
    this.setState({
      isOpen: !this.state.isOpen,
      selectedOption: option
    });
    this.props.onSelect && this.props.onSelect(option);
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  };

  componentDidUpdate(prevProps, prevState) {
    if (this.props.opened !== prevProps.opened) {
      handleAnyClick(this.props.opened, this.handleClick);
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }

    if (this.props.selectedOption !== prevProps.selectedOption) {
      this.setState({ selectedOption: this.props.selectedOption });
    }
  };

  render() {
    //console.log("ComboBox render");
    const {
      dropDownMaxHeight, 
      isDisabled, 
      directionX, 
      directionY, 
      scaled, 
      children, 
      options, 
      noBorder, 
      advancedOptions 
    } = this.props;
    const { isOpen, selectedOption } = this.state;

    const dropDownMaxHeightProp = dropDownMaxHeight ? { maxHeight: dropDownMaxHeight } : {};
    const dropDownManualWidthProp = scaled ? { manualWidth: '100%' } : {};
    const boxIconColor = isDisabled ? '#D0D5DA' : '#333333';
    const arrowIconColor = isDisabled ? '#D0D5DA' : '#A3A9AE';
   
    return (
      <StyledComboBox ref={this.ref}
        {...this.props}
        {...this.state}
        data={selectedOption}
        onClick={this.comboBoxClick}
        onSelect={this.stopAction}
      >
        <StyledComboButton noBorder={noBorder}>
          {children &&
            <StyledOptionalItem className='optionalBlock'>
              {children}
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
          <StyledArrowIcon needDisplay={options.length > 0 || advancedOptions !== undefined} noBorder={noBorder} isOpen={isOpen}>
            {(options.length > 0 || advancedOptions !== undefined) &&
              React.createElement(Icons['ExpanderDownIcon'],
                {
                  size: 'scale',
                  color: arrowIconColor,
                  isfill: true
                })
            }
          </StyledArrowIcon>
        </StyledComboButton>
        <DropDown
          directionX={directionX}
          directionY={directionY}
          manualY='102%'
          isOpen={isOpen}
          {...dropDownMaxHeightProp}
          {...dropDownManualWidthProp}
        >
          {advancedOptions 
          ? advancedOptions 
          : options.map((option) =>
            <DropDownItem {...option}
              disabled={option.disabled || (option.label === selectedOption.label)}
              onClick={this.optionClick.bind(this, option)}
            />
          )}
        </DropDown>
      </StyledComboBox>
    );
  }
};

ComboBox.propTypes = {
  noBorder: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedOption: PropTypes.object.isRequired,

  options: PropTypes.array.isRequired,
  advancedOptions: PropTypes.element,

  onSelect: PropTypes.func,
  dropDownMaxHeight: PropTypes.number,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge', 'content']),
  scaled: PropTypes.bool
}

ComboBox.defaultProps = {
  noBorder: false,
  isDisabled: false,
  size: 'base',
  scaled: true
}

export default ComboBox;