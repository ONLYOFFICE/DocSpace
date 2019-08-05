import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, text, boolean, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Paging } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Paging', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => {

    const pageItems = [
      {
        key: 1,
        label: '1 of 5'
      },
      {
        key: 2,
        label: '2 of 5'
      },
      {
        key: 3,
        label: '3 of 5'
      },
      {
        key: 4,
        label: '4 of 5'
      },
      {
        key: 5,
        label: '5 of 5'
      }
    ];

    const countItems = [
      {
        key: 25,
        label: '25 per page'
      },
      {
        key: 50,
        label: '50 per page'
      },
      {
        key: 100,
        label: '100 per page'
      }
    ];

    const displayItems = boolean('Display pageItems', true);
    const displayCount = boolean('Display countItems', true);
    const selectedPage = select('selectedPage', [1, 2, 3, 4, 5], 3);
    const selectedCount = select('selectedCount', [25, 50, 100], 100);

    return (
      <Section>
        <Paging previousLabel={text('previousLabel', 'Previous')}
          nextLabel={text('nextLabel', 'Next')}
          pageItems={displayItems ? pageItems : undefined}
          countItems={displayCount ? countItems : undefined}
          disablePrevious={boolean('disablePrevious', false)}
          disableNext={boolean('disableNext', false)}
          previousAction={() => console.log('Prev')}
          nextAction={() => console.log('Next')}
          openDirection='bottom'
          selectedPage={selectedPage}
          selectedCount={selectedCount}
          onSelectPage={(a) => console.log(a)}
          onSelectCount={(a) => console.log(a)}
        />
      </Section>
    )
  });