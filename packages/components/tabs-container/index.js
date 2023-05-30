import React, { Component } from "react";
import PropTypes from "prop-types";

import Text from "../text";
import { NavItem, Label, StyledScrollbar } from "./styled-tabs-container";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

class TabContainer extends Component {
  constructor(props) {
    super(props);

    this.arrayRefs = [];
    const countElements = props.elements.length;

    let item = countElements;
    while (item !== 0) {
      this.arrayRefs.push(React.createRef());
      item--;
    }

    this.state = {
      activeTab: this.props.selectedItem,
      onScrollHide: true,
    };

    this.scrollRef = React.createRef();
  }

  titleClick = (index, item, ref) => {
    if (this.state.activeTab !== index) {
      this.setState({ activeTab: index });
      let newItem = Object.assign({}, item);
      delete newItem.content;
      this.props.onSelect && this.props.onSelect(newItem);

      this.setTabPosition(index, ref);
    }
  };

  getWidthElements = () => {
    const arrayWidths = [];
    const length = this.arrayRefs.length - 1;
    let widthItem = 0;
    while (length + 1 !== widthItem) {
      arrayWidths.push(this.arrayRefs[widthItem].current.offsetWidth);
      widthItem++;
    }

    return arrayWidths;
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { activeTab, onScrollHide } = this.state;
    const { isDisabled, elements } = this.props;
    if (
      activeTab === nextState.activeTab &&
      isDisabled === nextProps.isDisabled &&
      onScrollHide === nextState.onScrollHide &&
      elements === nextProps.elements
    ) {
      return false;
    }
    return true;
  }

  componentDidMount() {
    const { activeTab } = this.state;
    if (activeTab !== 0 && this.arrayRefs[activeTab].current !== null) {
      this.setPrimaryTabPosition(activeTab);
    }
  }

  setTabPosition = (index, currentRef) => {
    const arrayOfWidths = this.getWidthElements(); //get tabs widths
    const scrollLeft = this.scrollRef.current.getScrollLeft(); // get scroll position relative to left side
    const staticScroll = this.scrollRef.current.getScrollWidth(); //get static scroll width
    const containerWidth = this.scrollRef.current.getClientWidth(); //get main container width
    const currentTabWidth = currentRef.current.offsetWidth;
    const marginRight = 8;

    //get tabs of left side
    let leftTabs = 0;
    let leftFullWidth = 0;
    while (leftTabs !== index) {
      leftTabs++;
      leftFullWidth += arrayOfWidths[leftTabs] + marginRight;
    }
    leftFullWidth += arrayOfWidths[0] + marginRight;

    //get tabs of right side
    let rightTabs = this.arrayRefs.length - 1;
    let rightFullWidth = 0;
    while (rightTabs !== index - 1) {
      rightFullWidth += arrayOfWidths[rightTabs] + marginRight;
      rightTabs--;
    }

    //Out of range of left side
    if (leftFullWidth > containerWidth + scrollLeft) {
      let prevIndex = index - 1;
      let widthBlocksInContainer = 0;
      while (prevIndex !== -1) {
        widthBlocksInContainer += arrayOfWidths[prevIndex] + marginRight;
        prevIndex--;
      }

      const difference = containerWidth - widthBlocksInContainer;
      const currentContainerWidth = currentTabWidth;

      this.scrollRef.current.scrollLeft(
        difference * -1 + currentContainerWidth + marginRight
      );
    }
    //Out of range of left side
    else if (rightFullWidth > staticScroll - scrollLeft) {
      this.scrollRef.current.scrollLeft(staticScroll - rightFullWidth);
    }
  };

  setPrimaryTabPosition = (index) => {
    const arrayOfWidths = this.getWidthElements(); //get tabs widths
    const marginRight = 8;
    let rightTabs = this.arrayRefs.length - 1;
    let rightFullWidth = 0;
    while (rightTabs !== index - 1) {
      rightFullWidth += arrayOfWidths[rightTabs] + marginRight;
      rightTabs--;
    }
    rightFullWidth -= marginRight;
    const staticScroll = this.scrollRef.current.getScrollWidth(); //get static scroll width
    this.scrollRef.current.scrollLeft(staticScroll - rightFullWidth);
  };

  onMouseEnter = () => {
    this.setState({ onScrollHide: false });
  };

  onMouseLeave = () => {
    this.setState({ onScrollHide: true });
  };

  onClick = (index, item) => {
    this.titleClick(index, item, this.arrayRefs[index]);
  };

  render() {
    //console.log("Tabs container render");

    const { isDisabled, elements } = this.props;
    const { activeTab, onScrollHide } = this.state;

    return (
      <>
        <StyledScrollbar
          autoHide={onScrollHide}
          stype="preMediumBlack"
          className="scrollbar"
          ref={this.scrollRef}
        >
          <NavItem className="className_items">
            {elements.map((item, index) => (
              <ColorTheme
                {...this.props}
                themeId={ThemeType.TabsContainer}
                onMouseMove={this.onMouseEnter}
                onMouseLeave={this.onMouseLeave}
                ref={this.arrayRefs[index]}
                onClick={() => this.onClick(index, item)}
                key={item.key}
                selected={activeTab === index}
                isDisabled={isDisabled}
              >
                <Text fontWeight={600} className="title_style" fontSize="13px">
                  {item.title}
                </Text>
              </ColorTheme>
            ))}
          </NavItem>
        </StyledScrollbar>
        <div>{elements[activeTab].content}</div>
      </>
    );
  }
}

TabContainer.propTypes = {
  /** Child elements */
  elements: PropTypes.PropTypes.arrayOf(PropTypes.object.isRequired).isRequired,
  /** Disables the TabContainer  */
  isDisabled: PropTypes.bool,
  /** Sets a callback function that is triggered when the title is selected */
  onSelect: PropTypes.func,
  /** Selected title of tabs container */
  selectedItem: PropTypes.number,
};

TabContainer.defaultProps = {
  selectedItem: 0,
};

export default TabContainer;
