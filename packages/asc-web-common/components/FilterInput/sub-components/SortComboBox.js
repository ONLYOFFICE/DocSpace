import React from "react";
import equal from "fast-deep-equal/react";
import PropTypes from "prop-types";
import { isMobileOnly } from "react-device-detect";

import ComboBox from "@appserver/components/combobox";
import IconButton from "@appserver/components/icon-button";
import DropDownItem from "@appserver/components/drop-down-item";
import RadioButtonGroup from "@appserver/components/radio-button-group";

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
  shouldComponentUpdate(nextProps, nextState) {
    //TODO
    /*const comboboxText = this.combobox.current.ref.current.children[0].children[1];
    if(comboboxText.scrollWidth > Math.round(comboboxText.getBoundingClientRect().width)){
        comboboxText.style.opacity = "0";
    }else{
        comboboxText.style.opacity = "1";
    }*/
    const { sortDirection } = nextProps;
    if (this.props.sortDirection !== sortDirection) {
      this.setState({
        sortDirection,
      });
      return true;
    }
    return !equal(this.props, nextProps) || !equal(this.state, nextState);
  }

  onToggleAction = () => {
    this.setState({
      opened: !this.state.opened,
    });
  };

  render() {
    const {
      options,
      directionAscLabel,
      directionDescLabel,
      isDisabled,
      selectedOption,
      viewAs,
      viewSettings,
    } = this.props;
    const { sortDirection, opened } = this.state;

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
        {viewSettings && viewSettings.length && viewAs && isMobileOnly && (
          <>
            <DropDownItem isSeparator />
            <DropDownItem noHover>
              <RadioButtonGroup
                fontWeight={600}
                isDisabled={isDisabled}
                name={"view"}
                onClick={this.onChangeView}
                options={viewSettings}
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
            color={"#A3A9AE"}
            iconName="/static/images/z-a.sorting.react.svg"
            isDisabled={isDisabled}
            isFill={true}
            onClick={this.onToggleAction}
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
  sortDirection: PropTypes.number,
  viewAs: PropTypes.string,
};

SortComboBox.defaultProps = {
  isDisabled: false,
  sortDirection: 0,
};

export default SortComboBox;
