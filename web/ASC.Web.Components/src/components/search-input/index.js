import React from "react";
import PropTypes from "prop-types";
import styled from 'styled-components';
import InputBlock from '../input-block';

const StyledSearchInput = styled.div`
  font-family: Open Sans;
  font-style: normal;

  .search-input-block {
    & > input { 
      font-size: 14px;
      font-weight: 600;
    }
  }
`;

class SearchInput extends React.Component {
  constructor(props) {
    super(props);

    this.input = React.createRef();
    this.timerId = null;

    this.state = {
      inputValue: props.value,
    };
  }

  clearSearch = () => {
    this.setState({
      inputValue: ''
    });
    typeof this.props.onClearSearch === 'function' && this.props.onClearSearch();
  }

  onInputChange = (e) => {
    this.setState({
      inputValue: e.target.value
    });
    if (this.props.autoRefresh)
      this.setSearchTimer(e.target.value);
  }

  setSearchTimer = (value) => {
    clearTimeout(this.timerId);
    this.timerId = setTimeout(() => 
      {
        this.props.onChange(value);
        clearTimeout(this.timerId);
        this.timerId = null;
      }, 
      this.props.refreshTimeout
    );
  }
  componentDidUpdate(prevProps) {
    if (this.props.value != prevProps.value) {
      this.setState({ inputValue: this.props.value });
      return true;
    }
  }

  render() {
    //console.log("Search input render");
    let clearButtonSize = 15;
    switch (this.props.size) {
      case 'base':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton ? 12 : 15;
        break;
      case 'middle':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton ? 16 : 18;
        break;
      case 'big':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton ? 19 : 21;
        break;
      case 'huge':
        clearButtonSize = !!this.state.inputValue || this.props.showClearButton ? 22 : 24;
        break;
    }

    return (
      <StyledSearchInput className={this.props.className} style={this.props.style}>
        <InputBlock
          className='search-input-block'
          ref={this.input}
          id={this.props.id}
          name={this.props.name}
          isDisabled={this.props.isDisabled}
          iconName={!!this.state.inputValue || this.props.showClearButton ? "CrossIcon" : "SearchIcon"}
          isIconFill={true}
          iconSize={clearButtonSize}
          iconColor={"#D0D5DA"}
          hoverColor={!!this.state.inputValue || this.props.showClearButton ? "#555F65" : "#D0D5DA"}
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