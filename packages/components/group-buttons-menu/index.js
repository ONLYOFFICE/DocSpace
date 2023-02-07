import React from "react";

import PropTypes from "prop-types";
import GroupButton from "../group-button";
import DropDownItem from "../drop-down-item";
import throttle from "lodash/throttle";
import {
  StyledGroupButtonsMenu,
  CloseButton,
  GroupMenuWrapper,
} from "./styled-group-buttons-menu";

class GroupButtonsMenu extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      priorityItems: props.menuItems,
      moreItems: [],
      visible: props.visible,
    };

    this.throttledResize = throttle(this.updateMenu, 200);
  }

  closeMenu = (e) => {
    this.setState({ visible: false });
    this.props.onClose && this.props.onClose(e);
  };

  groupButtonClick = (e) => {
    const { priorityItems } = this.state;
    const index = e.currentTarget.dataset.index;
    const item = priorityItems[index];

    if (item.disabled) return;

    item.onClick && item.onClick(e);
  };

  groupMoreMenuButtonClick = (e) => {
    const { moreItems } = this.state;
    const index = e.currentTarget.dataset.index;
    const item = moreItems[index];

    if (item.disabled) return;

    item.onClick && item.onClick(e);
  };

  updateItemsWidth = () => {
    const groupMenuElement = document.getElementById("groupMenu");

    const groupMenuItems = groupMenuElement ? groupMenuElement.children : [0];
    const groupMenuItemsArray = [...groupMenuItems];

    this.widthsArray = groupMenuItemsArray.map((item) => item.offsetWidth);
  };

  componentDidMount() {
    this.updateItemsWidth();

    window.addEventListener("resize", this.throttledResize);
    window.addEventListener("orientationchange", this.throttledResize);

    this.updateMenu();
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ visible: this.props.visible });
    }

    if (
      this.props.sectionWidth !== prevProps.sectionWidth ||
      this.state.priorityItems.length !== prevState.priorityItems.length ||
      this.state.moreItems.length !== prevState.moreItems.length ||
      this.props.menuItems !== prevProps.menuItems
    ) {
      if (
        this.state.priorityItems.length !== prevState.priorityItems.length &&
        this.props.sectionWidth === prevProps.sectionWidth &&
        this.state.moreItems.length === prevState.moreItems.length
      ) {
        this.updateItemsWidth();
      }

      this.updateMenu();
    }
  }

  countMenuItems = (array, outerWidth, moreWidth) => {
    const itemsArray = array || [];
    let total = (moreWidth || 0) + 10;

    for (let i = 0, len = itemsArray.length; i < len; i++) {
      if (total + itemsArray[i] > outerWidth) {
        return i < 1 ? 1 : i;
      } else {
        total += itemsArray[i];
      }
    }
  };

  updateMenu = () => {
    const { sectionWidth } = this.props;
    let groupMenuOuterWidth = sectionWidth;

    if (!sectionWidth) {
      const groupMenuOuterElement = document.getElementById("groupMenuOuter");
      const groupMenuOuterValues =
        groupMenuOuterElement && groupMenuOuterElement.getBoundingClientRect();
      const screenWidth = window.innerWidth;
      const xWidth = groupMenuOuterValues && groupMenuOuterValues.x;
      groupMenuOuterWidth = screenWidth - xWidth;
    }
    const moreMenuElement = document.getElementById("moreMenu");
    const moreMenuWidth =
      moreMenuElement && moreMenuElement.getBoundingClientRect().width;
    const visibleItemsCount = this.countMenuItems(
      this.widthsArray,
      groupMenuOuterWidth,
      moreMenuWidth
    );
    const navItemsCopy = this.props.menuItems;

    const priorityItems = navItemsCopy.slice(0, visibleItemsCount);
    const moreItems =
      priorityItems.length !== navItemsCopy.length
        ? navItemsCopy.slice(visibleItemsCount, navItemsCopy.length)
        : [];

    this.setState({
      priorityItems: priorityItems,
      moreItems: moreItems,
    });
  };

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
    window.removeEventListener("orientationchange", this.throttledResize);
  }

  render() {
    //console.log("GroupButtonsMenu render");
    const {
      selected,
      moreLabel,
      closeTitle,
      checked,
      isIndeterminate,
      onChange,
      className,
    } = this.props;
    const { priorityItems, moreItems, visible } = this.state;

    return (
      <StyledGroupButtonsMenu
        id="groupMenuOuter"
        className={`${className} not-selectable`}
        visible={visible}
      >
        <GroupMenuWrapper id="groupMenu">
          {priorityItems.length &&
            priorityItems.map((item, i) => {
              //if (item.disabled) return;
              return (
                <GroupButton
                  key={`navItem-${i}`}
                  label={item.label}
                  isDropdown={item.isDropdown}
                  isSeparator={item.isSeparator}
                  isSelect={item.isSelect}
                  onSelect={item.onSelect}
                  selected={selected}
                  fontWeight={item.fontWeight}
                  disabled={item.disabled}
                  onClick={this.groupButtonClick}
                  data-index={i}
                  activated={item.activated}
                  checked={checked}
                  dropDownMaxHeight={item.dropDownMaxHeight}
                  hovered={item.hovered}
                  isIndeterminate={isIndeterminate}
                  onChange={onChange}
                  opened={item.opened}
                  alt={item.alt}
                >
                  {item.children}
                </GroupButton>
              );
            })}
        </GroupMenuWrapper>
        {moreItems.length > 0 && (
          <GroupButton id="moreMenu" isDropdown={true} label={moreLabel}>
            {moreItems.map((item, i) => (
              <DropDownItem
                key={`moreNavItem-${i}`}
                label={item.label}
                disabled={item.disabled}
                onClick={this.groupMoreMenuButtonClick}
                data-index={i}
              />
            ))}
          </GroupButton>
        )}
        <CloseButton
          className="not-selectable"
          title={closeTitle}
          onClick={this.closeMenu}
        />
      </StyledGroupButtonsMenu>
    );
  }
}

GroupButtonsMenu.propTypes = {
  /** onClick action on GroupButton's */
  onClick: PropTypes.func,
  /** onClose action if menu closing */
  onClose: PropTypes.func,
  /** onChange action on use selecting */
  onChange: PropTypes.func,
  onSelect: PropTypes.func,
  /** Button collection */
  menuItems: PropTypes.array,
  /** Sets initial value of checkbox */
  checked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  /** Selected header value */
  selected: PropTypes.string,
  /** Sets menu visibility */
  visible: PropTypes.bool,
  /** Label for more button */
  moreLabel: PropTypes.string,
  /** Title for close menu button */
  closeTitle: PropTypes.string,
  sectionWidth: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
};

GroupButtonsMenu.defaultProps = {
  checked: false,
  selected: "Select",
  visible: true,
  moreLabel: "More",
  closeTitle: "Close",
};

export default GroupButtonsMenu;
