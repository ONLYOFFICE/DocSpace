import React from 'react';
import { storiesOf } from '@storybook/react';
import { NavMenu } from 'asc-web-components';

storiesOf('Components|NavMenu', module)
  .add('base', () => (
      <NavMenu></NavMenu>
  ));