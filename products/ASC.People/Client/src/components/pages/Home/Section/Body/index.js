import React from 'react';
import { connect } from 'react-redux';
import { ContentRow } from 'asc-web-components';
import UserContent from './userContent';

const getUserDepartment = (user) => {
  return {
      title: user.department,
      action: () => console.log("Department action")
    };
};

const getUserPhone = (user) => {
  return {
      title: user.mobilePhone,
      action: () => console.log("Phone action")
    };
};

const getUserEmail = (user) => {
  return {
      title: user.email,
      action: () => console.log("Email action")
    };
};

const getUserRole = (user) => {
  if(user.isOwner)
    return "owner";
  else if(user.isAdmin)
    return "admin";
  else if(user.isVisitor)
    return "guest";
  else
    return "user";
};

const getUserStatus = (user) => {
  if(user.state === 1 && user.activationStatus === 1)
    return "normal";
  else if(user.state === 1 && user.activationStatus === 2)
    return "pending";
  else if(user.state === 2)
    return "disabled";
  else
    return "normal";
};

const getUserContextOptions = (user) => {
  return [
    {
      key: "key1",
      label: "Send e-mail",
      onClick: () => console.log("Context action: Send e-mail")
    },
    {
      key: "key2",
      label: "Send message",
      onClick: () => console.log("Context action: Send message")
    },
    { key: "key3", isSeparator: true },
    {
      key: "key4",
      label: "Edit",
      onClick: () => console.log("Context action: Edit")
    },
    {
      key: "key5",
      label: "Change password",
      onClick: () => console.log("Context action: Change password")
    },
    {
      key: "key6",
      label: "Change e-mail",
      onClick: () => console.log("Context action: Change e-mail")
    },
    {
      key: "key7",
      label: "Disable",
      onClick: () => console.log("Context action: Disable")
    }
  ];
};

const getIsHead = (user) => {
  return false;
};

const SectionBodyContent = ({ isAdmin, users, onSelect, isHeaderChecked }) => {
  console.log("SectionBodyContent isAdmin=", isAdmin);

  return (
    <>
      {users.map(user => (
        isAdmin ?
        <ContentRow
          key={user.id}
          status={getUserStatus(user)}
          data={user}
          avatarRole={getUserRole(user)}
          avatarSource={user.avatar}
          avatarName={user.userName}
          contextOptions={getUserContextOptions(user, isAdmin)}
          checked={isHeaderChecked}
          onSelect= {(checked, data) => {
            // console.log("ContentRow onSelect", checked, data);
            onSelect(checked, data);
          }}
        >
          <UserContent
            userName={user.userName}
            department={getUserDepartment(user)}
            phone={getUserPhone(user)}
            email={getUserEmail(user)}
            headDepartment={getIsHead(user)}
            status={user.status}
          />
        </ContentRow>
        :
        <ContentRow
          key={user.id}
          status={getUserStatus(user)}
          avatarRole={getUserRole(user)}
          avatarSource={user.avatar}
          avatarName={user.userName}
        >
          <UserContent
            userName={user.userName}
            department={getUserDepartment(user)}
            phone={getUserPhone(user)}
            email={getUserEmail(user)}
            headDepartment={getIsHead(user)}
            status={user.status}
          />
        </ContentRow>
      ))}
    </>
  );
};


const mapStateToProps = (state) => {
  return {
    isAdmin: state.auth.user.isAdmin || state.auth.user.isOwner
  }
}

export default connect(mapStateToProps)(SectionBodyContent);