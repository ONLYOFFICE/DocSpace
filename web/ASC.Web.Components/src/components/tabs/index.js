import React, { Component } from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import { Icons } from '../icons';
import { getCssFromSvg } from '../icons/get-css-from-svg';
import ReactDOMServer from 'react-dom/server';

var tabsIcon/*, tabsIcon2*/;
(function () { tabsIcon = getCssFromSvg(ReactDOMServer.renderToString(<Icons.TabsIcon />)); }());
//tabsIcon2 = getCssFromSvg(ReactDOMServer.renderToString(<Icons.TabsIcon />)); 

// Основной контейнер
const TabsContainer = styled.div``;

// Шапка
const NavItem = styled.div`
    position: relative;
    box-shadow: 0px 5px 20px rgba(0,0,0,0.13);
`;

//Исправить!!!
const hoverCss = css`
  width: 80px;
  height: 32px;
  background-color: #F8F9F9;
  border-radius: 16px;
`;

// Заголовки шапки
const Label = styled.label`
  margin:0;
  min-width: 80px;
  position: relative;
  background-repeat: no-repeat;
  p {text-align: center; margin-top: 6px;}
  margin-right: 5px;
  label {margin:0}

  ${props => props.selected ?
    `background-image: url("data:image/svg+xml,${tabsIcon}"); cursor: default; p {color: #fff}` :
    `background-image: none;
    &:hover{
      cursor: pointer;
      ${hoverCss}
    }`
  }
`;

// Контенn в зависимости от шапки
const BodyContainer = styled.div``;


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
      console.log('My update setState()');
    }
  };

  render() {
    //console.log(this.props.selected);
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
        <BodyContainer> {this.props.children[this.state.activeTab].body} </BodyContainer>
      </TabsContainer>
    );
  }
}

export default Tabs;

Tabs.propTypes = {
  children: PropTypes.object
};
Tabs.defaultProps = {/*isChecked: false*/ };