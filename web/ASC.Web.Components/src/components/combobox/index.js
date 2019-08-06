import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import InputBlock from '../input-block'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import { handleAnyClick } from '../../utils/event';

const StyledComboBox = styled.div`
    & > div {
        ${props => !props.withBorder && `border-color: transparent;`} 
    }
     
    & > div > input {
        ${props => !props.isDisabled && `cursor: pointer !important;`}
        ${props => !props.withBorder && `border-color: transparent !important;`}
        
        &::placeholder {
            font-family: Open Sans;
            font-style: normal;
            font-weight: 600;
            font-size: 13px;
            line-height: 20px;
            ${props => !props.isDisabled && `color: #333333;`}
            ${props => (!props.withBorder & !props.isDisabled) && `border-bottom: 1px dotted #333333;`}
            opacity: 1;
        }
    }
`;

class ComboBox extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    const selectedItem = this.props.options.find(x => x.key === this.props.selectedOption) || this.props.options[0];
    console.log("combobox constructor label", selectedItem.label);

    this.state = {
      isOpen: props.opened,
      boxLabel: selectedItem.label,
      options: props.options
    };

    this.handleClick = this.handleClick.bind(this);
    this.stopAction = this.stopAction.bind(this);
    this.toggle = this.toggle.bind(this);
    this.comboBoxClick = this.comboBoxClick.bind(this);
    this.optionClick = this.optionClick.bind(this);

    if(props.opened)
      handleAnyClick(true, this.handleClick);
  }

  handleClick = (e) => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);

  stopAction = (e) => e.preventDefault();

  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  comboBoxClick = (e) => {
    if (!!e.target.closest('.input-group-prepend')) return;
    
    this.setState({ 
      option: this.props.option,
      isOpen: !this.state.isOpen
    });
  };

  optionClick = (option) => {
    this.setState({ 
      boxLabel: option.label,
      isOpen: !this.state.isOpen
    });
    this.props.onSelect && this.props.onSelect(option);
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if(this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }

    if(this.props.selectedOption !== prevProps.selectedOption) {
      const label = this.props.options.find(x => x.key === this.props.selectedOption).label;
      console.log("combobox componentDidUpdate label", label, this.props.selectedOption);
      this.setState({boxLabel: label } );
    }
  }

  render() {
    console.log("ComboBox render");
    return (
      <StyledComboBox ref={this.ref} {...this.props} data={this.state.boxLabel} onClick={this.comboBoxClick} onSelect={this.stopAction} >
        <InputBlock placeholder={this.state.boxLabel}
          iconName='ExpanderDownIcon'
          iconSize={8}
          isIconFill={true}
          iconColor='#A3A9AE'
          scale={true}
          isDisabled={this.props.isDisabled}
          isReadOnly={true}
        >
          {this.props.children}
          <DropDown
            directionX={this.props.directionX}
            directionY={this.props.directionY}
            manualWidth='100%'
            manualY='102%'
            isOpen={this.state.isOpen}
          >
            {this.state.options.map((option) =>
              <DropDownItem {...option}
                disabled={option.label === this.state.boxLabel}
                onClick={this.optionClick.bind(this, option)}
              />
            )}
          </DropDown>
        </InputBlock>
      </StyledComboBox>
    );
  }
};

ComboBox.propTypes = {
  isDisabled: PropTypes.bool,
  withBorder: PropTypes.bool,
  selectedOption: PropTypes.any,
  options: PropTypes.array,
  onSelect: PropTypes.func
}

ComboBox.defaultProps = {
  isDisabled: false,
  withBorder: true
}

export default ComboBox;