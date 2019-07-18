import React from 'react';
import { ContentRow } from 'asc-web-components';
import UserContent from './userContent';

const SectionBodyContent = ({ users, onSelect, isHeaderChecked }) => {
  return (
    <>
      {users.map(user => (
        <ContentRow
          key={user.id}
          status={user.status}
          checked={isHeaderChecked}
          data={user}
          onSelect={(checked, data) => {
            // console.log("ContentRow onSelect", checked, data);
            onSelect(checked, data);
          }}
          avatarRole={user.role}
          avatarSource={user.avatar}
          avatarName={user.userName}
          contextOptions={user.contextOptions}
        >
          <UserContent
            userName={user.userName}
            department={user.departments[0]}
            phone={user.phones[0]}
            email={user.emails[0]}
            headDepartment={user.isHead}
            status={user.status}
          />
        </ContentRow>
      ))}
    </>
  );
};


export default SectionBodyContent;