import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, boolean, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import { DropDown, DropDownItem, DropDownProfileItem } from 'asc-web-components'
import Section from '../../../.storybook/decorators/section';

storiesOf('Components | DropDown | DropDownItem', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base item', () => {

    const isUserPreview = boolean('Show user preview', true);
    const isHeader = boolean('Show category`s', true);
    const isSeparator = boolean('Show separator', true);
    const useIcon = boolean('Show icons', true);
    const direction = select('direction', ['left', 'right'], 'left');

    return (
      <Section>
        <DropDown
          isUserPreview={isUserPreview}
          withArrow={isUserPreview}
          directionX={direction}
          manualY='1%'
          opened={true}>
          {isUserPreview &&
            <DropDownProfileItem
              avatarRole='admin'
              avatarSource='https://static-www.onlyoffice.com/images/team/developers_photos/personal_44_2x.jpg'
              displayName='Jane Doe'
              email='janedoe@gmail.com' />
          }
          <DropDownItem
            isHeader={isHeader}
            label={isHeader ? 'Category' : ''}
            onClick={() => console.log('Button 1 clicked')} />
          <DropDownItem
            icon={useIcon ? 'WindowsMsnIcon' : ''}
            label='Button 1'
            onClick={() => console.log('Button 1 clicked')} />
          <DropDownItem
            icon={useIcon ? 'PlaneIcon' : ''}
            label='Button 2'
            onClick={() => console.log('Button 2 clicked')} />
          <DropDownItem
            disabled
            icon={useIcon ? 'CopyIcon' : ''}
            label='Button 3'
            onClick={() => console.log('Button 3 clicked')} />
          <DropDownItem
            icon={useIcon ? 'ActionsDocumentsIcon' : ''}
            label='Button 4'
            onClick={() => console.log('Button 4 clicked')} />
          <DropDownItem
            isSeparator={isSeparator} />
          <DropDownItem
            isHeader={isHeader}
            label={isHeader ? 'Category' : ''}
            onClick={() => console.log('Button 1 clicked')} />
          <DropDownItem
            icon={useIcon ? 'NavLogoIcon' : ''}
            label='Button 5'
            onClick={() => console.log('Button 5 clicked')} />
          <DropDownItem
            disabled
            icon={useIcon ? 'NavLogoIcon' : ''}
            label='Button 6'
            onClick={() => console.log('Button 6 clicked')} />
        </DropDown>
      </Section>
    )
  });