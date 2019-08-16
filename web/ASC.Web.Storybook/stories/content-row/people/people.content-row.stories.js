import React from 'react';
import { storiesOf } from '@storybook/react';
import { ContentRow, PeopleRow } from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';

const users = [
  {
    id: '1',
    userName: 'Helen Walton',
    avatar: '',
    role: 'owner',
    status: 'normal',
    isHead: false,
    department: 'Administration',
    mobilePhone: '+5 104 6473420',
    email: 'percival1979@yahoo.com',
    contextOptions: [
      { key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail') },
      { key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message') },
      { key: 'key3', isSeparator: true },
      { key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit') },
      { key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password') },
      { key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail') },
      { key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable') }
    ]
  },
  {
    id: '2',
    userName: 'Nellie Harder',
    avatar: '',
    role: 'user',
    status: 'normal',
    isHead: true,
    department: 'Development',
    mobilePhone: '+1 716 3748605',
    email: 'herta.reynol@yahoo.com',
    contextOptions: []
  },
  {
    id: '3',
    userName: 'Alan Mason',
    avatar: '',
    role: 'admin',
    status: 'normal',
    isHead: true,
    department: '',
    mobilePhone: '+3 956 2064314',
    email: 'davin_lindgr@hotmail.com',
    contextOptions: [
      { key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail') },
      { key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message') },
      { key: 'key3', isSeparator: true },
      { key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit') },
      { key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password') },
      { key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail') },
      { key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable') }
    ]
  },
  {
    id: '4',
    userName: 'Michael Goldstein',
    avatar: '',
    role: 'guest',
    status: 'normal',
    isHead: false,
    department: 'Visitors',
    mobilePhone: '+7 715 6018678',
    email: 'fidel_kerlu@hotmail.com',
    contextOptions: [
      { key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail') },
      { key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message') },
      { key: 'key3', isSeparator: true },
      { key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit') },
      { key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password') },
      { key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail') },
      { key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable') }
    ]
  },
  {
    id: '5',
    userName: 'Robert Gardner Robert Gardner',
    avatar: '',
    role: 'user',
    status: 'pending',
    isHead: false,
    department: '',
    mobilePhone: '',
    email: 'robert_gardner@hotmail.com',
    contextOptions: [
      { key: 'key1', label: 'Edit', onClick: () => console.log('Context action: Edit') },
      { key: 'key2', label: 'Invite again', onClick: () => console.log('Context action: Invite again') },
      { key: 'key3', label: 'Delete profile', onClick: () => console.log('Context action: Delete profile') }
    ]
  },
  {
    id: '6',
    displayName: 'Timothy Morphis',
    avatar: '',
    role: 'user',
    status: 'disabled',
    isHead: false,
    department: 'Disabled',
    mobilePhone: '',
    email: 'timothy_j_morphis@hotmail.com',
    contextOptions: [
      { key: 'key1', label: 'Edit', onClick: () => console.log('Context action: Edit') },
      { key: 'key2', label: 'Reassign data', onClick: () => console.log('Context action: Reassign data') },
      { key: 'key3', label: 'Delete personal data', onClick: () => console.log('Context action: Delete personal data') },
      { key: 'key4', label: 'Delete profile', onClick: () => console.log('Context action: Delete profile') }
    ]
  }
];

storiesOf('EXAMPLES|ContentRow', module)
  .add('people row', () => {

    return (
      <Section>
        {users.map(user => {
          return (
            <ContentRow 
              key={user.id}
              status={user.status}
              checked={false}
              data={user}
              avatarRole={user.role}
              avatarSource={user.avatar}
              avatarName={user.userName}
              contextOptions={user.contextOptions}
            >
              <PeopleRow
                status={user.status}
                displayName={user.userName}
                department={user.department}
                phone={user.mobilePhone}
                email={user.email}
              />
            </ContentRow>
          );
        })}
      </Section>
    );
  });
