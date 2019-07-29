import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, text, boolean } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Paging } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import { isUndefined } from 'util';

storiesOf('Components|Paging', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => {

        const pageItems = [
            {
                key: '1',
                label: '1 of 5',
                onClick: () => console.log('set paging 1 of 5')
            },
            {
                key: '2',
                label: '2 of 5',
                onClick: () => console.log('set paging 2 of 5')
            },
            {
                key: '3',
                label: '3 of 5',
                onClick: () => console.log('set paging 3 of 5')
            },
            {
                key: '4',
                label: '4 of 5',
                onClick: () => console.log('set paging 4 of 5')
            },
            {
                key: '5',
                label: '5 of 5',
                onClick: () => console.log('set paging 5 of 5')
            }
        ];

        const perPageItems = [
            {
                key: '1-1',
                label: '25 per page',
                onClick: () => console.log('set paging 25 action')
            },
            {
                key: '1-2',
                label: '50 per page',
                onClick: () => console.log('set paging 50 action')
            },
            {
                key: '1-3',
                label: '100 per page',
                onClick: () => console.log('set paging 100 action')
            }
        ];

        const displayItems = boolean('Display pageItems', true);
        const displayPerPage = boolean('Display perPageItems', true);
      
        return (
            <Section>
                <Paging previousLabel={text('previousLabel', 'Previous')} 
                        nextLabel={text('nextLabel', 'Next')} 
                        pageItems={displayItems ? pageItems : undefined}
                        perPageItems={displayPerPage ? perPageItems : undefined}
                        disablePrevious={boolean('disablePrevious', false)}
                        disableNext={boolean('disableNext', false)}
                        previousAction={ () => console.log('Prev')}
                        nextAction={ () => console.log('Next')}
                        openDirection='bottom'
                />
            </Section>
        )
    });