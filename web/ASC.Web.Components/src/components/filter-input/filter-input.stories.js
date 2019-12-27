import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { StringValue } from 'react-values';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import FilterInput from '.';
import Button from '../button';
import Section from '../../../.storybook/decorators/section';

const sizeOptions = ['base', 'middle', 'big', 'huge'];

function getData() {
    return [
      { key: 'filter-status', group: 'filter-status', label: 'Status', isHeader: true },
        { key: '0', group: 'filter-status', label: 'Active' },
        { key: '1', group: 'filter-status', label: 'Disabled' },
      { key: 'filter-type', group: 'filter-type', label: 'Type', isHeader: true },
        { key: '0', group: 'filter-type', label: 'Folders' },
        { key: '1', group: 'filter-type', label: 'Employee' },
      { key: 'filter-test', group: 'filter-test', label: 'Test', isHeader: true },
        { key: '0', group: 'filter-test', label: 'test1' },
        { key: '1', group: 'filter-test', label: 'test2' },
      { key: 'filter-other', group: 'filter-other', label: 'Other', isHeader: true },
        { key: '0', group: 'filter-other', subgroup: 'filter-groups', defaultSelectLabel: 'Select', label: 'Groups' },
          { key: '0', inSubgroup: true, group: 'filter-groups', label: 'Administration'},
          { key: '1', inSubgroup: true, group: 'filter-groups', label: 'Public Relations'}
    ];
  }
function getSortData() {
    return [
        {key: 'name', label: 'Name', default: true},
        {key: 'surname', label: 'Surname', default: true}
    ];
}

class FilterStory extends React.Component  {
  constructor(props) {
    super(props);
    this.state = {
      selectedFilterData: {
        inputValue: "text",
        filterValues: [
          {key: "1", group: "filter-status"}
        ]
      }
    };
    this.buttonClick = this.buttonClick.bind(this);
  }
  buttonClick(){
    this.setState({ 
      selectedFilterData: {
        filterValues: [
          {key: "-1", group: "filter-groups"}
        ],
        sortDirection: "asc",
        sortId: "surname",
        inputValue: "text 123"
      }
    });   
  }
  render(){
    return(
      <Section>
        <StringValue
          onChange={e => {
              action('onChange')(e);
            }
          }
        >
          {({ value, set }) => (
            <Section>
                <div style={{marginBottom: '20px'}}>
                  <Button
                      label="Change props"
                      onClick={this.buttonClick}
                  />
                </div>
                <div>
                    <FilterInput 
                        id={text('id', '')}
                        isDisabled={boolean('isDisabled', false)}
                        size={select('size', sizeOptions, 'base')}
                        scale={boolean('scale', false)}
                        getFilterData={getData}
                        getSortData={getSortData}
                        placeholder={text('placeholder', 'Search')}
                        onFilter={(result) => {console.log(result)}}
                        value={value}
                        selectedFilterData={this.state.selectedFilterData}
                        onChange={e => { 
                            set(e.target.value);
                        }}
                    />
                </div>
            </Section>
          )}
        </StringValue>
    </Section>
    )
  }
}

storiesOf('Components|Filter', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <FilterStory/>
  ));