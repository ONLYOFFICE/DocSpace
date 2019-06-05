import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme';
import styled from '@emotion/styled';
import { withKnobs, select, color } from '@storybook/addon-knobs/react';
import Readme from './README.md';

import {CalendarIcon, PeopleIcon} from 'asc-web-components';


const IconList = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr 1fr 1fr 1fr;
`;

const IconItem = styled.div`
  padding: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
`;

const IconContainer = styled.div`
  margin: 16px 0;
`;

const sizeOptions = ['small', 'medium', 'big'];

storiesOf('Components|Icons', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('All Icons', () => (
    <IconList>
      <IconItem>
        <IconContainer>
          <CalendarIcon
            size={select('size', sizeOptions, 'medium')}
            color={color('color', "dimgray")}
          />
        </IconContainer>
        <span>{CalendarIcon.displayName}</span>
      </IconItem>
      <IconItem>
        <IconContainer>
          <PeopleIcon
            size={select('size', sizeOptions, 'medium')}
            color={color('color', "dimgray")}
          />
        </IconContainer>
        <span>{PeopleIcon.displayName}</span>
      </IconItem>
    </IconList>
  ));