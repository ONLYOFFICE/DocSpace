import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import GroupButton from "../group-button";
import DropDownItem from "../drop-down-item";
import throttle from "lodash/throttle";
import { isArrayEqual } from "../../utils/array";
import { tablet, desktop } from "../../utils/device";

const StyledGroupButtonsMenu = styled.div`
  box-sizing: border-box;
  position: sticky;
  top: 0;
  background: #ffffff;
  box-shadow: 0px 10px 18px -8px rgba(0, 0, 0, 0.100306);
  height: 56px;
  list-style: none;
  padding: 0 18px 19px 0;
  width: 100%;
  white-space: nowrap;

  display: ${(props) => (props.visible ? "block" : "none")};
  z-index: 189;

  @media ${desktop} {
    margin-top: 1px;
  }
`;

const CloseButton = styled.div`
  position: absolute;
  right: 11px;
  top: 10px;
  width: 20px;
  height: 20px;
  padding: 8px;

  @media ${tablet} {
    right: 3px;
  }

  &:hover {
    cursor: pointer;

    &:before,
    &:after {
      background-color: #555f65;
    }
  }

  &:before,
  &:after {
    position: absolute;
    left: 15px;
    content: " ";
    height: 20px;
    width: 1px;
    background-color: #d0d5da;
  }

  &:before {
    transform: rotate(45deg);
  }

  &:after {
    transform: rotate(-45deg);
  }
`;

const GroupMenuWrapper = styled.div`
  display: inline-block;
`;

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

  componentDidMount() {
    const groupMenuElement = document.getElementById("groupMenu");

    const groupMenuItems = groupMenuElement ? groupMenuElement.children : [0];
    const groupMenuItemsArray = [...groupMenuItems];

    this.widthsArray = groupMenuItemsArray.map((item) => item.offsetWidth);

    window.addEventListener("resize", this.throttledResize);
    window.addEventListener("orientationchange", this.throttledResize);

    this.updateMenu();
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ visible: this.props.visible });
    }

    if (!isArrayEqual(this.props.menuItems, prevProps.menuItems)) {
      this.setState({ priorityItems: this.props.menuItems });
    }

    if (
      this.props.sectionWidth !== prevProps.sectionWidth ||
      this.state.priorityItems.length !== prevState.priorityItems.length ||
      this.state.moreItems.length !== prevState.moreItems.length
    ) {
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
    } = this.props;
    const { priorityItems, moreItems, visible } = this.state;

    return (
      <StyledGroupButtonsMenu id="groupMenuOuter" visible={visible}>
        <GroupMenuWrapper id="groupMenu">
          {priorityItems.map((item, i) => (
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
            >
              {item.children}
            </GroupButton>
          ))}
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
        <CloseButton title={closeTitle} onClick={this.closeMenu} />
      </StyledGroupButtonsMenu>
    );
  }
}

GroupButtonsMenu.propTypes = {
  onClick: PropTypes.func,
  onClose: PropTypes.func,
  onChange: PropTypes.func,
  onSelect: PropTypes.func,
  menuItems: PropTypes.array,
  checked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  selected: PropTypes.string,
  visible: PropTypes.bool,
  moreLabel: PropTypes.string,
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
