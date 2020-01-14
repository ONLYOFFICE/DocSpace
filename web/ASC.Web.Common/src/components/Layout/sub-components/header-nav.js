import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import ProfileActions from "./profile-actions";
import { utils } from "asc-web-components";
const { tablet } = utils.device;

const StyledNav = styled.nav`
  display: flex;
  padding: 0 24px 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: 190;

  & > div {
    margin: 0 0 0 16px;
    padding: 0;
    min-width: 24px;
  }

  @media ${tablet} {
    padding: 0 16px;
  }
`;

const HeaderNav = React.memo(props => {
  //console.log("HeaderNav render");
  return (
    <StyledNav>
      {props.modules.map(module => (
        <NavItem
          key={module.id}
          iconName={module.iconName}
          badgeNumber={module.notifications}
          onClick={module.onClick}
          onBadgeClick={module.onBadgeClick}
          noHover={true}
        />
      ))}
      {props.user && (
        <ProfileActions userActions={props.userActions} user={props.user} />
      )}
    </StyledNav>
  );
});

HeaderNav.displayName = "HeaderNav";

HeaderNav.propTypes = {
  modules: PropTypes.array,
  user: PropTypes.object,
  userActions: PropTypes.array
};

export default HeaderNav;
