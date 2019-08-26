import React, { memo } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import SearchInput from "../search-input";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { FixedSizeList } from "react-window";
import Link from "../link";
import Button from "../button";

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

    this.state = {};
  }

  onButtonClick = () => {};

  onSelect = option => {
    if (!this.props.isMultiSelect) this.props.onSelect(option);
  };

  renderRow = ({ data, index, style }) => {
    const option = data[index];

    return (
      <div class="option" style={style}>
        <Link as="span" truncate={true} onClick={this.onSelect.bind(this, option)}>
          {option.label}
        </Link>
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
      selectedOptions,
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
          stype="smallBlack"
          height={maxHeight}
          itemSize={32}
          itemCount={options.length}
          itemData={options}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {this.renderRow}
        </FixedSizeList>
        {isMultiSelect && (
          <Button
            className="add_members_btn"
            primary={true}
            size="big"
            label={buttonLabel}
            scale={true}
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
