import React, { Component } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import { Text } from "../text";

const TabsContainer = styled.div``;

const NavItem = styled.div`
  position: relative;
  white-space: nowrap;
`;

const TitleStyle = css`
  width: 80px;
  height: 32px;
  border-radius: 16px;
`;

const Label = styled.label`
  margin:0;
  min-width: 80px;
  position: relative;
  background-repeat: no-repeat;
  p {text-align: center; margin-top: 6px;}
  margin-right: 5px;
  label {margin:0}

  ${props => props.isDisabled ? `pointer-events: none;` : ``}

  ${props => props.selected ?
    `${TitleStyle}
    cursor: default;
    background-color: #265A8F;
    p {color: #fff}` :
    `
    &:hover{
      ${TitleStyle}
      cursor: pointer;
      background-color: #F8F9F9;
    }`
  }

  ${props => props.isDisabled && props.selected ?
    `${TitleStyle} background-color: #A3A9AE;
    p {color: #fff} ` : ``
  }
`;

const BodyContainer = styled.div`
  margin: 24px 16px 0px 16px;

  ${props => props.isDisabled ? `pointer-events: none;` : ``}
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

  render() {
    //console.log('Tab container render');
    return (
      <TabsContainer>
        <NavItem>
          {this.props.children.map((item, index) =>
            <Label
              onClick={this.titleClick.bind(this, index, item)}
              key={item.key}
              selected={this.state.activeTab === index}
              isDisabled={this.props.isDisabled}
            >
              <Text.Body> {item.title} </Text.Body>
            </Label>
          )}
        </NavItem>
        <BodyContainer> {this.props.children[this.state.activeTab].content} </BodyContainer>
      </TabsContainer>
    );
  }
}

TabContainer.propTypes = {
  children: PropTypes.PropTypes.arrayOf(
    PropTypes.object.isRequired
  ).isRequired,
  isDisabled: PropTypes.bool,
  onSelect: PropTypes.func,
  selectedItem: PropTypes.number
};

TabContainer.defaultProps = {
  selectedItem: 0
}

export default TabContainer;