import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import Headline from "../../Headline";
import Nav from "./nav";
import NavLogoItem from "./nav-logo-item";
import Loaders from "../../Loaders/index";
import { ReactSVG } from "react-svg";

import { utils } from "asc-web-components";
import { useTranslation } from "react-i18next";
// import { connect } from "react-redux";
// import {
//   getCurrentProductId,
//   getCurrentProductName,
//   getDefaultPage,
//   //getMainModules,
//   getTotalNotificationsCount,
//   getIsLoaded,
// } from "../../../store/auth/selectors";
//import store from "../../../store";
//const { moduleStore, settingsStore } = store;

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
    padding: 0 20px 0 6px;
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
  isAdmin,
  ...props
}) => {
  //console.log("Header render");
  const { t } = useTranslation();

  const isNavAvailable = mainModules.length > 0;
  const onLogoClick = () => {
    window.open(defaultPage, "_self");
  };

  const onBadgeClick = (e) => {
    if (!e) return;
    const id = e.currentTarget.dataset.id;
    const item = mainModules.find((m) => m.id === id);
    toggleAside();

    if (item) item.onBadgeClick(e);
  };

  const onItemClick = (e) => {
    if (!e) return;
    const link = e.currentTarget.dataset.link;
    window.open(link, "_self");
    e.preventDefault();
  };

  const getCustomModules = () => {
    if (!isAdmin) {
      return [];
    } // Temporarily hiding the settings module

    return (
      <>
        <NavItem
          separator={true}
          key={"nav-modules-separator"}
          data-id={"nav-modules-separator"}
        />
        <NavItem
          separator={false}
          key={"settings"}
          data-id={"settings"}
          data-link="/settings"
          opened={isNavOpened}
          active={"settings" == currentProductId}
          iconName={"SettingsIcon"}
          onClick={onItemClick}
          url="/settings"
        >
          {t("Settings")}
        </NavItem>
      </>
    );
  };

  return (
    <>
      <Header module={currentProductName}>
        <NavItem
          iconName="MenuIcon"
          badgeNumber={totalNotifications}
          onClick={onClick}
          noHover={true}
        />

        <a className="header-logo-wrapper" href={defaultPage}>
          <ReactSVG
            className="header-logo-icon"
            loading={() => (
              <Loaders.Rectangle
                width="168"
                height="24"
                backgroundColor="#fff"
                foregroundColor="#fff"
                backgroundOpacity={0.25}
                foregroundOpacity={0.2}
              />
            )}
            src={props.logoUrl}
          />
        </a>
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
          <NavItem
            separator={true}
            key={"nav-products-separator"}
            data-id={"nav-products-separator"}
          />
          {mainModules.map(
            ({
              id,
              separator,
              iconName,
              iconUrl,
              notifications,
              link,
              title,
            }) => (
              <NavItem
                separator={!!separator}
                key={id}
                data-id={id}
                data-link={link}
                opened={isNavOpened}
                active={id == currentProductId}
                iconName={iconName}
                iconUrl={iconUrl}
                badgeNumber={notifications}
                onClick={onItemClick}
                onBadgeClick={onBadgeClick}
                url={link}
              >
                {title}
              </NavItem>
            )
          )}
          {getCustomModules()}
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
  isAdmin: PropTypes.bool,
};

export default inject(({ auth }) => {
  const {
    settingsStore,
    moduleStore,
    isLoaded,
    isAuthenticated,
    isAdmin,
    product,
  } = auth;
  const { logoUrl, defaultPage, currentProductId } = settingsStore;
  const { modules, totalNotifications } = moduleStore;

  const mainModules = modules.filter((m) => !m.isolateMode);

  return {
    isAdmin,
    defaultPage,
    logoUrl,
    mainModules,
    totalNotifications,
    isLoaded,
    isAuthenticated,
    currentProductId,
    currentProductName: (product && product.title) || "",
  };
})(observer(HeaderComponent));
