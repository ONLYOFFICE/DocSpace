import React from 'react';
import { storiesOf } from '@storybook/react';
import { Nav } from 'asc-web-components';

storiesOf('Components|Nav', module)
  .add('base', () => (
      <Nav></Nav>
  ));