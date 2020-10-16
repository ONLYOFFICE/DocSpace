import React from "react";
import isEqual from "lodash/isEqual";
import {
  ComboBox,
  IconButton,
  DropDownItem,
  RadioButtonGroup,
} from "asc-web-components";
import PropTypes from "prop-types";
import { StyledIconButton } from "../StyledFilterInput";

class SortComboBox extends React.Component {
  constructor(props) {
    super(props);

    const { sortDirection } = props;

    this.state = {
      sortDirection,
      opened: false,
    };

    this.combobox = React.createRef();
  }
  onButtonClick = () => {
    const { onChangeSortDirection } = this.props;
    const { sortDirection } = this.state;
    typeof onChangeSortDirection === "function" &&
      onChangeSortDirection(+(sortDirection === 0 ? 1 : 0));
    this.setState({
      sortDirection: sortDirection === 0 ? 1 : 0,
    });
  };

  onChangeSortId = (e) => {
    const { onChangeSortId } = this.props;
    typeof onChangeSortId === "function" && onChangeSortId(e.target.value);
    this.setState({ opened: !this.state.opened });
  };

  onChangeView = (e) => {
    const { onChangeView } = this.props;
    typeof onChangeView === "function" && onChangeView(e.target.value);
  };

  onChangeSortDirection = (e) => {
    const sortDirection = +e.target.value;
    const { onChangeSortDirection } = this.props;
    this.setState({ sortDirection, opened: !this.state.opened });
    typeof onChangeSortDirection === "function" &&
      onChangeSortDirection(sortDirection);
  };

  onToggleAction = () => {
    this.setState({
      opened: !this.state.opened,
    });
  };

  shouldComponentUpdate(nextProps, nextState) {
    if(!isEqual(this.props, nextProps) || !isEqual(this.state, nextState)) {
      return true;
    }
    return false;
  }

  componentDidUpdate(prevProps, prevState) {
    const { sortDirection } = this.props;

    if (sortDirection !== prevProps.sortDirection) {
      this.setState({ sortDirection });
    }

  };

  render() {
    //console.log("SortComboBox render");
    const {
      directionAscLabel,
      directionDescLabel,
      isDisabled,
      viewAs,
      getSortData,
      sortId,
    } = this.props;
    const { sortDirection, opened } = this.state;

    const options = getSortData();
    const selectedOption =
      options.length > 0 ? options.find((x) => x.key === sortId) : {};

    let settingsArray = options.filter((item) => {
      item.value = item.key;
      return item.isSetting === true;
    });

    let sortArray = options.filter((item) => {
      item.value = item.key;
      return item.isSetting !== true;
    });

    let sortDirectionArray = [
      { value: "0", label: directionAscLabel },
      { value: "1", label: directionDescLabel },
    ];

    const advancedOptions = (
      <>
        <DropDownItem noHover>
          <RadioButtonGroup
            fontWeight={600}
            isDisabled={isDisabled}
            name={"direction"}
            onClick={this.onChangeSortDirection}
            options={sortDirectionArray}
            orientation="vertical"
            selected={sortDirection.toString()}
            spacing="0px"
          />
        </DropDownItem>
        <DropDownItem isSeparator />
        <DropDownItem noHover>
          <RadioButtonGroup
            fontWeight={600}
            isDisabled={isDisabled}
            name={"sort"}
            onClick={this.onChangeSortId}
            options={sortArray}
            orientation="vertical"
            selected={selectedOption.key}
            spacing="0px"
          />
        </DropDownItem>
        {settingsArray.length !== 0 && viewAs && (
          <>
            <DropDownItem isSeparator />
            <DropDownItem noHover>
              <RadioButtonGroup
                fontWeight={600}
                isDisabled={isDisabled}
                name={"view"}
                onClick={this.onChangeView}
                options={settingsArray}
                orientation="vertical"
                selected={viewAs}
                spacing="0px"
              />
            </DropDownItem>
          </>
        )}
      </>
    );
    return (
      <ComboBox
        opened={opened}
        toggleAction={this.onToggleAction}
        advancedOptions={advancedOptions}
        className="styled-sort-combobox"
        directionX="right"
        isDisabled={isDisabled}
        options={[]}
        ref={this.combobox}
        scaled={true}
        selectedOption={selectedOption}
        size="content"
      >
        <StyledIconButton sortDirection={!!sortDirection}>
          <IconButton
            clickColor={"#333"}
            color={"#A3A9AE"}
            hoverColor={"#333"}
            iconName={"ZASortingIcon"}
            isDisabled={isDisabled}
            isFill={true}
            onClick={this.onButtonClick}
            size={10}
          />
        </StyledIconButton>
      </ComboBox>
    );
  }
}

SortComboBox.propTypes = {
  directionAscLabel: PropTypes.string,
  directionDescLabel: PropTypes.string,
  isDisabled: PropTypes.bool,
  onButtonClick: PropTypes.func,
  onChangeSortDirection: PropTypes.func,
  onChangeSortId: PropTypes.func,
  onChangeView: PropTypes.func,
  getSortData: PropTypes.func,
  sortDirection: PropTypes.number,
  viewAs: PropTypes.bool, // TODO: include viewSelector after adding method getThumbnail - PropTypes.string
  sortId: PropTypes.string,
};

SortComboBox.defaultProps = {
  isDisabled: false,
  sortDirection: 0,
};

export default SortComboBox;
