import PropTypes from "prop-types";
import React from "react";
import equal from "fast-deep-equal/react";

import ComboButton from "./sub-components/combo-button";

import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import StyledComboBox from "./styled-combobox";

class ComboBox extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
      selectedOption: props.selectedOption,
    };
  }

  shouldComponentUpdate(nextProps, nextState) {
    const needUpdate =
      !equal(this.props, nextProps) || !equal(this.state, nextState);

    return needUpdate;
  }

  stopAction = (e) => e.preventDefault();

  setIsOpen = (isOpen) => {
    const { setIsOpenItemAccess } = this.props;
    this.setState({ isOpen: isOpen });
    setIsOpenItemAccess && setIsOpenItemAccess(isOpen);
  };

  handleClickOutside = (e) => {
    const { setIsOpenItemAccess } = this.props;

    if (this.ref.current.contains(e.target)) return;

    this.setState({ isOpen: !this.state.isOpen }, () => {
      this.props.toggleAction && this.props.toggleAction(e, this.state.isOpen);
    });

    setIsOpenItemAccess && setIsOpenItemAccess(!this.state.isOpen);
  };

  comboBoxClick = (e) => {
    const {
      disableIconClick,
      disableItemClick,
      isDisabled,
      toggleAction,
      isLoading,
      setIsOpenItemAccess,
    } = this.props;

    if (
      isDisabled ||
      disableItemClick ||
      isLoading ||
      (disableIconClick && e && e.target.closest(".optionalBlock")) ||
      e?.target.classList.contains("nav-thumb-vertical")
    )
      return;

    this.setState({ isOpen: !this.state.isOpen }, () => {
      toggleAction && toggleAction(e, this.state.isOpen);
    });
    setIsOpenItemAccess && setIsOpenItemAccess(!this.state.isOpen);
  };

  optionClick = (option) => {
    const { setIsOpenItemAccess } = this.props;
    this.setState({
      isOpen: !this.state.isOpen,
      selectedOption: option,
    });
    setIsOpenItemAccess && setIsOpenItemAccess(!this.state.isOpen);
    this.props.onSelect && this.props.onSelect(option);
  };

  componentDidUpdate(prevProps) {
    const { setIsOpenItemAccess } = this.props;
    if (this.props.opened !== prevProps.opened) {
      this.setIsOpen(this.props.opened);
      setIsOpenItemAccess && setIsOpenItemAccess(this.props.opened);
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
      type,
      options,
      advancedOptions,
      isDisabled,
      children,
      noBorder,
      scaledOptions,
      displayType,
      toggleAction,
      textOverflow,
      showDisabledItems,
      comboIcon,
      manualY,
      manualX,
      isDefaultMode,
      manualWidth,
      displaySelectedOption,
      fixedDirection,
      withBlur,
      fillIcon,
      offsetLeft,
      modernView,
      withBackdrop,
      isAside,
      withBackground,
      advancedOptionsCount,
      isMobileView,
      withoutPadding,
      isLoading,
      isNoFixedHeightOptions,
      hideMobileView,
    } = this.props;
    const { tabIndex, ...props } = this.props;
    const { isOpen, selectedOption } = this.state;

    const dropDownMaxHeightProp = dropDownMaxHeight
      ? { maxHeight: dropDownMaxHeight }
      : {};

    const dropDownManualWidthProp =
      scaledOptions && !isDefaultMode
        ? { manualWidth: "100%" }
        : scaledOptions && this.ref.current
        ? { manualWidth: this.ref.current.clientWidth + "px" }
        : { manualWidth: manualWidth };

    const optionsLength = options.length
      ? options.length
      : displayType !== "toggle"
      ? 0
      : 1;

    const withAdvancedOptions = !!advancedOptions?.props.children;

    let optionsCount = optionsLength;

    if (withAdvancedOptions) {
      const advancedOptionsWithoutSeparator =
        advancedOptions.props.children.filter((option) => option.key !== "s1");

      const advancedOptionsWithoutSeparatorLength =
        advancedOptionsWithoutSeparator.length;

      optionsCount = advancedOptionsCount
        ? advancedOptionsCount
        : advancedOptionsWithoutSeparatorLength
        ? advancedOptionsWithoutSeparatorLength
        : 6;
    }

    const disableMobileView = optionsCount < 4 || hideMobileView;

    return (
      <StyledComboBox
        ref={this.ref}
        isDisabled={isDisabled}
        scaled={scaled}
        size={size}
        data={selectedOption}
        onClick={this.comboBoxClick}
        toggleAction={toggleAction}
        isOpen={isOpen}
        disableMobileView={disableMobileView}
        withoutPadding={withoutPadding}
        {...props}
      >
        <ComboButton
          noBorder={noBorder}
          isDisabled={isDisabled}
          selectedOption={selectedOption}
          withOptions={optionsLength > 0}
          optionsLength={optionsLength}
          withAdvancedOptions={withAdvancedOptions}
          innerContainer={children}
          innerContainerClassName="optionalBlock"
          isOpen={isOpen}
          size={size}
          scaled={scaled}
          comboIcon={comboIcon}
          modernView={modernView}
          fillIcon={fillIcon}
          tabIndex={tabIndex}
          isLoading={isLoading}
          type={type}
        />

        {displayType !== "toggle" && (
          <DropDown
            id={this.props.dropDownId}
            className="dropdown-container not-selectable"
            directionX={directionX}
            directionY={directionY}
            manualY={manualY}
            manualX={manualX}
            open={isOpen}
            forwardedRef={this.ref}
            clickOutsideAction={this.handleClickOutside}
            style={advancedOptions && { padding: "6px 0px" }}
            {...dropDownMaxHeightProp}
            {...dropDownManualWidthProp}
            showDisabledItems={showDisabledItems}
            isDefaultMode={isDefaultMode}
            fixedDirection={fixedDirection}
            withBlur={withBlur}
            offsetLeft={offsetLeft}
            withBackdrop={withBackdrop}
            isAside={isAside}
            withBackground={withBackground}
            isMobileView={isMobileView && !disableMobileView}
            isNoFixedHeightOptions={isNoFixedHeightOptions}
          >
            {advancedOptions
              ? advancedOptions
              : options.map((option) => {
                  const disabled =
                    option.disabled ||
                    (!displaySelectedOption &&
                      option.label === selectedOption.label);

                  const isActive =
                    displaySelectedOption &&
                    option.label === selectedOption.label;

                  const isSelected = option.label === selectedOption.label;
                  return (
                    <DropDownItem
                      {...option}
                      textOverflow={textOverflow}
                      key={option.key}
                      disabled={disabled}
                      backgroundColor={option.backgroundColor}
                      onClick={this.optionClick.bind(this, option)}
                      fillIcon={fillIcon}
                      isModern={noBorder}
                      isActive={isActive}
                      isSelected={isSelected}
                    />
                  );
                })}
          </DropDown>
        )}
      </StyledComboBox>
    );
  }
}

