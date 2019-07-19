import React from 'react';
import { storiesOf } from '@storybook/react';
import { Toast, toastr } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Toast', module)
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .addParameters({ options: { showAddonPanel: false } })
  .add('all', () => {
    return (
      <>
        <Section>

          <Toast>

            {toastr.success('Demo text for success Toast closes in 30 seconds or on click', null, 30000)}
            {toastr.error('Demo text for error Toast closes in 28 seconds or on click', null, 28000)}
            {toastr.warning('Demo text for warning Toast closes in 25 seconds or on click', null, 25000)}
            {toastr.info('Demo text for info Toast closes in 15 seconds or on click', null, 15000)}

            {toastr.success('Demo text for success Toast with title closes in 12 seconds or on click', 'Demo title', 12000)}
            {toastr.error('Demo text for error Toast with title closes in 10 seconds or on click', 'Demo title', 10000)}
            {toastr.warning('Demo text for warning Toast with title closes in 8 seconds or on click', 'Demo title', 8000)}
            {toastr.info('Demo text for info Toast with title closes in 6 seconds or on click', 'Demo title', 6000)}

            {toastr.success('Demo text for success manual closed Toast', null, 0, true, true)}
            {toastr.error('Demo text for error manual closed Toast', null, 0, true, true)}
            {toastr.warning('Demo text for warning manual closed Toast', null, 0, true, true)}
            {toastr.info('Demo text for info manual closed Toast', null, 0, true, true)}

            {toastr.success('Demo text for success manual closed Toast with title', 'Demo title', 0, true, true)}
            {toastr.error('Demo text for error manual closed Toast with title', 'Demo title', 0, true, true)}
            {toastr.warning('Demo text for warning manual closed Toast with title', 'Demo title', 0, true, true)}
            {toastr.info('Demo text for info manual closed Toast with title', 'Demo title', 0, true, true)}

          </Toast>
          
        </Section>
      </>
    );
  });
