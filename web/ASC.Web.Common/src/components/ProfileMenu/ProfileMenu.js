import React from "react";
import PropTypes from "prop-types";
import { Avatar, DropDown } from "asc-web-components";
import {
  AvatarContainer,
  LabelContainer,
  MainLabelContainer,
  MenuContainer,
  StyledProfileMenu,
  TopArrow,
} from "./StyledProfileMenu";

class ProfileMenu extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const {
      avatarRole,
      avatarSource,
      children,
      className,
      displayName,
      email,
      clickOutsideAction,
      open,
    } = this.props;

    return (
      <DropDown
        className={className}
        directionX="right"
        open={open}
        clickOutsideAction={clickOutsideAction}
      >
        <StyledProfileMenu>
          <MenuContainer>
            <AvatarContainer>
              <Avatar
                size="medium"
                role={avatarRole}
                source={avatarSource}
                userName={displayName}
              />
            </AvatarContainer>
            <MainLabelContainer>{displayName}</MainLabelContainer>
            <LabelContainer>{email}</LabelContainer>
          </MenuContainer>
          <TopArrow />
        </StyledProfileMenu>
        {children}
      </DropDown>
    );
  }
}

ProfileMenu.displayName = "ProfileMenu";

ProfileMenu.propTypes = {
  avatarRole: PropTypes.oneOf(["owner", "admin", "guest", "user"]),
  avatarSource: PropTypes.string,
  children: PropTypes.any,
  className: PropTypes.string,
  displayName: PropTypes.string,
  email: PropTypes.string,
  id: PropTypes.string,
  open: PropTypes.bool,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  clickOutsideAction: PropTypes.func,
};

export default ProfileMenu;
