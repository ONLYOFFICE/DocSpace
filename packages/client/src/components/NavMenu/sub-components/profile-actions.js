import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import Avatar from "@docspace/components/avatar";
import DropDownItem from "@docspace/components/drop-down-item";
import Link from "@docspace/components/link";
import ProfileMenu from "./profile-menu";
import api from "@docspace/common/api";

import ToggleButton from "@docspace/components/toggle-button";
import Button from "@docspace/components/button";

const StyledDiv = styled.div`
  width: 32px;
  height: 32px;

  ${isMobileOnly &&
  css`
    @media (min-width: 428px) {
      .backdrop-active {
        background-color: unset;
        backdrop-filter: unset;
      }
    }
  `}
`;

const StyledButtonWrapper = styled.div`
  width: 100%;

  padding: 12px 16px;

  box-sizing: border-box;
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
      this.getAvatar();
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
    let isModuleAdmin =
      user?.listAdminModules && user?.listAdminModules?.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  onClose = (e) => {
    const path = e.path || (e.composedPath && e.composedPath());
    const dropDownItem = path ? path.find((x) => x === this.ref.current) : null;
    if (dropDownItem) return;

    const navElement = document.getElementsByClassName("profileMenuIcon");

    if (navElement?.length > 0) {
      navElement[0].style.setProperty("z-index", 180, "important");
    }

    this.setOpened(!this.state.opened);
  };

  onClick = (action, e) => {
    action.onClick && action.onClick(e);

    const navElement = document.getElementsByClassName("profileMenuIcon");

    if (navElement?.length > 0) {
      navElement[0].style.setProperty("z-index", 210, "important");
    }

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
            {this.props.userActions.map(
              (action, index) =>
                action &&
                (action?.isButton ? (
                  <StyledButtonWrapper key={action.key}>
                    <Button
                      size="normal"
                      scale={true}
                      label={action.label}
                      onClick={action.onClick}
                    />
                  </StyledButtonWrapper>
                ) : (
                  <Link
                    noHover={true}
                    key={action.key}
                    href={action.url}
                    onClick={this.onClickItemLink}
                  >
                    <DropDownItem {...action} />
                  </Link>
                ))
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
