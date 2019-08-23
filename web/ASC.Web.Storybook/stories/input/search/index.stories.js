import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { StringValue } from 'react-values';
import { withKnobs, boolean, text, select, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { SearchInput, Button } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const sizeOptions = ['base', 'middle', 'big', 'huge'];

function getData() {
    return [
      { key: 'filter-status', group: 'filter-status', label: 'Status', isHeader: true },
        { key: '0', group: 'filter-status', label: 'Active' },
        { key: '1', group: 'filter-status', label: 'Disabled' },
      { key: 'filter-type', group: 'filter-type', label: 'Type', isHeader: true },
        { key: '0', group: 'filter-type', label: 'Folders' },
      { key: 'filter-other', group: 'filter-other', label: 'Other', isHeader: true },
        { key: '0', group: 'filter-other', subgroup: 'filter-groups', defaultSelectLabel: 'Select', label: 'Groups' },
          { key: '0', inSubgroup: true, group: 'filter-groups', label: 'Administration'},
        { key: '1', group: 'filter-other', subgroup: 'filter-empty', defaultSelectLabel: 'Select', label: 'Empty' },
        { key: '2', group: 'filter-other', subgroup: 'filter-test', defaultSelectLabel: 'Select', label: 'Test' },
          { key: '0', inSubgroup: true, group: 'filter-test', label: 'test1'},
          { key: '1', inSubgroup: true, group: 'filter-test', label: 'test2'},

    ];
}
class SearchStory extends React.Component  {
  constructor(props) {
    super(props);
    this.state = {
      selectedFilterData: []
    };
    this.buttonClick = this.buttonClick.bind(this);
  }
  buttonClick(){
    this.setState({ selectedFilterData:[
        {key: "1", group: "filter-status"},
        {key: "-1", group: "filter-groups"}
      ]
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
              <SearchInput 
                id={text('id', '')}
                isDisabled={boolean('isDisabled', false)}
                size={select('size', sizeOptions, 'base')}
                scale={boolean('scale', false)}
                isNeedFilter={boolean('isNeedFilter', true)}
                getFilterData={getData}
                selectedFilterData={this.state.selectedFilterData}
                placeholder={text('placeholder', 'Search')}
                onChangeFilter={(result) => {console.log(result)}}
                value={value}
                onChange={e => { 
                    set(e.target.value);
                }}
            />
            </Section>
          )}
        </StringValue>
      </Section>
    )
  }
}

storiesOf('Components|Input', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('search', () => (
    <SearchStory />
  ));