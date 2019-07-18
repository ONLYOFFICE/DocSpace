import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Paging } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

storiesOf('Components|Paging', module)
    .addDecorator(withReadme(Readme))
    .add('base', () => {

        const pageItems = [
            {
                label: '1 of 5',
                onClick: () => console.log('set paging 1 of 5')
            },
            {
                label: '2 of 5',
                onClick: () => console.log('set paging 2 of 5')
            },
            {
                label: '3 of 5',
                onClick: () => console.log('set paging 3 of 5')
            },
            {
                label: '4 of 5',
                onClick: () => console.log('set paging 4 of 5')
            },
            {
                label: '5 of 5',
                onClick: () => console.log('set paging 5 of 5')
            }
        ];

        const perPageItems = [
            {
                label: '25 per page',
                onClick: () => console.log('set paging 25 action')
            },
            {
                label: '50 per page',
                onClick: () => console.log('set paging 50 action')
            },
            {
                label: '100 per page',
                onClick: () => console.log('set paging 100 action')
            }
        ];
      
        return (
            <Section>
                <Paging previousLabel='Previous' 
                        nextLabel='Next' 
                        pageItems={pageItems}
                        perPageItems={perPageItems}
                        previousAction={ () => console.log('Prev')}
                        nextAction={ () => console.log('Next')}
                />
            </Section>
        )
    });