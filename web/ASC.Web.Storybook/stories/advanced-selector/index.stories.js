import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, text, number } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { AdvancedSelector } from 'asc-web-components';
import Section from '../../.storybook/decorators/section';
import { boolean } from '@storybook/addon-knobs/dist/deprecated';

storiesOf('Components|AdvancedSelector', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => {
    const optionsCount = number("Users count", 100);

    let options = [{key: "self", label: "Me"}]; 
    
    options = [...options, ...[...Array(optionsCount).keys()].map(
      index => {
        return { key: `user${index}`, label: `User ${index+1} of ${optionsCount}` };
      }
    )];

    return (
    <Section>
        <AdvancedSelector 
          placeholder={text("placeholder", "Search users")}
          onSearchChanged={(e) => console.log(e.target.value)}
          options={options}
          isMultiSelect={boolean("isMultiSelect", true)}
          buttonLabel={text("buttonLabel", "Add members")}
          onSelect={(selectedOptions) => console.log("onSelect", selectedOptions)}
        />
    </Section>
    );
  });