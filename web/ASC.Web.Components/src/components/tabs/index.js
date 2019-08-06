import React, { Component } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

const TabsContainer = styled.div``;

const NavItem = styled.div`
    position: relative;
    white-space: nowrap;
`;

const Label = styled.label`
  margin:0;
  min-width: 80px;
  position: relative;
  background-repeat: no-repeat;
  p {text-align: center; margin-top: 6px;}
  margin-right: 5px;
  label {margin:0}

  ${props => props.selected ?
    `width: 80px;
    height: 32px;
    background-color: #265A8F;
    border-radius: 16px;
    cursor: default;
    p {color: #fff}` :
    `background-image: none;
    &:hover{
      cursor: pointer;
      width: 80px;
      height: 32px;
      background-color: #F8F9F9;
      border-radius: 16px;
    }`
  }
`;

const BodyContainer = styled.div`
  margin: 24px 16px 0px 16px;
`;

class Tabs extends Component {
  constructor(props) {
    super(props);
    this.state = {
      activeTab: '0'
    };
  }

  labelClick = (tab) => {
    if (this.state.activeTab !== tab.id) {
      this.setState({ activeTab: tab.id });
    }
  };

  render() {
    return (
      <TabsContainer>
        <NavItem>
          {this.props.children.map((item) =>
            <Label
              selected={item.id === this.state.activeTab}
              key={item.id}
              onClick={() => {
                this.labelClick(item);
              }}>
              {item.title}
            </Label>
          )}
        </NavItem>
        <BodyContainer> {this.props.children[this.state.activeTab].content} </BodyContainer>
      </TabsContainer>
    );
  }
}

export default Tabs;

Tabs.propTypes = {
  children: PropTypes.object
};

