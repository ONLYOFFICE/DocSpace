import React from 'react';
import { storiesOf } from '@storybook/react';
import { NavMenu } from 'asc-web-components';

//"https://nct.onlyoffice.com/skins/default/images/onlyoffice_logo/light_small_general.svg"

storiesOf('Components|NavMenu', module)
  .add('base', () => (
      <NavMenu logoUrl="/light_small_general.svg" href="/?path=/story/components-navmenu--base" >
      </NavMenu>
  ));