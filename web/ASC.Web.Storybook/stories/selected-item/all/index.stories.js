import React from 'react';
import { storiesOf } from '@storybook/react';
import { SelectedItem } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import styled from '@emotion/styled';

const StyledContainer = styled.div`
  padding:0;
  display: grid;
  grid-gap: 10px;
`;

const StyledContainerInline = styled.div`
  display: flex;
  flex-wrap: wrap;
  margin-bottom:10px;

  > * {
    margin-right: 10px;
    margin-bottom:10px;
  }
`;

storiesOf('Components|SelectedItem', module)
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .addParameters({ options: { showAddonPanel: false } })
  .add('all', () => {

    return (
      <Section>
        <StyledContainerInline>
          <SelectedItem
            text='Selected item'
            isInline={true}
          />
          <SelectedItem
            text='Selected item'
            isInline={true}
          />
          <SelectedItem
            text='Selected item'
            isInline={true}
          />
          <SelectedItem
            text='Selected item'
            isInline={true}
          />
        </StyledContainerInline>

        <StyledContainer>
          <SelectedItem
            text='Selected item'
            isInline={false}
          />
          <SelectedItem
            text='Selected item'
            isInline={false}
          />
          <SelectedItem
            text='Selected item'
            isInline={false}
          />
          <SelectedItem
            text='Selected item'
            isInline={false}
          />
        </StyledContainer>
      </Section>
    )
  });
