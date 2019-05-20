import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Button } from 'asc-web-components';

const colStyle = { margin: '0 8px 0 8px' };

const rowStyle = { margin: '4px 0 4px 0' };

function onClick(e) {
  e = e || window.event;
    var target = e.target || e.srcElement,
        text = target.textContent || target.innerText;
  console.log("onClick", text);
}

storiesOf('Components|Button', module)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: 'responsive' }})
  .addDecorator(withReadme(Readme))
  .add('all', () => (
    <>
      <div>
        <Button style={colStyle} onClick={onClick}>Base button</Button>
        <Button size="middle" style={colStyle} onClick={onClick}>
          Middle button
        </Button>
        <Button size="big" style={colStyle} onClick={onClick}>
          Big button
        </Button>
        <Button size="huge" style={colStyle} onClick={onClick}>
          Huge button
        </Button>
      </div>
      <div style={rowStyle}>
        <Button disabled style={colStyle} onClick={onClick}>
          Base button
        </Button>
        <Button disabled size="middle" style={colStyle} onClick={onClick}>
          Middle button
        </Button>
        <Button disabled size="big" style={colStyle} onClick={onClick}>
          Big button
        </Button>
        <Button disabled size="huge" style={colStyle} onClick={onClick}>
          Huge button
        </Button>
      </div>
      <div style={rowStyle}>
        <Button primary style={colStyle} onClick={onClick}>
          Base button
        </Button>
        <Button size="middle" primary style={colStyle} onClick={onClick}>
          Middle button
        </Button>
        <Button size="big" primary style={colStyle} onClick={onClick}>
          Big button
        </Button>
        <Button size="huge" primary style={colStyle} onClick={onClick}>
          Huge button
        </Button>
      </div>
      <div style={rowStyle}>
        <Button disabled primary style={colStyle} onClick={onClick}>
          Base button
        </Button>
        <Button disabled size="middle" primary style={colStyle} onClick={onClick}>
          Middle button
        </Button>
        <Button disabled size="big" primary style={colStyle} onClick={onClick}>
          Big button
        </Button>
        <Button disabled size="huge" primary style={colStyle} onClick={onClick}>
          Huge button
        </Button>
      </div>
    </>
  ));
