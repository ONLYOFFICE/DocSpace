import React from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';
import GroupButton from '../group-button';
import DropDownItem from '../drop-down-item';
import throttle from 'lodash/throttle';
import { isArrayEqual } from '../../utils/array';

const StyledGroupButtonsMenu = styled.div`
    position: sticky;
    top: 0;
    background: #FFFFFF;
    box-shadow: 0px 2px 18px rgba(0, 0, 0, 0.100306);
    height: 56px;
    list-style: none;
    padding: 0 18px 19px 0;
    width: 100%;
    white-space: nowrap;
    display: ${props => props.visible ? 'block' : 'none'};
    z-index: 350;
`;

const CloseButton = styled.div`
    position: absolute;
    right: 20px;
    top: 20px;
    width: 20px;
    height: 20px;

    &:hover{
        cursor: pointer;
    }

    &:before, &:after {
        position: absolute;
        left: 15px;
        content: ' ';
        height: 20px;
        width: 1px;
        background-color: #D8D8D8;
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

class GroupButtonsMenu extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      priorityItems: props.menuItems,
      moreItems: [],
      visible: true
    }
  
    this.throttledResize = throttle(this.updateMenu, 300);
  }

  closeMenu = (e) => {
    this.setState({ visible: false });
    this.props.onClose && this.props.onClose(e);
  };

  groupButtonClick = (item) => {
    if (item.disabled) return;
    item.onClick();
    this.closeMenu();
  };

  componentDidMount() {
    const groupMenuItems = document.getElementById("groupMenu") 
      ? document.getElementById("groupMenu").children 
      : [];
    const groupMenuItemsArray = [...groupMenuItems];

    this.widthsArray = groupMenuItemsArray.map(item => item.getBoundingClientRect().width);
    window.addEventListener('resize', this.throttledResize);
    this.updateMenu();
  }

  componentDidUpdate(prevProps) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ visible: this.props.visible });
    }

    if(!isArrayEqual(this.props.menuItems, prevProps.menuItems)){
      this.setState({ priorityItems: this.props.menuItems, });
      this.updateMenu();
    }
  }

  countMenuItems = (array, outerWidth, initialWidth) => {
    let total = (initialWidth + 80);
    for (let i = 0, len = array.length; i < len; i++) {
      if (total + array[i] > outerWidth) {
        return i < 1 ? 1 : i;
      } else {
        total += array[i];
      }
    }
  };

  updateMenu = () => {
    const moreMenuElement = document.getElementById("moreMenu");
    const groupMenuOuterElement = document.getElementById("groupMenuOuter");

    const moreMenuWidth = moreMenuElement ? moreMenuElement.getBoundingClientRect().width : 0;
    const groupMenuOuterWidth = groupMenuOuterElement ? groupMenuOuterElement.getBoundingClientRect().width : 0;

    const visibleItemsCount = this.countMenuItems(this.widthsArray, groupMenuOuterWidth, moreMenuWidth);
    const navItemsCopy = this.props.menuItems;
    
    const priorityItems = navItemsCopy.slice(0, visibleItemsCount);
    const moreItems = priorityItems.length !== navItemsCopy.length ? navItemsCopy.slice(visibleItemsCount, navItemsCopy.length) : [];

    this.setState({
      priorityItems: priorityItems,
      moreItems: moreItems
    });
  };

  componentWillUnmount() {
    window.removeEventListener('resize', this.throttledResize);
  }

  render() {
    //console.log("GroupButtonsMenu render");
    const { selected, moreLabel, closeTitle } = this.props;
    const { priorityItems, moreItems, visible } = this.state;
    
    return (
      <StyledGroupButtonsMenu id="groupMenuOuter" visible={visible} >
        <GroupMenuWrapper id="groupMenu">
          {priorityItems.map((item, i) =>
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
              onClick={this.groupButtonClick.bind(this, item)}
              {...this.props}
            >
              {item.children}
            </GroupButton>
          )}
        </GroupMenuWrapper>
        {moreItems.length > 0 &&
          <GroupButton 
            id="moreMenu" 
            isDropdown={true}
            label={moreLabel}
          >
            {moreItems.map((item, i) =>
              <DropDownItem
                key={`moreNavItem-${i}`}
                label={item.label}
                disabled={item.disabled}
                onClick={this.groupButtonClick.bind(this, item)}
              />
            )}
          </GroupButton>
        }
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
  selected: PropTypes.string,
  visible: PropTypes.bool,
  moreLabel: PropTypes.string,
  closeTitle: PropTypes.string
}

GroupButtonsMenu.defaultProps = {
  checked: false,
  selected: 'Select',
  visible: PropTypes.bool,
  moreLabel: 'More',
  closeTitle: 'Close'
}

export default GroupButtonsMenu;