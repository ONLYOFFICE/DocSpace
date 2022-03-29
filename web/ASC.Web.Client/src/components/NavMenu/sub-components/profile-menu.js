import React from "react";
import PropTypes from "prop-types";
import Avatar from "@appserver/components/avatar";
import DropDown from "@appserver/components/drop-down";

import styled, { css } from "styled-components";
import DropDownItem from "@appserver/components/drop-down-item";
import { isDesktop, isTablet } from "react-device-detect";
import { Base } from "@appserver/components/themes";

const commonStyle = css`
  font-family: "Open Sans", sans-serif, Arial;
  font-style: normal;
  color: ${(props) => props.theme.menuContainer.color};
  margin-left: 60px;
  margin-top: -3px;
  max-width: 300px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export const StyledProfileMenu = styled(DropDownItem)`
  position: relative;
  overflow: visible;
  padding: 0px;
  cursor: pointer;
  display: inline-block;
  margin-top: -6px;
`;

export const MenuContainer = styled.div`
  position: relative;
  height: 76px;
  background: ${(props) => props.theme.menuContainer.background};
  border-radius: 6px 6px 0px 0px;
  padding: 16px;
  cursor: default;
  box-sizing: border-box;
`;

MenuContainer.defaultProps = { theme: Base };

export const AvatarContainer = styled.div`
  display: inline-block;
  float: left;
`;

export const MainLabelContainer = styled.div`
  font-size: 16px;
  line-height: 28px;

  ${commonStyle}
`;

MainLabelContainer.defaultProps = { theme: Base };

export const LabelContainer = styled.div`
  font-weight: normal;
  font-size: 11px;
  line-height: 16px;

  ${commonStyle}
`;

LabelContainer.defaultProps = { theme: Base };

export const TopArrow = styled.div`
  position: absolute;
  cursor: default;
  top: -6px;
  right: 16px;
  width: 24px;
  height: 6px;
  box-sizing: border-box;
  border-left: 12px solid transparent;
  border-right: 12px solid transparent;
  border-bottom: 6px solid ${(props) => props.theme.menuContainer.arrowTop};
`;

TopArrow.defaultProps = { theme: Base };

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
      forwardedRef,
    } = this.props;
    const right = isTablet ? "4px" : "8px";
    const top = "62px";

    return (
      <DropDown
        className={className}
        directionX="right"
        right={right}
        top={top}
        open={open}
        clickOutsideAction={clickOutsideAction}
        forwardedRef={forwardedRef}
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
