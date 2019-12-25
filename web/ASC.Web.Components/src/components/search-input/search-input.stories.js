import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { StringValue } from 'react-values';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import SearchInput from '.';
import Button from '../button';
import Section from '../../../.storybook/decorators/section';

const sizeOptions = ['base', 'middle', 'big', 'huge'];

class SearchStory extends React.Component  {
  constructor(props) {
    super(props);
    this.state = {
      value: "test1"
    };
    this.buttonClick = this.buttonClick.bind(this);
  }
  buttonClick(){
    this.setState({ value: "test"});   
  }
  render(){
    return(
      <Section>
        <StringValue
          onChange={value => {
              action('onChange')(value);
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
                placeholder={text('placeholder', 'Search')}
                value={value}
                onChange={value => { 
                    set(value);
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