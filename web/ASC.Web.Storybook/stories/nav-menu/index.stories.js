import React from 'react';
import { storiesOf } from '@storybook/react';
import { NavMenu, NavLogo } from 'asc-web-components';

//"https://nct.onlyoffice.com/skins/default/images/onlyoffice_logo/light_small_general.svg"

storiesOf('Components|NavMenu', module)
  .add('base', () => (
    <NavMenu>
    </NavMenu>
  ))
  .add('with logo', () => (
    <NavMenu>
      <NavLogo imageUrl="./light_small_general.svg" href="/?path=/story/components-navmenu--base" />
    </NavMenu>
  ));