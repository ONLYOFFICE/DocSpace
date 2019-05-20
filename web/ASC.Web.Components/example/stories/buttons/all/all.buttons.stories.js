import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Button } from 'asc-web-components';

const colStyle = { margin: '0 8px 0 8px' };

const rowStyle = { margin: '4px 0 4px 0' };

storiesOf('Components|Button', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('all', () => (
    <>
      <div>
        <Button style={colStyle}>Base button</Button>
        <Button size="middle" style={colStyle}>
          Middle button
        </Button>
        <Button size="big" style={colStyle}>
          Big button
        </Button>
        <Button size="huge" style={colStyle}>
          Huge button
        </Button>
      </div>
      <div style={rowStyle}>
        <Button disabled style={colStyle}>
          Base button
        </Button>
        <Button disabled size="middle" style={colStyle}>
          Middle button
        </Button>
        <Button disabled size="big" style={colStyle}>
          Big button
        </Button>
        <Button disabled size="huge" style={colStyle}>
          Huge button
        </Button>
      </div>
      <div style={rowStyle}>
        <Button primary style={colStyle}>
          Base button
        </Button>
        <Button size="middle" primary style={colStyle}>
          Middle button
        </Button>
        <Button size="big" primary style={colStyle}>
          Big button
        </Button>
        <Button size="huge" primary style={colStyle}>
          Huge button
        </Button>
      </div>
      <div style={rowStyle}>
        <Button disabled primary style={colStyle}>
          Base button
        </Button>
        <Button disabled size="middle" primary style={colStyle}>
          Middle button
        </Button>
        <Button disabled size="big" primary style={colStyle}>
          Big button
        </Button>
        <Button disabled size="huge" primary style={colStyle}>
          Huge button
        </Button>
      </div>
    </>
  ));
