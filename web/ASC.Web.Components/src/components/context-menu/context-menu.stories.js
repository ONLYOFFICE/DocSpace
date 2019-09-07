import React from 'react';
import { storiesOf } from '@storybook/react';
import ContextMenu from '.';
import Row from '../row';
import Section from '../../../.storybook/decorators/section';
import Readme from './README.md';
import withReadme from 'storybook-readme/with-readme';

storiesOf('Components|ContextMenu', module)
  .addDecorator(withReadme(Readme))
  .add('base', () => {
    const array = Array.from(Array(10).keys());

    let localState = [];

    const getOptions = options => {
      return localState[0] = options;
    };

    return (
      <Section>
        <div id='rowContainer'>
        {array.map(item => {
          const context = [{ key: item, label: item },{ key: item+1, label: item+1 },{ key: item+2, label: item+2 }];
          return (
            <Row key={item} checked={false} contextOptions={context} onContextClick={getOptions} >
              <span>Context me {item}</span>
            </Row>
          );
        })}
        </div>
        <ContextMenu targetAreaId='rowContainer' options={localState} />
      </Section>
    );
  });