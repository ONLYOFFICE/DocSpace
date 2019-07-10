import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Layout } from 'asc-web-components'

const currentUser = {
  id: '00000000-0000-0000-0000-000000000000',
  userName: 'Jane Doe',
  email: 'janedoe@gmail.com',
  isOwner: false,
  isAdmin: false,
  isVisitor: false,
  avatarSmall: '',
  avatarMedium: ''
};

const currentUserActions = [
  { key: 'ProfileBtn', label: 'Profile', onClick: () => console.log('ProfileBtn') },
  { key: 'AboutBtn', label: 'About', onClick: () => console.log('AboutBtn') },
  { key: 'LogoutBtn', label: 'Log out', onClick: () => console.log('LogoutBtn') },
];

const availableModules = [
  {
    seporator: true,
    id: 'nav-seporator-1'
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
    seporator: true,
    id: 'nav-seporator-2'
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

const onLogoClick = (e) => {action('Logo Clicked')(e)};

const asideContent = <p style={{padding: 40}}>Aside Content</p>;

const pageContent = <p style={{padding: 40}}>Page Content</p>;

storiesOf('Components|Layout', module)
  .add('Layout', () => (
    <Layout
      isNavigationOpen={false}
      currentUser={currentUser}
      currentUserActions={currentUserActions}
      availableModules={availableModules}
      currentModuleId={currentModuleId}
      onLogoClick={onLogoClick}
      asideContent={asideContent}
    >
      {pageContent}
    </Layout>
  ));
