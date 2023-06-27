import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import ProfileActions from "./profile-actions";
import { useTranslation } from "react-i18next";
import { tablet, mobile } from "@docspace/components/utils/device";
import { inject, observer } from "mobx-react";
import { isMobile, isMobileOnly } from "react-device-detect";

const StyledNav = styled.nav`
  display: flex;
  padding: 0 20px 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 48px;
  z-index: 180 !important;

  & > div {
    margin: 0 0 0 16px;
    padding: 0;
    min-width: 24px;
  }

  @media ${tablet} {
    padding: 0 16px;
  }
  .icon-profile-menu {
    cursor: pointer;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  ${isMobile &&
  css`
    padding: 0 16px 0 16px !important;
  `}

  @media ${mobile} {
    padding: 0 16px 0 16px;
  }

  ${isMobileOnly &&
  css`
    padding: 0 0 0 16px !important;
  `}
`;
const HeaderNav = ({
  user,
  isAuthenticated,
  userIsUpdate,
  setUserIsUpdate,
  getActions,
}) => {
  const { t } = useTranslation(["NavMenu", "Common", "About"]);
  const userActions = getActions(t);

  return (
    <StyledNav className="profileMenuIcon hidingHeader">
      {isAuthenticated && user ? (
        <>
          <ProfileActions
            userActions={userActions}
            user={user}
            userIsUpdate={userIsUpdate}
            setUserIsUpdate={setUserIsUpdate}
          />
        </>
      ) : (
        <></>
      )}
    </StyledNav>
  );
};

HeaderNav.displayName = "HeaderNav";

HeaderNav.propTypes = {
  user: PropTypes.object,
  isAuthenticated: PropTypes.bool,
};

export default inject(({ auth, profileActionsStore }) => {
  const { userStore, isAuthenticated } = auth;
  const { user, userIsUpdate, setUserIsUpdate } = userStore;
  const { getActions } = profileActionsStore;

  return {
    user,
    isAuthenticated,
    userIsUpdate,
    setUserIsUpdate,
    getActions,
  };
})(observer(HeaderNav));
