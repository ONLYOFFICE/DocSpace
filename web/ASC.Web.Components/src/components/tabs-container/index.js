import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Text } from "../text";
import Scrollbar from "../scrollbar";

const TabsContainer = styled.div`
  .scrollbar {
    width: 100% !important;
    height: 50px !important;
  }
`;
const NavItem = styled.div`
  position: relative;
  white-space: nowrap;
  display: flex;
`;

const Label = styled.div`
  height: 32px;
  border-radius: 16px;
  min-width: fit-content;
  margin-right: 8px;
  width: fit-content;

  .title_style {
    text-align: center;
    margin: 7px 15px 7px 15px;
    overflow: hidden;

    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
  }

  ${props => (props.isDisabled ? `pointer-events: none;` : ``)}

  ${props =>
    props.selected
      ? `cursor: default
         background-color: #265A8F
         .title_style {
           color: #fff
          }`
      : `
    &:hover{
      cursor: pointer;
      background-color: #F8F9F9;
    }`}

  ${props =>
    props.isDisabled && props.selected
      ? `background-color: #A3A9AE
       .title_style {color: #fff}`
      : ``}
`;

const BodyContainer = styled.div`
  margin: 24px 16px 0px 16px;
`;

class TabContainer extends Component {
  constructor(props) {
    super(props);

    this.arrayRefs = [];
    const countElements = props.children.length;

    let item = countElements;
    while (item !== 0) {
      this.arrayRefs.push(React.createRef());
      item--;
    }

    this.state = {
      activeTab: this.props.selectedItem
    };

    this.scrollRef = React.createRef();
  }

  titleClick = (index, item, ref) => {
    if (this.state.activeTab !== index) {
      this.setState({ activeTab: index });
      let newItem = Object.assign({}, item);
      delete newItem.content;
      this.props.onSelect && this.props.onSelect(newItem);

      const position = ref.current.offsetLeft - 40;
      this.scrollRef.current.scrollLeft(position);
    }
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { activeTab } = this.state;
    const { isDisabled } = this.props;
    if (
      activeTab === nextState.activeTab &&
      isDisabled === nextProps.isDisabled
    ) {
      return false;
    }
    return true;
  }

  render() {
    //console.log("Tabs container render");

    const { isDisabled, children } = this.props;
    const { activeTab } = this.state;

    return (
      <TabsContainer>
        <Scrollbar
          values={this.onScrollFrame}
          autoHide
          autoHideTimeout={1000}
          className="scrollbar"
          ref={this.scrollRef}
        >
          <NavItem className="className_items">
            {children.map((item, index) => (
              <Label
                ref={this.arrayRefs[index]}
                onClick={this.titleClick.bind(
                  this,
                  index,
                  item,
                  this.arrayRefs[index]
                )}
                key={item.key}
                selected={activeTab === index}
                isDisabled={isDisabled}
              >
                <Text.Body className="title_style" fontSize={13}>
                  {item.title}
                </Text.Body>
              </Label>
            ))}
          </NavItem>
        </Scrollbar>
        <BodyContainer>{children[activeTab].content}</BodyContainer>
      </TabsContainer>
    );
  }
}

TabContainer.propTypes = {
  children: PropTypes.PropTypes.arrayOf(PropTypes.object.isRequired).isRequired,
  isDisabled: PropTypes.bool,
  onSelect: PropTypes.func,
  selectedItem: PropTypes.number
};

TabContainer.defaultProps = {
  selectedItem: 0
};

export default TabContainer;
