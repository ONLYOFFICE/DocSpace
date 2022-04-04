import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Link as LinkWithoutRedirect } from "react-router-dom";
import { isMobileOnly, isMobile } from "react-device-detect";
import NavItem from "./nav-item";
import Headline from "@appserver/common/components/Headline";
import Nav from "./nav";
import NavLogoItem from "./nav-logo-item";
import Link from "@appserver/components/link";
import history from "@appserver/common/history";
import { useTranslation } from "react-i18next";
import HeaderNavigationIcon from "./header-navigation-icon";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import {
  desktop,
  isDesktop,
  tablet,
  mobile,
} from "@appserver/components/utils/device";
import i18n from "../i18n";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import NoUserSelect from "@appserver/components/utils/commonStyles";
import {
  getLink,
  checkIfModuleOld,
  onItemClick,
} from "@appserver/studio/src/helpers/utils";
import StyledExternalLinkIcon from "@appserver/studio/src/components/StyledExternalLinkIcon";
import HeaderCatalogBurger from "./header-catalog-burger";
import { Base } from "@appserver/components/themes";

const { proxyURL } = AppServerConfig;

const Header = styled.header`
  display: flex;
  align-items: center;

  background-color: ${(props) => props.theme.header.backgroundColor};

  width: 100vw;
  height: 48px;

  .header-logo-wrapper {
    height: 24px;
    height: 26px;

    display: flex;
    align-items: center;

    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    ${NoUserSelect}
  }

  .header-logo-icon {
    ${(props) =>
      (props.isPersonal || props.isPreparationPortal) && `margin-left: 20px;`}
    height: 24px;
    position: relative;
    padding-right: 20px;
    padding-left: ${(props) =>
      !props.needNavMenu || props.isPersonal || props.isDesktopView
        ? "20px"
        : "4px"};
    cursor: pointer;

    @media ${tablet} {
      padding-left: 16px;
    }
  }
  .mobile-short-logo {
    width: 146px;
  }

  .header-items-wrapper {
    display: flex;
    margin-left: 82px;
  }
`;

Header.defaultProps = { theme: Base };

const StyledLink = styled.div`
  display: inline;
  .nav-menu-header_link {
    color: ${(props) => props.theme.header.linkColor};
    font-size: 13px;
  }

  a {
    text-decoration: none;
  }
  :hover {
    color: ${(props) => props.theme.header.linkColor};
    -webkit-text-decoration: underline;
    text-decoration: underline;
  }
`;

StyledLink.defaultProps = { theme: Base };

const versionBadgeProps = {
  fontWeight: "600",
  fontSize: "13px",
};

