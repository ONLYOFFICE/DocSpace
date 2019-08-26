import React, { memo } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import SearchInput from "../search-input";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import Link from "../link";
import Checkbox from "../checkbox";
import Button from "../button";
import { isArrayEqual } from "../../utils/array";
import findIndex from "lodash/findIndex";
import filter from "lodash/filter";

const Container = ({
  value,
  placeholder,
  isMultiSelect,
  mode,
  width,
  maxHeight,
  isDisabled,
  onSearchChanged,
  options,
  selectedOptions,
  buttonLabel,
  ...props
}) => <div {...props} />;

const StyledContainer = styled(Container)`
  ${props => (props.width ? `width: ${props.width}px;` : "")}

  .options_searcher {
    margin-bottom: 12px;
  }

  .options_list {
    .option {
      line-height: 32px;
      cursor: pointer;

      .option_checkbox {
        margin-left: 8px;
      }

      .option_link {
        padding-left: 8px;
      }

      &:hover {
        background-color: #eceef1;
      }
    }
  }

  .add_members_btn {
    margin: 16px 0;
  }
`;

/*const Row = memo(({ data, index, style }) => {
  const option = data[index];

  return (
    <div class="option" style={style}>
      <Link as="span" truncate={true}>
        {option.label}
      </Link>
    </div>
  );
});*/

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      selectedOptions: this.props.selectedOptions || []
    };
  }

  componentDidUpdate(prevProps) {
    if (!isArrayEqual(this.props.selectedOptions, prevProps.selectedOptions)) {
      this.setState({ selectedOptions: this.props.selectedOptions });
    }

    if(this.props.isMultiSelect !== prevProps.isMultiSelect) {
      this.setState({ selectedOptions: [] });
    }
  }

  onButtonClick = () => {
    this.props.onSelect && this.props.onSelect(this.state.selectedOptions);
  };

  onSelect = option => {
    this.props.onSelect && this.props.onSelect(option);
  };

  onChange = (option, e) => {
    const newSelectedOptions = e.target.checked
              ? [...this.state.selectedOptions, option] 
              : filter(this.state.selectedOptions, (obj) => obj.key !== option.key);

    //console.log("onChange", option, e.target.checked, newSelectedOptions);

    this.setState({
      selectedOptions: newSelectedOptions
    });
  };

  renderRow = ({ data, index, style }) => {
    const option = data[index];
    var isChecked = findIndex(this.state.selectedOptions, { key: option.key }) > -1;

    //console.log("renderRow", option, isChecked, this.state.selectedOptions);
    return (
      <div className="option" style={style} key={option.key}>
        {this.props.isMultiSelect ? (
          <Checkbox
            label={option.label}
            isChecked={isChecked}
            className="option_checkbox"
            onChange={this.onChange.bind(this, option)}
          />
        ) : (
          <Link
            as="span"
            truncate={true}
            className="option_link"
            onClick={this.onSelect.bind(this, option)}
          >
            {option.label}
          </Link>
        )}
      </div>
    );
  };

  render() {
    const {
      value,
      placeholder,
      maxHeight,
      isDisabled,
      onSearchChanged,
      options,
      isMultiSelect,
      buttonLabel
    } = this.props;

    return (
      <StyledContainer {...this.props}>
        <SearchInput
          className="options_searcher"
          isDisabled={isDisabled}
          size="base"
          scale={true}
          isNeedFilter={false}
          placeholder={placeholder}
          value={value}
          onChange={onSearchChanged}
        />
        <FixedSizeList
          className="options_list"
          height={maxHeight}
          itemSize={32}
          itemCount={options.length}
          itemData={options}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {this.renderRow.bind(this)}
        </FixedSizeList>
        {isMultiSelect && (
          <Button
            className="add_members_btn"
            primary={true}
            size="big"
            label={buttonLabel}
            scale={true}
            isDisabled={!this.state.selectedOptions || !this.state.selectedOptions.length}
            onClick={this.onButtonClick}
          />
        )}
      </StyledContainer>
    );
  }
}

AdvancedSelector.propTypes = {
  value: PropTypes.string,
  placeholder: PropTypes.string,
  isMultiSelect: PropTypes.bool,
  mode: PropTypes.oneOf(["base", "compact"]),
  width: PropTypes.number,
  maxHeight: PropTypes.number,
  isDisabled: PropTypes.bool,
  onSearchChanged: PropTypes.func,
  options: PropTypes.array.isRequired,
  selectedOptions: PropTypes.array,
  buttonLabel: PropTypes.string,
  onSelect: PropTypes.func
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  width: 325,
  maxHeight: 545,
  mode: "base",
  buttonLabel: "Add members"
};

export default AdvancedSelector;
