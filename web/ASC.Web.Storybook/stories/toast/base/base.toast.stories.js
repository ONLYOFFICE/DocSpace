import React from 'react';
import { storiesOf } from '@storybook/react';
import { Toast, toastr } from 'asc-web-components';
import Readme from './README.md';
import { text, boolean, withKnobs, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';

const type = ['success', 'error', 'warning', 'info'];

storiesOf('Components|Toast', module)
   .addDecorator(withKnobs)
   .addDecorator(withReadme(Readme))
   .add('base', () => {
      const toastType = `${select('type', type, 'success')}`;
      const toastData = `${text('data (only text in storybook)', 'Demo text for Toast')}`;
      const titleToast = `${text('title', 'Demo title')}`;
      const withCross = boolean('withCross', false);
      const timeout = number('timeout', '5000');
      return (
         <>
            <Toast />
            <Section>
               <button onClick={() => {
                  switch (toastType) {
                     case 'error':
                        toastr.error(toastData, titleToast, timeout, withCross);
                        break;
                     case 'warning':
                        toastr.warning(toastData, titleToast, timeout, withCross);
                        break;
                     case 'info':
                        toastr.info(toastData, titleToast, timeout, withCross);
                        break;
                     default:
                        toastr.success(toastData, titleToast, timeout, withCross);
                        break;
                  }
               }}>
                  Show toast</button>
            </Section>
         </>
      );
   });
