import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import styled from '@emotion/styled';
import Layout from '../Layout';
import PageLayout from '.';
import Headline from '../Headline';
import {IconButton, ContextMenuButton, MainButton, SearchInput, Paging} from 'asc-web-components';
import withReadme from 'storybook-readme/with-readme';
import { boolean, withKnobs } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import withProvider from "../../../.storybook/decorators/redux";

const HeaderContent = styled.div`
  display: flex;
  align-items: center;

  & > * {
    margin-right: 8px !important;
  }
`;

const pageItems = [
  {
    key: '1',
    label: '1 of 2',
    onClick: (e) => action('set paging 1 of 2')(e)
  },
  {
    key: '2',
    label: '2 of 2',
    onClick: (e) => action('set paging 2 of 2')(e)
  }
];

const perPageItems = [
  {
    key: '1-1',
    label: '25 per page',
    onClick: (e) => action('set paging 25 action')(e)
  },
  {
    key: '1-2',
    label: '50 per page',
    onClick: (e) => action('set paging 50 action')(e)
  }
];

const articleHeaderContent = <Headline type="menu">Article Header</Headline>;

const articleMainButtonContent = <MainButton
  text='Actions'
  clickAction={(e) => action('MainButton Clicked')(e)}
/>;

const articleBodyContent = <p style={{ padding: 40 }}>Article Content</p>;

const sectionHeaderContent = <HeaderContent>
  <IconButton
    iconName={"ArrowPathIcon"}
    size='16'
    onClick={(e) => action('ArrowPathIcon Clicked')(e)}
  />
  <Headline type='content'>Section Header</Headline>
  <IconButton
    iconName={"PlusIcon"}
    size='16'
    onClick={(e) => action('PlusIcon Clicked')(e)}
  />
  <ContextMenuButton
    title="Actions"
    getData={() => [
      {
        key: 'key',
        label: 'label',
        onClick: (e) => action('label Clicked')(e)
      }
    ]}
  />
</HeaderContent>;

const sectionFilterContent = <SearchInput
  isNeedFilter={true}
  getFilterData={() => [
    {
      key: 'filter-example',
      group: 'filter-example',
      label: 'example group',
      isHeader: true
    },
    {
      key: 'filter-example-test',
      group: 'filter-example',
      label: 'Test'
    }
  ]}
  onSearchClick={(result) => { console.log(result) }}
  onChangeFilter={(result) => { console.log(result) }}
/>

const sectionBodyContent = <p style={{ padding: 40 }}>Section Content</p>;

const sectionPagingContent = <Paging
  previousLabel="Previous"
  nextLabel="Next"
  pageItems={pageItems}
  perPageItems={perPageItems}
  selectedPageItem={pageItems[0]}
  selectedCountItem={perPageItems[0]}
  onSelectPage={(a) => console.log(a)}
  onSelectCount={(a) => console.log(a)}
  previousAction={(e) => action('Prev Clicked')(e)}
  nextAction={(e) => action('Next Clicked')(e)}
  openDirection="top"
/>

storiesOf('Components|PageLayout', module)
  .addDecorator(withProvider)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Layout
      isBackdropVisible={boolean("isBackdropVisible", false)}
      isNavHoverEnabled={boolean("isNavHoverEnabled", true)}
      isNavOpened={boolean("isNavOpened", false)}
      isAsideVisible={boolean("isAsideVisible", false)}
    >
      <PageLayout
        articleHeaderContent={articleHeaderContent}
        articleMainButtonContent={articleMainButtonContent}
        articleBodyContent={articleBodyContent}
        sectionHeaderContent={sectionHeaderContent}
        sectionFilterContent={sectionFilterContent}
        sectionBodyContent={sectionBodyContent}
        sectionPagingContent={sectionPagingContent}
      />
    </Layout>

  ));
