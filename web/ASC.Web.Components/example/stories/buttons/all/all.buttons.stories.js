import React from 'react';
import { storiesOf } from '@storybook/react';
import { Container, Row, Col } from 'reactstrap';
import { Button } from 'asc-web-components';

function onClick(e) {
  e = e || window.event;
  var target = e.target || e.srcElement,
    text = target.textContent || target.innerText;
  console.log("onClick", text);
}

const rowStyle = {
  marginTop: 8
};

const getButtons = () => {
  const primary = [true, false];
  const sizes = ['huge', 'big', 'middle', 'base'];
  const states = ['isActivated', 'isHovered', 'isPressed', 'isDisabled', 'isLoading'];

  const baseButton = {
    size: 'base',
    primary: true,
    isActivated: false,
    isHovered: false,
    isPressed: false,
    isDisabled: false,
    isLoading: false,
    onClick: onClick,
    label: "base button"
  };

  let buttons = [];
  primary.forEach(type => {
    baseButton.primary = type;
    sizes.forEach(size => {
      let sizeButtons = [];
      states.forEach(state => {
        let btn = { ...baseButton, size: size, label: `${size} button` }
        btn[state] = true;
        sizeButtons.push(btn);
      })
      buttons.push({
        size: size,
        buttons: sizeButtons
      });
    });
  });

  return buttons;
};

storiesOf('Components|Buttons', module)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .addParameters({ options: { showAddonPanel: false } })
  .add('all', () => (
    <>
      <Container fluid>
        <Row style={rowStyle}>
          <Col>Active</Col>
          <Col>Hover</Col>
          <Col>Click*(otional)</Col>
          <Col>Disable</Col>
          <Col>Loading</Col>
        </Row>

        {Object.values(getButtons()).map((btnSize, i) => {
          console.log(btnSize);
          return (
            <Row style={rowStyle}>
              {Object.values(btnSize.buttons).map((btn, j) => (
                <Col key={i}>
                  <Button key={j} {...btn} />
                </Col>))}
            </Row>
          )
        })}

      </Container>
    </>
  ));
