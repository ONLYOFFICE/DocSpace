import React from 'react';
import { storiesOf } from '@storybook/react';
import { Link, DropDown } from 'asc-web-components';
import Readme from './README.md';
import {text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import { Container, Row, Col } from 'reactstrap';

const type = ['action', 'page'];
const colors = ['black', 'gray', 'blue', 'filter', 'profile'];
const target = ['_blank', '_self', '_parent', '_top'];
const dropdownType = ['filter', 'menu', 'none'];
const dropdownColor = ['filter', 'profile', 'sorting','number','email', 'group'];
const rowStyle = {
  marginTop: 8,
  paddingLeft: 20,
  fontSize: 12
};

storiesOf('Components|Link', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Link 
      type={select('type', type, 'page')}
      color={select('color', colors, 'black')}
      fontSize={text('fontSize', '12px')}
      href={text('href', 'https://github.com')}
      isBold={boolean('isBold', false)}
      title={text('title', '')}
      target={select('target', target, '_top')}
      rel={text('rel', '')}
      isTextOverflow={boolean('isTextOverflow', false)}
      isHovered={boolean('isHovered', false)}
      isDotted={boolean('isDotted', false)}
      isHoverDotted={boolean('isHoverDotted', false)}
      isDropdown={boolean('isDropdown', false)}
      dropdownType={select('dropdownType', dropdownType, 'none')}
      dropdownColor={select('dropdownColor', dropdownColor, 'filter')}
      dropdownRightIndent={text('dropdownRightIndent', '-10px')}
      displayDropdownAfterHover={boolean('displayDropdownAfterHover', false)}
      >Simple link</Link>
  ))
  .add('all', () => (
    <Container fluid>
      <Row style={rowStyle}>
        <Link type = "page" color = "black" href="https://github.com" >Page link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "page" color = "black" href="https://github.com" isHovered = {true}>Page link hovered</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "page" color = "black" href="https://github.com" isBold = {true}>Bold page link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "page" color = "gray">Page link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "page" color = "gray" href="https://github.com" isHovered = {true}>Page link hovered</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "page" color = "blue" href="https://github.com" >Page link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "page" color = "blue" href="https://github.com" isHovered = {true}>Page link hovered</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "black" isHoverDotted = {true}>Action link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "black" isHovered = {true} isHoverDotted = {true}>Action link hovered</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "black" isDotted = {true} isHoverDotted = {true}>Action link dotted</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "gray" isHoverDotted = {true}>Action link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "gray" isHovered = {true} isHoverDotted = {true}>Action link hovered</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "gray" isDotted = {true} isHoverDotted = {true}>Action link dotted</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "blue" isHoverDotted = {true}>Action link</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "blue" isHovered = {true} isHoverDotted = {true}>Action link hovered</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "blue" isDotted = {true} isHoverDotted = {true}>Action link dotted</Link>
      </Row>
      <Row style={rowStyle}>
        Sorting:&nbsp;<Link type = "action" color = "black" isDotted = {true} isDropdown = {true} dropdownColor = 'sorting' isHoverDotted = {true}>Last name</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "profile" isDropdown = {true} dropdownColor = 'profile' isHoverDotted = {true}>My profile</Link>
      </Row>
      <Row style={rowStyle}>
        Show on page:&nbsp;<Link type = "action" color = "black" isDotted = {true} isDropdown = {true} dropdownColor = 'number'>25</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "black" isDropdown = {true} dropdownColor = 'email' displayDropdownAfterHover = {true}>Dropdown appear after hover</Link>
      </Row>
      <Row style={rowStyle}>
        <Link type = "action" color = "filter" isDropdown = {true} dropdownColor = 'filter' isDropdown = {true} isDotted = {true}>Some group filter</Link>
      </Row>
    </Container>
  ))
    
  ;