import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { StringValue } from 'react-values';
import { withKnobs, boolean, text, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from '../README.md';
import { FilterInput } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const sizeOptions = ['base', 'middle', 'big', 'huge'];

const defaultFilterData = {
  filterValue: [
    {key: "filter-status", value: "filter-status-disabled"}
  ],
  sortDirection: "desc",
  sortId: "name",
  inputValue: "text"
};

function getData() {
    return [
      { key: 'filter-status', group: 'filter-status', label: 'Status', isHeader: true },
      { key: 'filter-status-active', group: 'filter-status', label: 'Active' },
      { key: 'filter-status-disabled', group: 'filter-status', label: 'Disabled' },
      { key: 'filter-type', group: 'filter-type', label: 'Type', isHeader: true },
      { key: 'filter-type-administrator', group: 'filter-type', label: 'Folders' },
      { key: 'filter-type-employee', group: 'filter-type', label: 'Employee' },
    ];
  }
function getSortData() {
    return [
        {
            id: 'name', label: 'Name'
        },
        {
            id: 'surname', label: 'Surname'
        }
    ];
}
storiesOf('Components|Filter', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Section>
        <StringValue
          onChange={e => {
              action('onChange')(e);
            }
          }
        >
          {({ value, set }) => (
            <Section>
                <div>
                    <FilterInput 
                        id={text('id', '')}
                        isDisabled={boolean('isDisabled', false)}
                        size={select('size', sizeOptions, 'base')}
                        scale={boolean('scale', false)}
                        isNeedFilter={boolean('isNeedFilter', true)}
                        getFilterData={getData}
                        getSortData={getSortData}
                        placeholder={text('placeholder', 'Search')}
                        onFilter={(result) => {console.log(result)}}
                        value={value}
                        defaultFilterData={defaultFilterData}
                        onChange={e => { 
                            set(e.target.value);
                        }}
                    />
                </div>
            </Section>
          )}
        </StringValue>
    </Section>
  ));