ComboBox.propTypes = {
  /** Displays advanced options */
  advancedOptions: PropTypes.element,
  /** Children elements */
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** X direction position */
  directionX: PropTypes.oneOf(["left", "right"]),
  /** Y direction position */
  directionY: PropTypes.oneOf(["bottom", "top", "both"]),
  /** Component Display Type */
  displayType: PropTypes.oneOf(["default", "toggle"]),
  /** Height of Dropdown */
  dropDownMaxHeight: PropTypes.number,
  /** Displays disabled items when displayType !== toggle */
  showDisabledItems: PropTypes.bool,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts id for dropdown container */
  dropDownId: PropTypes.string,
  /** Indicates that component contains a backdrop */
  withBackdrop: PropTypes.bool,
  /** Indicates that component is disabled */
  isDisabled: PropTypes.bool,
  /** Indicates that component is displayed without borders */
  noBorder: PropTypes.bool,
  /** Is triggered whenever ComboBox is a selected option */
  onSelect: PropTypes.func,
  /** Sets the component open */
  opened: PropTypes.bool,
  /** Combo box options */
  options: PropTypes.array.isRequired,
  /** Indicates that component is scaled by parent */
  scaled: PropTypes.bool,
  /** Indicates that component`s options are scaled by ComboButton */
  scaledOptions: PropTypes.bool,
  /** Selected option */
  selectedOption: PropTypes.object.isRequired,
  /** Sets the component's width from the default settings */
  size: PropTypes.oneOf(["base", "middle", "big", "huge", "content"]),
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** The event is triggered by clicking on a component when `displayType: toggle` */
  toggleAction: PropTypes.func,
  /** Accepts css text-overflow */
  textOverflow: PropTypes.bool,
  /** Disables clicking on the icon */
  disableIconClick: PropTypes.bool,
  /** Sets the operation mode of the component. The default option is set to portal mode */
  isDefaultMode: PropTypes.bool,
  /** Y offset */
  offsetDropDownY: PropTypes.string,
  /** Sets an icon that is displayed in the combo button */
  comboIcon: PropTypes.string,
  /** Sets the precise distance from the parent */
  manualY: PropTypes.string,
  /** Sets the precise distance from the parent */
  manualX: PropTypes.string,
  /** Dropdown manual width */
  manualWidth: PropTypes.string,
  /** Displays the selected option */
  displaySelectedOption: PropTypes.bool,
  /** Disables position checking. Used for explicit direction setting */
  fixedDirection: PropTypes.bool,
  /** Disables clicking on the item */
  disableItemClick: PropTypes.bool,
  /** Indicates that component will fill selected item icon */
  fillIcon: PropTypes.bool,
  /** Sets the left offset for the dropdown */
  offsetLeft: PropTypes.number,
  /** Sets the combo-box to be displayed in modern view */
  modernView: PropTypes.bool,
  /** Count of advanced options  */
  advancedOptionsCount: PropTypes.number,
  /** Accepts css tab-index style */
  tabIndex: PropTypes.number,
  /** Disables the combo box padding */
  withoutPadding: PropTypes.bool,
  /** Indicates when the component is loading */
  isLoading: PropTypes.bool,
  /**Type ComboBox */
  type: PropTypes.oneOf(["badge", null]),
};

ComboBox.defaultProps = {
  displayType: "default",
  isDisabled: false,
  noBorder: false,
  scaled: true,
  scaledOptions: false,
  size: "base",
  disableIconClick: true,
  showDisabledItems: false,
  manualY: "102%",
  isDefaultMode: true,
  manualWidth: "200px",
  displaySelectedOption: false,
  fixedDirection: false,
  disableItemClick: false,
  modernView: false,
  tabIndex: -1,
  withoutPadding: false,
  isLoading: false,
};

export default ComboBox;
