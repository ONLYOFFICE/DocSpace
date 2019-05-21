import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Container, Row, Col } from 'reactstrap';
import { Button } from 'asc-web-components';

function onClick(e) {
  e = e || window.event;
  var target = e.target || e.srcElement,
    text = target.textContent || target.innerText;
  console.log("onClick", text);
}

const rowStyle = { marginTop: 8 };

storiesOf('Components|Button', module)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .addDecorator(withReadme(Readme))
  .add('all', () => (
    <>
      <Container>
        <Row style={rowStyle}>
          <Col>Active</Col>
          <Col>Hover</Col>
          <Col>Click*(otional)</Col>
          <Col>Disable</Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button size="huge" primary isActivated onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="huge" primary isHovered onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="huge" primary isClicked onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="huge" primary isDisabled>Save it button</Button></Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button size="big" primary isActivated onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="big" primary isHovered onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="big" primary isClicked onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="big" primary isDisabled>Save it button</Button></Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button size="middle" primary isActivated onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="middle" primary isHovered onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="middle" primary isClicked onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="middle" primary isDisabled>Save it button</Button></Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button primary isActivated onClick={onClick}>Ok</Button></Col>
          <Col><Button primary isHovered onClick={onClick}>Ok</Button></Col>
          <Col><Button primary isClicked onClick={onClick}>Ok</Button></Col>
          <Col><Button primary isDisabled>Ok</Button></Col>
        </Row>
      </Container>
      <Container>
        <Row style={rowStyle}>
          <Col><Button size="huge" isActivated onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="huge" isHovered onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="huge" isClicked onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="huge" isDisabled>Save it button</Button></Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button size="big" isActivated onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="big" isHovered onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="big" isClicked onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="big" isDisabled>Save it button</Button></Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button size="middle" isActivated onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="middle" isHovered onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="middle" isClicked onClick={onClick}>Save it button</Button></Col>
          <Col><Button size="middle" isDisabled>Save it button</Button></Col>
        </Row>
        <Row style={rowStyle}>
          <Col><Button isActivated onClick={onClick}>Ok</Button></Col>
          <Col><Button isHovered onClick={onClick}>Ok</Button></Col>
          <Col><Button isClicked onClick={onClick}>Ok</Button></Col>
          <Col><Button isDisabled>Ok</Button></Col>
        </Row>
      </Container>
    </>
  ));
