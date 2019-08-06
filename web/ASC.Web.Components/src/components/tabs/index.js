import React, { Component } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

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
    background-color: #265A8F;
    cursor: default;
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
`;

class TabContainer extends Component {
  constructor(props) {
    super(props);
    this.state = {
      activeTab: 0
    };
  }

  titleClick = (index) => {
    if (this.state.activeTab !== index) {
      this.setState({ activeTab: index});
    }
    console.log(this.state.activeTab);
    
  };

  render() {    
   
    return (
      <TabsContainer>
        <NavItem>
          {this.props.children.map((item, index) =>
            <Label
              selected={this.state.activeTab === index}
              isDisabled={this.props.isDisabled}
              key={item.id}
              onClick={() => {
                this.titleClick(index);
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

export default TabContainer;

TabContainer.propTypes = {
  children: PropTypes.object,
  isDisabled: PropTypes.boolean
};

