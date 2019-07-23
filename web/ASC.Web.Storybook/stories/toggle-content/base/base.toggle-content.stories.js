import React from 'react';
import { storiesOf } from '@storybook/react';
import { ToggleContent } from 'asc-web-components';
import Readme from './README.md';
import withReadme from 'storybook-readme/with-readme';
import { text, withKnobs } from '@storybook/addon-knobs/react';

storiesOf('Components|ToggleContent', module)
   .addDecorator(withKnobs)
   .addDecorator(withReadme(Readme))
   .add('base', () => {
      return (
         <>
            <ToggleContent
               label={text('label', 'Some label')}
            >Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae
            </ToggleContent>
         </>
      );
   });
