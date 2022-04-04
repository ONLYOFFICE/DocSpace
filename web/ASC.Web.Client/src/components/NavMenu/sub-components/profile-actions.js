import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Avatar from "@appserver/components/avatar";
import DropDownItem from "@appserver/components/drop-down-item";
import Link from "@appserver/components/link";
import ProfileMenu from "./profile-menu";
import api from "@appserver/common/api";

import ToggleButton from "@appserver/components/toggle-button";

const StyledDiv = styled.div`
  width: 32px;
  height: 32px;
`;

const StyledDropdownItem = styled(DropDownItem)`
  justify-content: space-between;

  .toggle-button {
    position: relative;
    align-items: center;

    grid-gap: 0px;
  }
`;

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
    if (this.props.userIsUpdate) {
      this.getAvatar();
    } else {
      this.setState({ avatar: this.state.user.avatar });
    }
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
      <StyledDiv isProduct={this.props.isProduct} ref={this.ref}>
        <Avatar
          style={{ width: "32px", height: "32px" }}
          onClick={this.onClick}
          role={userRole}
          size="min"
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
          forwardedRef={this.ref}
        >
          <div style={{ paddingTop: "8px" }}>
            {this.props.userActions.map((action) =>
              action && !action?.withToggle ? (
                <Link
                  noHover={true}
                  key={action.key}
                  href={action.url}
                  onClick={this.onClickItemLink}
                >
                  <DropDownItem {...action} />
                </Link>
              ) : (
                <StyledDropdownItem noHover={true} key={action.key}>
                  {action.label}
                  <ToggleButton
                    className="toggle-button"
                    onChange={action.onClick}
                    isChecked={action.isChecked}
                  />
                </StyledDropdownItem>
              )
            )}
          </div>
        </ProfileMenu>
      </StyledDiv>
    );
  }
}

ProfileActions.propTypes = {
  opened: PropTypes.bool,
  user: PropTypes.object,
  userActions: PropTypes.array,
  userIsUpdate: PropTypes.bool,
  setUserIsUpdate: PropTypes.func,
  isProduct: PropTypes.bool,
};

ProfileActions.defaultProps = {
  opened: false,
  user: {},
  userActions: [],
  userIsUpdate: false,
  isProduct: false,
};

export default ProfileActions;
