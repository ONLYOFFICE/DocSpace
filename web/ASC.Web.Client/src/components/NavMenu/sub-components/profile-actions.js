import React from "react";
import PropTypes from "prop-types";
import Avatar from "@appserver/components/avatar";
import DropDownItem from "@appserver/components/drop-down-item";
import Link from "@appserver/components/link";
import ProfileMenu from "./profile-menu";
import api from "@appserver/common/api";
class ProfileActions extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      opened: props.opened,
      user: props.user,
      avatar: "",
    };
  }

  setOpened = (opened) => {
    this.setState({ opened: opened });
  };

  componentDidMount() {
    this.getAvatar();
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.user !== prevProps.user) {
      this.setState({ user: this.props.user });
    }

    if (this.props.opened !== prevProps.opened) {
      this.setOpened(this.props.opened);
    }

    if (this.props.userIsUpdate !== prevProps.userIsUpdate) {
      this.getAvatar();
      this.props.setUserIsUpdate(false);
    }
  }

  getUserRole = (user) => {
    let isModuleAdmin = user.listAdminModules && user.listAdminModules.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  onClose = (e) => {
    const path = e.path || (e.composedPath && e.composedPath());
    const dropDownItem = path ? path.find((x) => x === this.ref.current) : null;
    if (dropDownItem) return;

    this.setOpened(!this.state.opened);
  };

  onClick = (action, e) => {
    action.onClick && action.onClick(e);

    this.setOpened(!this.state.opened);
  };

  onClickItemLink = (e) => {
    this.setOpened(!this.state.opened);

    e.preventDefault();
  };

  getAvatar = async () => {
    const user = await api.people.getUser();
    const avatar = user.avatar;
    this.setState({ avatar: avatar });
  };

  render() {
    //console.log("Layout sub-component ProfileActions render");
    const { user, opened, avatar } = this.state;
    const userRole = this.getUserRole(user);

    return (
      <div ref={this.ref}>
        <Avatar
          onClick={this.onClick}
          role={userRole}
          size="small"
          source={avatar}
          userName={user.displayName}
          className="icon-profile-menu"
        />
        <ProfileMenu
          className="profile-menu"
          avatarRole={userRole}
          avatarSource={avatar}
          displayName={user.displayName}
          email={user.email}
          open={opened}
          clickOutsideAction={this.onClose}
        >
          <div style={{ paddingTop: "8px" }}>
            {this.props.userActions.map((action) => (
              <Link
                noHover={true}
                key={action.key}
                href={action.url}
                onClick={this.onClickItemLink}
              >
                <DropDownItem {...action} />
              </Link>
            ))}
          </div>
        </ProfileMenu>
      </div>
    );
  }
}

ProfileActions.propTypes = {
  opened: PropTypes.bool,
  user: PropTypes.object,
  userActions: PropTypes.array,
  userIsUpdate: PropTypes.bool,
  setUserIsUpdate: PropTypes.func,
};

ProfileActions.defaultProps = {
  opened: false,
  user: {},
  userActions: [],
  userIsUpdate: false,
};

export default ProfileActions;
