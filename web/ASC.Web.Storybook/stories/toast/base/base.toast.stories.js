import React from 'react';
import { storiesOf } from '@storybook/react';
import { Toast, toastr } from 'asc-web-components';
import Readme from './README.md';
import { text, boolean, withKnobs, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Section from '../../../.storybook/decorators/section';

const type = ['success', 'error', 'warning', 'info'];

storiesOf('Components|Toast', module)
   .addDecorator(withKnobs)
   .addDecorator(withReadme(Readme))
   .add('base', () => {
      const toastType = `${select('type', type, 'success')}`;
      const toastText = `${text('text', 'Demo text for Toast')}`;
      const titleToast = `${text('title', 'Demo title')}`;
      const autoClosed = `${boolean('autoClosed', true)}`;

      return (
         <>
            <Toast />
            <Section>
               <button onClick={() => {
                  switch (toastType) {
                     case 'error':
                        toastr.error(toastText, titleToast, JSON.parse(autoClosed));
                        break;
                     case 'warning':
                        toastr.warning(toastText, titleToast, JSON.parse(autoClosed));
                        break;
                     case 'info':
                        toastr.info(toastText, titleToast, JSON.parse(autoClosed));
                        break;
                     default:
                        toastr.success(toastText, titleToast, JSON.parse(autoClosed));
                        break;
                  }
               }}>
                  Show toast</button>
            </Section>
         </>
      );
   });
