import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import styled from '@emotion/styled';
import Layout from '.';
import PageLayout from "../PageLayout";
import Headline from '../Headline';
import {IconButton, ContextMenuButton, MainButton, SearchInput, Paging} from "asc-web-components"
import withReadme from 'storybook-readme/with-readme';
import { boolean, withKnobs } from '@storybook/addon-knobs/react';
import Readme from './README.md';
import withProvider from "../../../.storybook/decorators/redux";

const currentUser = {
  id: '00000000-0000-0000-0000-000000000000',
  displayName: 'Jane Doe',
  email: 'janedoe@gmail.com',
  isOwner: false,
  isAdmin: false,
  isVisitor: false,
  avatarSmall: '',
  avatarMedium: ''
};

const currentUserActions = [
  {
    key: 'ProfileBtn',
    label: 'Profile',
    onClick: (e) => action('ProfileBtn Clicked')(e)
  },
  {
    key: 'AboutBtn',
    label: 'About',
    onClick: (e) => action('AboutBtn Clicked')(e)
  },
  {
    key: 'LogoutBtn',
    label: 'Log out',
    onClick: (e) => action('LogoutBtn Clicked')(e)
  }
];

const availableModules = [
  {
    separator: true,
    id: 'nav-separator-1'
  },
  {
    id: '11111111-1111-1111-1111-111111111111',
    title: 'Documents',
    iconName: 'DocumentsIcon',
    notifications: 2,
    url: '/products/documents/',
    onClick: (e) => action('DocumentsIcon Clicked')(e),
    onBadgeClick: (e) => action('DocumentsIconBadge Clicked')(e)
  },
  {
    id: '22222222-2222-2222-2222-222222222222',
    title: 'Chat',
    iconName: 'ChatIcon',
    notifications: 3,
    url: '/products/chat/',
    onClick: (e) => action('ChatIcon Clicked')(e),
    isolateMode: true
  },
  {
    id: '33333333-3333-3333-3333-333333333333',
    title: 'Mail',
    iconName: 'MailIcon',
    notifications: 7,
    url: '/products/mail/',
    onClick: (e) => action('MailIcon Clicked')(e),
    onBadgeClick: (e) => action('MailIconBadge Clicked')(e)
  },
  {
    id: '44444444-4444-4444-4444-444444444444',
    title: 'Projects',
    iconName: 'ProjectsIcon',
    notifications: 5,
    onClick: (e) => action('ProjectsIcon Clicked')(e),
    onBadgeClick: (e) => action('ProjectsIconBadge Clicked')(e)
  },
  {
    separator: true,
    id: 'nav-separator-2'
  },
  {
    id: '55555555-5555-5555-5555-555555555555',
    title: 'Calendar',
    iconName: 'CalendarCheckedIcon',
    notifications: 0,
    onClick: (e) => action('CalendarIcon Clicked')(e),
    onBadgeClick: (e) => action('CalendarIconBadge Clicked')(e)
  },
  {
    id: '66666666-6666-6666-6666-666666666666',
    title: 'CRM',
    iconName: 'CrmIcon',
    notifications: 0,
    onClick: (e) => action('CrmIcon Clicked')(e),
    isolateMode: true
  }
];

const currentModuleId = '44444444-4444-4444-4444-444444444444';

const onLogoClick = (e) => action('Logo Clicked')(e);

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

const asideContent = <p style={{ padding: 40 }}>Aside Content</p>;

const articleHeaderContent = <Headline type='menu'>Article Header</Headline>;

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

storiesOf('Components|Layout', module)
  .addDecorator(withProvider)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => (
    <Layout
      currentUser={currentUser}
      currentUserActions={currentUserActions}
      availableModules={availableModules}
      currentModuleId={currentModuleId}
      onLogoClick={onLogoClick}
      asideContent={asideContent}
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
