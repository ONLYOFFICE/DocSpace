import React, { memo } from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import {
  ContentRow,
  toastr,
  CustomScrollbarsVirtualList,
  EmptyScreenContainer,
  Icons,
  Link
} from "asc-web-components";
import UserContent from "./userContent";
//import config from "../../../../../../package.json";
import {
  selectUser,
  deselectUser,
  setSelection
} from "../../../../../store/people/actions";
import {
  isUserSelected,
  getUserStatus,
  getUserRole,
  isUserDisabled
} from "../../../../../store/people/selectors";
import { isAdmin, isMe } from "../../../../../store/auth/selectors";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";

const Row = memo(
  ({
    data,
    index,
    style,
    onContentRowSelect,
    history,
    settings,
    selection,
    viewer,
    getUserContextOptions
  }) => {
    // Data passed to List as "itemData" is available as props.data
    const user = data[index];
    const contextOptions = getUserContextOptions(user, viewer);
    const contextOptionsProps = !contextOptions.length ? {} : {contextOptions};

    return (
      <ContentRow
        key={user.id}
        status={getUserStatus(user)}
        data={user}
        avatarRole={getUserRole(user)}
        avatarSource={user.avatar}
        avatarName={user.displayName}
        checked={isUserSelected(selection, user.id)}
        onSelect={onContentRowSelect}
        style={style}
        {...contextOptionsProps}
      >
        <UserContent user={user} history={history} settings={settings} />
      </ContentRow>
    );
  },
  areEqual
);

class SectionBodyContent extends React.PureComponent {
  onEmailSentClick = () => {
    toastr.success("Context action: Send e-mail");
  };

  onSendMessageClick = () => {
    toastr.success("Context action: Send message");
  };

  onEditClick = user => {
    const { history, settings } = this.props;
    history.push(`${settings.homepage}/edit/${user.userName}`);
  };

  onChangePasswordClick = () => {
    toastr.success("Context action: Change password");
  };

  onChangeEmailClick = () => {
    toastr.success("Context action: Change e-mail");
  };

  onDisableClick = () => {
    toastr.success("Context action: Disable");
  };

  onEnableClick = () => {
    toastr.success("Context action: Enable");
  };
  
  onReassignDataClick = () => {
    toastr.success("Context action: Reassign data");
  };
  
  onDeletePersonalDataClick = (user) => {
    toastr.success("Context action: Delete personal data");
  };
  
  onDeleteProfileClick = () => {
    toastr.success("Context action: Delete profile");
  };
  
  onInviteAgainClick = () => {
    toastr.success("Context action: Invite again");
  };
  getUserContextOptions = (user, viewer) => {

    let status = "";

    if(isAdmin(viewer) || (!isAdmin(viewer) && isMe(user, viewer.userName))) {
      status = getUserStatus(user); 
    }

    //console.log("getUserContextOptions", user, viewer, status);

    switch (status) {
      case "normal":
      case "unknown":
        return [
          {
            key: "send-email",
            label: "Send e-mail",
            onClick: this.onEmailSentClick
          },
          {
            key: "send-message",
            label: "Send message",
            onClick: this.onSendMessageClick
          },
          { key: "key3", isSeparator: true },
          {
            key: "edit",
            label: "Edit",
            onClick: this.onEditClick.bind(this, user)
          },
          {
            key: "change-password",
            label: "Change password",
            onClick: this.onChangePasswordClick
          },
          {
            key: "change-email",
            label: "Change e-mail",
            onClick: this.onChangeEmailClick
          },
          {
            key: "disable",
            label: "Disable",
            onClick: this.onDisableClick
          }
        ];
      case "disabled":
        return [
          {
            key: "enable",
            label: "Enable",
            onClick: this.onEnableClick
          },
          {
            key: "reassign-data",
            label: "Reassign data",
            onClick: this.onReassignDataClick
          },
          {
            key: "delete-personal-data",
            label: "Delete personal data",
            onClick: this.onDeletePersonalDataClick.bind(this, user)
          },
          {
            key: "delete-profile",
            label: "Delete profile",
            onClick: this.onDeleteProfileClick
          }
        ];
      case "pending":
        return [
          {
            key: "edit",
            label: "Edit",
            onClick: this.onEditClick.bind(this, user)
          },
          {
            key: "invite-again",
            label: "Invite again",
            onClick: this.onInviteAgainClick
          },
          {
            key: "delete-profile",
            label: "Delete profile",
            onClick: this.onDeleteProfileClick
          }
        ];
      default:
        return [];
    }
  };

  onContentRowSelect = (checked, user) => {
    console.log("ContentRow onSelect", checked, user);
    if (checked) {
      this.props.selectUser(user);
    } else {
      this.props.deselectUser(user);
    }
  };

  render() {
    console.log("Home SectionBodyContent render()");
    const { users, viewer, selection, history, settings } = this.props;

    return users.length > 0 ? (
      <AutoSizer>
        {({ height, width }) => (
          <List
            className="List"
            height={height}
            width={width}
            itemSize={46} // ContentRow height
            itemCount={users.length}
            itemData={users}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {({ data, index, style }) => (
              <Row
                data={data}
                index={index}
                style={style}
                onContentRowSelect={this.onContentRowSelect}
                history={history}
                settings={settings}
                selection={selection}
                viewer={viewer}
                getUserContextOptions={this.getUserContextOptions}
              />
            )}
          </List>
        )}
      </AutoSizer>
    ) : (
      <EmptyScreenContainer
        imageSrc="images/empty_screen_filter.png"
        imageAlt="Empty Screen Filter image"
        headerText="No results matching your search could be found"
        descriptionText="No people matching your filter can be displayed in this section. Please select other filter options or clear filter to view all the people in this section."
        buttons={
          <>
            <Icons.CrossIcon size="small" style={{ marginRight: "4px" }} />
            <Link
              type="action"
              isHovered={true}
              onClick={e => console.log("Reset filter clicked", e)}
            >
              Reset filter
            </Link>
          </>
        }
      />
    );
  }
}

SectionBodyContent.defaultProps = {
  users: []
};

const mapStateToProps = state => {
  return {
    selection: state.people.selection,
    selected: state.people.selected,
    users: state.people.users,
    viewer: state.auth.user,
    settings: state.auth.settings
  };
};

export default connect(
  mapStateToProps,
  { selectUser, deselectUser, setSelection }
)(withRouter(SectionBodyContent));
