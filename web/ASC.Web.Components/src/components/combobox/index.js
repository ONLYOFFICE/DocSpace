import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import { handleAnyClick } from '../../utils/event';
import isEqual from 'lodash/isEqual';
import ComboButton from './sub-components/combo-button'

const StyledComboBox = styled.div`
  width: ${props =>
    (props.scaled && '100%') ||
    (props.size === 'base' && '173px') ||
    (props.size === 'middle' && '300px') ||
    (props.size === 'big' && '350px') ||
    (props.size === 'huge' && '500px') ||
    (props.size === 'content' && 'fit-content')
  };

  position: relative;
`;

class ComboBox extends React.Component {
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

  handleClick = (e) =>{
    if (this.state.isOpen && !this.ref.current.contains(e.target)) {
      this.toggle(false);
    }
  }

  stopAction = (e) => e.preventDefault();

  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  comboBoxClick = (e) => {
    if (this.props.isDisabled || e && e.target.closest('.optionalBlock')) return;
    this.toggle(!this.state.isOpen);
  };

  optionClick = (option) => {
    this.setState({
      isOpen: !this.state.isOpen,
      selectedOption: option
    });

    this.props.onSelect && this.props.onSelect(option);
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

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
  }

  render() {
    //console.log("ComboBox render");
    const {
      dropDownMaxHeight,
      directionX,
      directionY,
      scaled,
      size,
      options,
      advancedOptions,
      isDisabled,
      children,
      noBorder,
      scaledOptions } = this.props;
    const { isOpen, selectedOption } = this.state;

    const dropDownMaxHeightProp = dropDownMaxHeight
      ? { maxHeight: dropDownMaxHeight }
      : {};
    const dropDownManualWidthProp = scaledOptions
      ? { manualWidth: '100%' }
      : {};

    const optionsLength = options.length
      ? options.length
      : 0;

    const advancedOptionsLength = advancedOptions
      ? advancedOptions.props.children.length
      : 0;

    return (
      <StyledComboBox
        ref={this.ref}
        isDisabled={isDisabled}
        scaled={scaled}
        size={size}
        data={selectedOption}
        onClick={this.comboBoxClick}
        {...this.props}
      >
        <ComboButton
          noBorder={noBorder}
          isDisabled={isDisabled}
          selectedOption={selectedOption}
          withOptions={optionsLength > 0}
          optionsLength={optionsLength}
          withAdvancedOptions={advancedOptionsLength > 0}
          innerContainer={children}
          innerContainerClassName='optionalBlock'
          isOpen={isOpen}
          size={size}
          scaled={scaled}
        />
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
                key={option.key}
                disabled={
                  option.disabled
                  || (option.label === selectedOption.label)}
                onClick={this.optionClick.bind(this, option)}
              />
            )}
        </DropDown>
      </StyledComboBox>
    );
  }
}

ComboBox.propTypes = {
  noBorder: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedOption: PropTypes.object.isRequired,

  options: PropTypes.array.isRequired,
  advancedOptions: PropTypes.element,
  children: PropTypes.any,
  opened: PropTypes.bool,

  onSelect: PropTypes.func,
  dropDownMaxHeight: PropTypes.number,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge', 'content']),
  directionX: PropTypes.oneOf(['left', 'right']),
  directionY: PropTypes.oneOf(['bottom', 'top']),
  scaled: PropTypes.bool,
  scaledOptions: PropTypes.bool
}

ComboBox.defaultProps = {
  noBorder: false,
  isDisabled: false,
  size: 'base',
  scaled: true,
  scaledOptions: false
}

export default ComboBox;