const StyledNavigationIconsWrapper = styled.div`
  height: 20px;
  position: absolute;
  left: ${isMobile ? "254px" : "275px"};
  display: ${isMobileOnly ? "none" : "flex"};
  justify-content: flex-start;
  align-items: center;

  @media ${tablet} {
    left: 254px;
  }

  @media ${mobile} {
    display: none;
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
  version,
  isAuthenticated,
  isAdmin,
  backdropClick,
  isPersonal,
  isPreparationPortal,
  theme,
  toggleArticleOpen,
  ...props
}) => {
  const { t } = useTranslation("Common");

  const isNavAvailable = mainModules.length > 0;

  const onLogoClick = () => {
    history.push(defaultPage);
    backdropClick();
  };

  const onBadgeClick = React.useCallback((e) => {
    if (!e) return;
    const id = e.currentTarget.dataset.id;
    const item = mainModules.find((m) => m.id === id);
    toggleAside();

    if (item) item.onBadgeClick(e);
  }, []);

  const handleItemClick = React.useCallback((e) => {
    onItemClick(e);
    backdropClick();
  }, []);

  const numberOfModules = mainModules.filter((item) => !item.separator).length;
  const needNavMenu = currentProductId !== "home";
  const mainModulesWithoutSettings = mainModules.filter(
    (module) => module.id !== "settings"
  );

  const navItems = mainModulesWithoutSettings.map(
    ({ id, separator, iconUrl, notifications, link, title, dashed }) => {
      const itemLink = getLink(link);
      const shouldRenderIcon = checkIfModuleOld(link);
      return (
        <NavItem
          separator={!!separator}
          key={id}
          data-id={id}
          data-link={itemLink}
          opened={isNavOpened}
          active={id == currentProductId}
          iconUrl={iconUrl}
          badgeNumber={notifications}
          onClick={handleItemClick}
          onBadgeClick={onBadgeClick}
          url={itemLink}
          dashed={dashed}
        >
          {title}
          {shouldRenderIcon && <StyledExternalLinkIcon color={linkColor} />}
        </NavItem>
      );
    }
  );

  const [isDesktopView, setIsDesktopView] = useState(isDesktop());

  const onResize = () => {
    const isDesktopView = isDesktop();
    if (isDesktopView === isDesktopView) setIsDesktopView(isDesktopView);
  };

  useEffect(() => {
    window.addEventListener("resize", onResize);
    return () => window.removeEventListener("resize", onResize);
  });

  return (
    <>
      <Header
        module={currentProductName}
        isLoaded={isLoaded}
        isPersonal={isPersonal}
        isPreparationPortal={isPreparationPortal}
        isAuthenticated={isAuthenticated}
        className="navMenuHeader hidingHeader"
        needNavMenu={needNavMenu}
        isDesktopView={isDesktopView}
      >
        {currentProductId !== "home" && (
          <HeaderCatalogBurger onClick={toggleArticleOpen} />
        )}
        <LinkWithoutRedirect className="header-logo-wrapper" to={defaultPage}>
          {!isPersonal ? (
            <img alt="logo" src={props.logoUrl} className="header-logo-icon" />
          ) : (
            <img
              alt="logo"
              className="header-logo-icon"
              src={combineUrl(
                AppServerConfig.proxyURL,
                "/static/images/personal.logo.react.svg"
              )}
            />
          )}
        </LinkWithoutRedirect>
        {isNavAvailable && isDesktopView && !isPersonal && (
          <StyledNavigationIconsWrapper>
            {mainModules.map((item) => {
              return (
                <React.Fragment key={item.id}>
                  {item.iconUrl &&
                    !item.separator &&
                    item.id !== "settings" && (
                      <HeaderNavigationIcon
                        key={item.id}
                        id={item.id}
                        data-id={item.id}
                        data-link={item.link}
                        active={item.id == currentProductId}
                        iconUrl={item.iconUrl}
                        badgeNumber={item.notifications}
                        onItemClick={onItemClick}
                        onBadgeClick={onBadgeClick}
                        url={item.link}
                      />
                    )}
                </React.Fragment>
              );
            })}
          </StyledNavigationIconsWrapper>
        )}
      </Header>

      {isNavAvailable && !isDesktopView && (
        <Nav
          opened={isNavOpened}
          onMouseEnter={onNavMouseEnter}
          onMouseLeave={onNavMouseLeave}
          numberOfModules={numberOfModules}
        >
          <NavLogoItem opened={isNavOpened} onClick={onLogoClick} />
          <NavItem
            separator={true}
            key={"nav-products-separator"}
            data-id={"nav-products-separator"}
          />
          {navItems}
          <Box className="version-box">
            <Link
              as="a"
              href={`https://github.com/ONLYOFFICE/AppServer/releases`}
              target="_blank"
              {...versionBadgeProps}
            >
              {t("Common:Version")} {version}
            </Link>
            <Text as="span" {...versionBadgeProps}>
              {" "}
              -{" "}
            </Text>
            <StyledLink>
              <LinkWithoutRedirect
                to={combineUrl(proxyURL, "/about")}
                className="nav-menu-header_link"
              >
                {t("Common:About")}
              </LinkWithoutRedirect>
            </StyledLink>
          </Box>
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
  version: PropTypes.string,
  isAuthenticated: PropTypes.bool,
  isAdmin: PropTypes.bool,
  needNavMenu: PropTypes.bool,
};

export default inject(({ auth }) => {
  const {
    settingsStore,
    moduleStore,
    isLoaded,
    isAuthenticated,
    isAdmin,
    product,
    availableModules,
    version,
  } = auth;
  const {
    logoUrl,
    defaultPage,
    currentProductId,
    personal: isPersonal,
    theme,
    toggleArticleOpen,
  } = settingsStore;
  const { totalNotifications } = moduleStore;

  //TODO: restore when chat will complete -> const mainModules = availableModules.filter((m) => !m.isolateMode);

  return {
    theme,
    isPersonal,
    isAdmin,
    defaultPage,
    logoUrl,
    mainModules: availableModules,
    totalNotifications,
    isLoaded,
    version,
    isAuthenticated,
    currentProductId,
    toggleArticleOpen,
    currentProductName: (product && product.title) || "",
  };
})(observer(HeaderComponent));
