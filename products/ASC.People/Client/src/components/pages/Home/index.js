import React, { useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import _ from "lodash";
import { PageLayout } from "asc-web-components";
import {ArticleHeaderContent, ArticleBodyContent, ArticleMainButtonContent} from '../../Article';
import {SectionHeaderContent, SectionBodyContent, SectionFilterContent, SectionPagingContent} from './Section';

let selection = [];

const Home = ({users}) => {
  const [isHeaderVisible, toggleHeaderVisible] = useState(false);
  const [isHeaderIndeterminate, toggleHeaderIndeterminate] = useState(false);
  const [isHeaderChecked, toggleHeaderChecked] = useState(false);
  
  const renderGroupButtonMenu = () => {
    const headerVisible = selection.length > 0;
    const headerIndeterminate = headerVisible && selection.length > 0 && selection.length < users.length;
    const headerChecked = headerVisible && selection.length === users.length;
    
    /*console.log(`renderGroupButtonMenu()
      headerVisible=${headerVisible} 
      headerIndeterminate=${headerIndeterminate} 
      headerChecked=${headerChecked}
      selection.length=${selection.length}
      users.length=${users.length}`);*/

    if(headerVisible)
      toggleHeaderVisible(headerVisible);

    if(isHeaderIndeterminate !== headerIndeterminate)
      toggleHeaderIndeterminate(headerIndeterminate);

    if(isHeaderChecked !== headerChecked)
      toggleHeaderChecked(headerChecked);
  };

  const onRowSelect = (checked, data) => {
    /*console.log(`onBodySelect 
        row.checked=${checked}`,
          data,
          selection);*/

    const id = _.result(
      _.find(selection, function(obj) {
        return obj.id === data.id;
      }),
      "id"
    );
    if (checked && !id) {
      selection.push(data);
    } else if (id) {
      selection = _.filter(selection, function(obj) {
        return obj.id !== id;
      });
    }

    renderGroupButtonMenu();
  };

  return (
    <PageLayout
      articleHeaderContent={<ArticleHeaderContent />}
      articleMainButtonContent={<ArticleMainButtonContent />}
      articleBodyContent={<ArticleBodyContent />}
      sectionHeaderContent={
        <SectionHeaderContent
          isHeaderVisible={isHeaderVisible}
          isHeaderIndeterminate={isHeaderIndeterminate}
          isHeaderChecked={isHeaderChecked}
          onCheck={checked => {
            toggleHeaderChecked(checked);
            selection = checked ? [...users] : [];
            /*console.log(`SectionHeaderContent onCheck 
                selection.length=${selection.length}`)*/
            renderGroupButtonMenu();
          }}
        />
      }
      sectionFilterContent={<SectionFilterContent />}
      sectionBodyContent={
        <SectionBodyContent
          users={users}
          isHeaderChecked={isHeaderChecked}
          onSelect={onRowSelect}
        />
      }
      sectionPagingContent={<SectionPagingContent />}
    />
  );
};

Home.propTypes = {
  users: PropTypes.array.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

const getUserDepartments = (user) => {
  return [
    {
      title: user.department,
      action: () => console.log("Department action")
    }
  ];
};

const getUserPhones = (user) => {
  return [
    {
      title: "+5 104 6473420",
      action: () => console.log("Phone action")
    }
  ];
}

const getUserEmails = (user) => {
  return [
    {
      title: user.email,
      action: () => console.log("Email action")
    }
  ];
}

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
}

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

const convertUsers = (users) => {
  return users.map(user => {
    return {
      id: user.id,
      userName: user.userName,
      avatar: user.avatar,
      role: getUserRole(user),
      status: getUserStatus(user),
      isHead: false,
      departments: getUserDepartments(user),
      phones: getUserPhones(user),
      emails: getUserEmails(user),
      contextOptions: getUserContextOptions(user)
    }
  });
};

function mapStateToProps(state) {
  const users = convertUsers(state.people.users)
  return {
    users: users,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(withRouter(Home));