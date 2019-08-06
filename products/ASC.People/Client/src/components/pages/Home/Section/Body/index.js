import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ContentRow, toastr } from "asc-web-components";
import UserContent from "./userContent";
//import config from "../../../../../../package.json";
import { selectUser, deselectUser, setSelection } from "../../../../../store/people/actions";
import { isSelected, getUserStatus, getUserRole, isUserDisabled } from '../../../../../store/people/selectors';
import { isAdmin } from '../../../../../store/auth/selectors';

class SectionBodyContent extends React.PureComponent {

  onEmailSentClick = () => {
    toastr.success("Context action: Send e-mail");
  }

  onSendMessageClick = () => {
    toastr.success("Context action: Send message");
  }

  onEditClick = (user) => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/edit/${user.userName}`);
  }

  onChangePasswordClick = () => {
    toastr.success("Context action: Change password");
  }

  onChangeEmailClick = () => {
    toastr.success("Context action: Change e-mail");
  }

  onDisableClick = () => {
    toastr.success("Context action: Disable");
  }

  getUserContextOptions = (user) => {

    const options = [{
          key: "key1",
          label: "Send e-mail",
          onClick: this.onEmailSentClick
      },
      {
          key: "key2",
          label: "Send message",
          onClick: this.onSendMessageClick
      },
      { key: "key3", isSeparator: true },
      {
          key: "key4",
          label: "Edit",
          onClick: this.onEditClick.bind(this, user)
      },
      {
          key: "key5",
          label: "Change password",
          onClick: this.onChangePasswordClick
      },
      {
          key: "key6",
          label: "Change e-mail",
          onClick: this.onChangeEmailClick
      }];

    return [...options,
         !isUserDisabled(user)
        ? {
            key: "key7",
            label: "Disable",
            onClick: this.onDisableClick
        }
        : {}
    ];
  };

  onContentRowSelect = (checked, user) => {
    console.log("ContentRow onSelect", checked, user);
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
          const contextOptions = this.getUserContextOptions(user);
          return isAdmin ? (
            <ContentRow
              key={user.id}
              status={getUserStatus(user)}
              data={user}
              avatarRole={getUserRole(user)}
              avatarSource={user.avatar}
              avatarName={user.displayName}
              contextOptions={contextOptions}
              checked={isSelected(selection, user.id)}
              onSelect={this.onContentRowSelect}
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
