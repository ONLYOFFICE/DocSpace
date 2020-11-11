import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import Headline from "../../Headline";
import Nav from "./nav";
import NavLogoItem from "./nav-logo-item";
import Loaders from "../../Loaders/index";

import { utils } from "asc-web-components";
import { connect } from "react-redux";
import {
  getCurrentProductId,
  getCurrentProductName,
  getDefaultPage,
  getMainModules,
  getTotalNotificationsCount,
  getIsLoaded,
} from "../../../store/auth/selectors";

const { desktop } = utils.device;

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  width: 100vw;
  height: 56px;

  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }

  .header-module-title {
    display: block;
    font-size: 21px;
    line-height: 0;

    @media ${desktop} {
      display: none;
    }
  }

  .header-logo-min_icon {
    display: none;
    cursor: pointer;

    width: 24px;
    height: 24px;

    @media (max-width: 620px) {
      padding: 0 12px 0 0;
      display: ${(props) => props.module && "block"};
    }
  }

  .header-logo-icon {
    width: 146px;
    height: 24px;
    position: relative;
    padding: 4px 20px 0 6px;
    cursor: pointer;

    @media (max-width: 620px) {
      display: ${(props) => (props.module ? "none" : "block")};
      padding: 0px 20px 0 6px;
    }
  }
`;

const HeaderComponent = ({
  currentProductName,
  totalNotifications,
  onClick,
  onNavMouseEnter,
  onNavMouseLeave,
  defaultPage,
  mainModules,
  isNavOpened,
  currentProductId,
  toggleAside,
  isLoaded,
  isAuthenticated,
  ...props
}) => {
  //console.log("Header render");

  const isNavAvailable = mainModules.length > 0;
  const onLogoClick = () => {
    window.open(defaultPage, "_self");
  };

  const onBadgeClick = (e) => {
    const item = mainModules.find(
      (module) => module.id === e.currentTarget.dataset.id
    );
    toggleAside();

    if (item) item.onBadgeClick(e);
  };

  return (
    <>
      <Header module={currentProductName}>
        {isLoaded && isAuthenticated ? (
          <>
            <NavItem
              iconName="MenuIcon"
              badgeNumber={totalNotifications}
              onClick={onClick}
              noHover={true}
            />

            <a className="header-logo-wrapper" href={defaultPage}>
              <img className="header-logo-icon" src={props.logoUrl} />
            </a>
          </>
        ) : !isLoaded && isAuthenticated ? (
          <Loaders.Header />
        ) : (
          <></>
        )}

        <Headline className="header-module-title" type="header" color="#FFF">
          {currentProductName}
        </Headline>
      </Header>
      {isNavAvailable && (
        <Nav
          opened={isNavOpened}
          onMouseEnter={onNavMouseEnter}
          onMouseLeave={onNavMouseLeave}
        >
          <NavLogoItem opened={isNavOpened} onClick={onLogoClick} />
          {mainModules.map(
            ({
              id,
              separator,
              iconName,
              iconUrl,
              notifications,
              onClick,
              url,
              title,
            }) => (
              <NavItem
                separator={!!separator}
                key={id}
                data-id={id}
                opened={isNavOpened}
                active={id == currentProductId}
                iconName={iconName}
                iconUrl={iconUrl}
                badgeNumber={notifications}
                onClick={onClick}
                onBadgeClick={onBadgeClick}
                url={url}
              >
                {title}
              </NavItem>
            )
          )}
        </Nav>
      )}
    </>
  );
};

HeaderComponent.displayName = "Header";

HeaderComponent.propTypes = {
  totalNotifications: PropTypes.number,
  onClick: PropTypes.func,
  currentProductName: PropTypes.string,
  defaultPage: PropTypes.string,
  mainModules: PropTypes.array,
  currentProductId: PropTypes.string,
  isNavOpened: PropTypes.bool,
  onNavMouseEnter: PropTypes.func,
  onNavMouseLeave: PropTypes.func,
  toggleAside: PropTypes.func,
  logoUrl: PropTypes.string,
  isLoaded: PropTypes.bool,
  isAuthenticated: PropTypes.bool,
};

const mapStateToProps = (state) => {
  const { logoUrl } = state.auth.settings;
  const { isAuthenticated } = state.auth;

  return {
    defaultPage: getDefaultPage(state),
    totalNotifications: getTotalNotificationsCount(state),
    mainModules: getMainModules(state),
    currentProductName: getCurrentProductName(state),
    currentProductId: getCurrentProductId(state),
    isLoaded: getIsLoaded(state),
    logoUrl,
    isAuthenticated,
  };
};

export default connect(mapStateToProps)(HeaderComponent);
