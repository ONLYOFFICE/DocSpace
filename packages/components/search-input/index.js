import React from "react";
import PropTypes from "prop-types";
import CrossIconReactSvgUrl from "PUBLIC_DIR/images/cross.react.svg?url";
import SearchIconReactSvgUrl from "PUBLIC_DIR/images/search.react.svg?url";
import InputBlock from "../input-block";
import StyledSearchInput from "./styled-search-input";
import { ReactSVG } from "react-svg";

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
      inputValue: "",
    });
    typeof this.props.onClearSearch === "function" &&
      this.props.onClearSearch();
  };

  onInputChange = (e) => {
    this.setState({
      inputValue: e.target.value,
    });
    if (this.props.autoRefresh) this.setSearchTimer(e.target.value);
  };

  setSearchTimer = (value) => {
    clearTimeout(this.timerId);
    this.timerId = setTimeout(() => {
      this.props.onChange(value);
      clearTimeout(this.timerId);
      this.timerId = null;
    }, this.props.refreshTimeout);
  };
  componentDidUpdate(prevProps) {
    if (this.props.value != prevProps.value) {
      this.setState({ inputValue: this.props.value });
      return true;
    }
  }

  render() {
    //console.log("Search input render");
    let clearButtonSize = 16;
    switch (this.props.size) {
      case "base":
        clearButtonSize =
          !!this.state.inputValue || this.props.showClearButton ? 12 : 16;
        break;
      case "middle":
        clearButtonSize =
          !!this.state.inputValue || this.props.showClearButton ? 16 : 18;
        break;
      case "big":
        clearButtonSize =
          !!this.state.inputValue || this.props.showClearButton ? 18 : 22;
        break;
      case "huge":
        clearButtonSize =
          !!this.state.inputValue || this.props.showClearButton ? 22 : 24;
        break;
    }

    const showCrossIcon = !!this.state.inputValue || this.props.showClearButton;

    const iconNode = (
      <>
        {showCrossIcon && (
          <ReactSVG
            className="icon-button_svg not-selectable"
            src={CrossIconReactSvgUrl}
          />
        )}

        {!showCrossIcon && (
          <ReactSVG
            className="icon-button_svg not-selectable"
            src={SearchIconReactSvgUrl}
          />
        )}
      </>
    );

    return (
      <StyledSearchInput
        theme={this.props.theme}
        className={this.props.className}
        style={this.props.style}
      >
        <InputBlock
          theme={this.props.theme}
          className="search-input-block"
          ref={this.input}
          id={this.props.id}
          name={this.props.name}
          isDisabled={this.props.isDisabled}
          iconNode={iconNode}
          iconButtonClassName={
            !!this.state.inputValue || this.props.showClearButton
              ? "search-cross"
              : "search-loupe"
          }
          isIconFill={true}
          iconSize={clearButtonSize}
          onIconClick={
            !!this.state.inputValue || this.props.showClearButton
              ? this.clearSearch
              : undefined
          }
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
  /** Used as HTML `id` property */
  id: PropTypes.string,
  /** Sets the unique element name */
  name: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Supported size of the input fields. */
  size: PropTypes.oneOf(["base", "middle", "big", "huge"]),
  /** Input value */
  value: PropTypes.string,
  /** Indicates that the input field has scale  */
  scale: PropTypes.bool,
  /** Placeholder text for the input */
  placeholder: PropTypes.string,
  /** Sets a callback function that allows handling the component's changing events */
  onChange: PropTypes.func,
  /** Sets a callback function that is triggered when the clear icon of the search input is clicked */
  onClearSearch: PropTypes.func,
  /** Indicates that the field cannot be used (e.g not authorized, or the changes have not been saved) */
  isDisabled: PropTypes.bool,
  /** Displays the Clear Button */
  showClearButton: PropTypes.bool,
  /** Sets the refresh timeout of the input  */
  refreshTimeout: PropTypes.number,
  /** Sets the input to refresh automatically */
  autoRefresh: PropTypes.bool,
  /** Child elements */
  children: PropTypes.any,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

SearchInput.defaultProps = {
  autoRefresh: true,
  size: "base",
  value: "",
  scale: false,
  isDisabled: false,
  refreshTimeout: 1000,
  showClearButton: false,
};

export default SearchInput;
