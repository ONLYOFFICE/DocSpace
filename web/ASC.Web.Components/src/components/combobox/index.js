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
  border: 1px solid #D0D5DA;
  border-radius: 3px;

  ${props => props.isDisabled && `
    border-color: #ECEEF1;
    background: #F8F9F9;
  `}

  height: 32px;

  :hover{
    border-color: ${state => state.isOpen ? '#2DA7DB' : '#A3A9AE'};

    ${props => props.isDisabled && `
      border-color: #ECEEF1;
  `}
  }
`;

const StyledComboButton = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  height: 30px;
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

margin-right: 8px;
`;

const StyledArrowIcon = styled.div`
  width: 8px;
  margin-right: 8px;
  margin-left: auto;

  ${state => state.isOpen && `
    transform: scale(1, -1);
    margin-top: 8px;
  `}
`;

class ComboBox extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    const selectedItem = this.getSelected();

    this.state = {
      isOpen: props.opened,
      boxLabel: selectedItem && selectedItem.label,
      boxIcon: selectedItem && selectedItem.icon,
      options: props.options
    };

    this.handleClick = this.handleClick.bind(this);
    this.stopAction = this.stopAction.bind(this);
    this.toggle = this.toggle.bind(this);
    this.comboBoxClick = this.comboBoxClick.bind(this);
    this.optionClick = this.optionClick.bind(this);

    if (props.opened)
      handleAnyClick(true, this.handleClick);
  }

  handleClick = (e) => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);

  stopAction = (e) => e.preventDefault();

  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  comboBoxClick = (e) => {
    if (this.props.isDisabled || e.target.closest('.optionalBlock')) return;
    
    this.setState({
      option: this.props.option,
      isOpen: !this.state.isOpen
    });
  };

  optionClick = (option) => {
    this.setState({
      boxLabel: option.label,
      boxIcon: option.icon,
      isOpen: !this.state.isOpen
    });
    this.props.onSelect && this.props.onSelect(option);
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  getSelected = () => {
    const selectedItem = this.props.options.find(x => x.key === this.props.selectedOption)
      || this.props.options[0];

    return selectedItem;
  }

  getSelectedLabel = () => {
    const selectedItem = this.getSelected();

    return selectedItem ? selectedItem.label : this.props.emptyOptionsPlaceholder;
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }

    if (this.props.options.length !== prevProps.options.length) { //TODO: Move options from state
      const label = this.getSelectedLabel();
      this.setState({
        options: this.props.options,
        boxLabel: label
      });
    }

    if (this.props.selectedOption !== prevProps.selectedOption) {
      const label = this.getSelectedLabel();
      this.setState({ boxLabel: label });
    }
  }

  render() {
    console.log("ComboBox render");

    const { dropDownMaxHeight, isDisabled, directionX, directionY, scaled, children } = this.props;
    const { boxLabel, boxIcon, isOpen, options } = this.state;

    const dropDownMaxHeightProp = dropDownMaxHeight ? { maxHeight: dropDownMaxHeight} : {};
    const dropDownManualWidthProp = scaled ? { manualWidth: '100%' } : {};
    const boxIconColor = isDisabled ? '#D0D5DA' : '#333333';
    const arrowIconColor = isDisabled ? '#D0D5DA' : '#A3A9AE';

    return (
      <StyledComboBox ref={this.ref}
        {...this.props}
        {...this.state}
        data={boxLabel}
        onClick={this.comboBoxClick}
        onSelect={this.stopAction}
      >
        <StyledComboButton>
          <StyledOptionalItem className='optionalBlock'>
            {children}
          </StyledOptionalItem>
          {boxIcon &&
            <StyledIcon>
              {React.createElement(Icons[boxIcon],
                {
                  size: 'scale',
                  color: boxIconColor,
                  isfill: true
                })
              }
            </StyledIcon>
          }
          <StyledLabel>
            {boxLabel}
          </StyledLabel>
          <StyledArrowIcon {...this.state}>
            {React.createElement(Icons['ExpanderDownIcon'],
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
          {options.map((option) =>
            <DropDownItem {...option}
              disabled={option.label === boxLabel}
              onClick={this.optionClick.bind(this, option)}
            />
          )}
        </DropDown>
      </StyledComboBox>
    );
  }
};

ComboBox.propTypes = {
  isDisabled: PropTypes.bool,
  withBorder: PropTypes.bool,
  selectedOption: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  options: PropTypes.array,
  onSelect: PropTypes.func,
  dropDownMaxHeight: PropTypes.number,
  emptyOptionsPlaceholder: PropTypes.string,

  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge', 'content']),
  scaled: PropTypes.bool,
}

ComboBox.defaultProps = {
  isDisabled: false,
  withBorder: true,
  emptyOptionsPlaceholder: 'Select',
  size: 'base',
  scaled: true
}

export default ComboBox;