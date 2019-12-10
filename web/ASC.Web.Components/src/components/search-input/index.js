import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';

import isEqual from 'lodash/isEqual';

const StyledSearchInput = styled.div`
  font-family: Open Sans;
  font-style: normal;
`;

class SearchInput extends React.Component {
  constructor(props) {
    super(props);

    this.input = React.createRef();
    this.timerId = null;

    this.state = {
      inputValue: props.value,
    };

    this.clearSearch = this.clearSearch.bind(this);
    this.onInputChange = this.onInputChange.bind(this);
    this.setSearchTimer = this.setSearchTimer.bind(this);
  }

  clearSearch() {
    this.setState({
      inputValue: ''
    });
    if (typeof this.props.onClearSearch === 'function') this.props.onClearSearch();
  }

  onInputChange(e) {
    this.setState({
      inputValue: e.target.value
    });
    if (this.props.autoRefresh)
      this.setSearchTimer(e.target.value);
  }

  setSearchTimer(value) {
    this.timerId && clearTimeout(this.timerId);
    this.timerId = null;
    this.timerId = setTimeout(() => {
      this.props.onChange(value);
      clearTimeout(this.timerId);
      this.timerId = null;
    }, this.props.refreshTimeout);
  }
  
  shouldComponentUpdate(nextProps, nextState) {
    if (this.props.value != nextProps.value) {
      this.setState({ inputValue: nextProps.value });
      return true;
    }

    return (!isEqual(this.state, nextState) || !isEqual(this.props, nextProps));
  }

  render() {
    //console.log("Search input render");
    let clearButtonSize = 15;
    switch (this.props.size) {
      case 'base':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton > 0 ? 12 : 15;
        break;
      case 'middle':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton > 0 ? 16 : 18;
        break;
      case 'big':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton > 0 ? 19 : 21;
        break;
      case 'huge':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton > 0 ? 22 : 24;
        break;
      default:
        break;
    }

    return (
      <StyledSearchInput className={this.props.className} style={this.props.style}>
        <InputBlock
          ref={this.input}
          id={this.props.id}
          name={this.props.name}
          isDisabled={this.props.isDisabled}
          iconName={!!this.state.inputValue || this.props.showClearButton > 0 ? "CrossIcon" : "SearchIcon"}
          isIconFill={true}
          iconSize={clearButtonSize}
          iconColor={"#A3A9AE"}
          onIconClick={!!this.state.inputValue || this.props.showClearButton ? this.clearSearch : undefined}
          size={this.props.size}
          scale={true}
          value={this.state.inputValue}
          placeholder={this.props.placeholder}
          onChange={this.onInputChange}
        >
          {this.props.children}

        </InputBlock>
      </StyledSearchInput>
    );
  }
}

SearchInput.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  className: PropTypes.string,
  size: PropTypes.oneOf(['base', 'middle', 'big', 'huge']),
  value: PropTypes.string,
  scale: PropTypes.bool,
  placeholder: PropTypes.string,
  onChange: PropTypes.func,
  onClearSearch: PropTypes.func,
  isDisabled: PropTypes.bool,
  showClearButton: PropTypes.bool,
  refreshTimeout: PropTypes.number,
  autoRefresh: PropTypes.bool,
  children: PropTypes.any,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

SearchInput.defaultProps = {
  autoRefresh: true,
  size: 'base',
  value: '',
  scale: false,
  isDisabled: false,
  refreshTimeout: 1000,
  showClearButton: false
};

export default SearchInput;