import React from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';
import GroupButton from '../group-button';
import DropDownItem from '../drop-down-item';

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
    display: ${state => state.visible ? 'block' : 'none'};
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

    this.fullMenuArray = this.props.menuItems;
    this.checkBox = this.props.checkBox;

    this.updateMenu = this.updateMenu.bind(this);
    this.howManyItemsInMenuArray = this.howManyItemsInMenuArray.bind(this);
    this.closeMenu = this.closeMenu.bind(this);
    this.groupButtonClick = this.groupButtonClick.bind(this);
  }

  closeMenu = (e) => {
    this.setState({ visible: false });
    this.props.onClose && this.props.onClose(e);
  };

  groupButtonClick = (item) => {
    item.onClick();
    this.closeMenu();
  };

  componentDidMount() {
    this.widthsArray = Array.from(document.getElementById("groupMenu").children).map(item => item.getBoundingClientRect().width);
    window.addEventListener('resize', _.throttle(this.updateMenu), 300);
    this.updateMenu();
  }

  componentDidUpdate(prevProps) {
    if (this.props.visible !== prevProps.visible) {
      this.setState({ visible: this.props.visible });
    }
  };

  howManyItemsInMenuArray = (array, outerWidth, initialWidth) => {
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
    const outerWidth = groupMenuOuterElement ? groupMenuOuterElement.getBoundingClientRect().width : 0;
    const moreMenuWidth = moreMenuElement ? moreMenuElement.getBoundingClientRect().width : 0;
    const arrayAmount = this.howManyItemsInMenuArray(this.widthsArray, outerWidth, moreMenuWidth);
    const navItemsCopy = this.fullMenuArray;
    const priorityItems = navItemsCopy.slice(0, arrayAmount);

    this.setState({
      priorityItems: priorityItems,
      moreItems: priorityItems.length !== navItemsCopy.length ? navItemsCopy.slice(arrayAmount, navItemsCopy.length) : []
    });
  };

  componentWillUnmount() {
    window.removeEventListener('resize', this.updateMenu());
  }

  render() {
    //console.log("GroupButtonsMenu render");
    return (
      <StyledGroupButtonsMenu id="groupMenuOuter" visible={this.state.visible} {...this.state}>
        <GroupMenuWrapper id="groupMenu">
          {this.state.priorityItems.map((item, i) =>
            <GroupButton key={`navItem-${i}`}
              label={item.label}
              isDropdown={item.isDropdown}
              isSeparator={item.isSeparator}
              isSelect={item.isSelect}
              onSelect={item.onSelect}
              fontWeight={item.fontWeight}
              onClick={this.groupButtonClick.bind(this, item)}
              {...this.props}
            >
              {item.children}
            </GroupButton>
          )}
        </GroupMenuWrapper>
        {this.state.moreItems.length > 0 &&
          <GroupButton id="moreMenu" isDropdown label={this.props.moreLabel}>
            {this.state.moreItems.map((item, i) =>
              <DropDownItem
                key={`moreNavItem-${i}`}
                label={item.label}
                onClick={this.groupButtonClick.bind(this, item)}
              />
            )}
          </GroupButton>
        }
        <CloseButton title={this.props.closeTitle} onClick={this.closeMenu} />
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

export default GroupButtonsMenu;