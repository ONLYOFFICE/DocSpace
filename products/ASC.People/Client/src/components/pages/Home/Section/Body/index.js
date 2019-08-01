import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ContentRow } from "asc-web-components";
import UserContent from "./userContent";
//import config from "../../../../../../package.json";
import { selectUser, deselectUser, setSelection } from "../../../../../store/people/actions";
import { isSelected, getUserStatus, getUserRole } from '../../../../../store/people/selectors';
import { isAdmin } from '../../../../../store/auth/selectors';

class SectionBodyContent extends React.PureComponent {

  getUserContextOptions = (user, isAdmin, history, settings) => {
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
            onClick: () => history.push(`${settings.homepage}/edit/${user.userName}`)
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

  onContentRowSelect = (checked, data, user) => {
    console.log("ContentRow onSelect", checked, data);
    if (checked) {
      this.props.selectUser(user);
    }
    else {
      this.props.deselectUser(user);
    }
  }

  render() {
    console.log("Home SectionBodyContent render()");
    const { users, isAdmin, selection, history, settings} = this.props;

    return (
      <>
        {users.map(user => {
          const contextOptions = this.getUserContextOptions(user, isAdmin, history, settings);
          return isAdmin ? (
            <ContentRow
              key={user.id}
              status={getUserStatus(user)}
              data={user}
              avatarRole={getUserRole(user)}
              avatarSource={user.avatar}
              avatarName={user.userName}
              contextOptions={contextOptions}
              checked={isSelected(selection, user.id)}
              onSelect={this.onContentRowSelect.bind(this, user)}
            >
              <UserContent
                user={user}
                history={history}
                settings={settings}
              />
            </ContentRow>
          ) : (
            <ContentRow
              key={user.id}
              status={getUserStatus(user)}
              avatarRole={getUserRole(user)}
              avatarSource={user.avatar}
              avatarName={user.userName}
            >
              <UserContent
                user={user}
                history={history}
                settings={settings}
              />
            </ContentRow>
          );
        })}
      </>
    );
  }
};

const mapStateToProps = state => {
  return {
    selection: state.people.selection,
    selected: state.people.selected,
    users: state.people.users,
    isAdmin: isAdmin(state.auth),
    settings: state.settings
  };
};

export default connect(
  mapStateToProps,
  { selectUser, deselectUser, setSelection }
)(withRouter(SectionBodyContent));
