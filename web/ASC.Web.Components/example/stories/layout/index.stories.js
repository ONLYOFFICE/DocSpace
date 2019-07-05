import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { Layout } from 'asc-web-components'

const currentUser = {
  id: '00000000-0000-0000-0000-000000000000',
  name: 'Jane Doe',
  email: 'janedoe@gmail.com',
  role: 'user',
  url: '/products/peaople/profile',
  smallAvatar: '',
  mediumAvatar: ''
};

const availableModules = [
  {
    id: '11111111-1111-1111-1111-111111111111',
    name: 'Documents',
    iconName: 'DocumentsIcon',
    notifications: 2,
    url: '/products/documents/',
    onClick: (e) => action('DocumentsIcon Clicked')(e),
    onBadgeClick: (e) => action('DocumentsIconBadge Clicked')(e)
  },
  {
    id: '22222222-2222-2222-2222-222222222222',
    name: 'Chat',
    iconName: 'ChatIcon',
    notifications: 10,
    url: '/products/chat/',
    onClick: (e) => action('ChatIcon Clicked')(e),
    onBadgeClick: (e) => action('ChatIconBadge Clicked')(e)
  },
  {
    id: '33333333-3333-3333-3333-333333333333',
    name: 'Mail',
    iconName: 'MailIcon',
    notifications: 7,
    url: '/products/mail/',
    onClick: (e) => action('MailIcon Clicked')(e),
    onBadgeClick: (e) => action('MailIconBadge Clicked')(e)
  }
];

const currentModuleId = '11111111-1111-1111-1111-111111111111';

const chatModuleId = '22222222-2222-2222-2222-222222222222';

storiesOf('Components|Layout', module)
  .add('Layout', () => (
    <Layout
      isNavigationOpen={false}
      currentUser={currentUser}
      availableModules={availableModules}
      currentModuleId={currentModuleId}
      chatModuleId={chatModuleId}
    >
      <div style={{padding: 40}}>Page Content</div>
    </Layout>
  ));
