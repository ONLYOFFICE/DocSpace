import React, { Component } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Text } from "../text";
import Scrollbar from "../scrollbar";

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

    this.state = {
      activeTab: this.props.selectedItem
    };
  }

  titleClick = (index, item) => {
    if (this.state.activeTab !== index) {
      this.setState({ activeTab: index });
      let newItem = Object.assign({}, item);
      delete newItem.content;
      this.props.onSelect && this.props.onSelect(newItem);
    }
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { activeTab } = this.state;
    if (activeTab !== nextState.activeTab) {
      return true;
    } else return false;
  }

  render() {
    //console.log("Tabs container render");

    const { isDisabled, children } = this.props;
    return (
      <>
        <Scrollbar style={{ width: "100%", height: 50 }}>
          <NavItem className="className_items">
            {children.map((item, index) => (
              <Label
                onClick={this.titleClick.bind(this, index, item)}
                key={item.key}
                selected={this.state.activeTab === index}
                isDisabled={isDisabled}
              >
                <Text.Body className="title_style" fontSize={13}>
                  {item.title}
                </Text.Body>
              </Label>
            ))}
          </NavItem>
        </Scrollbar>
        <BodyContainer>{children[this.state.activeTab].content}</BodyContainer>
      </>
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
