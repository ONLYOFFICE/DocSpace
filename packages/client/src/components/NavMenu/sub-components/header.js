import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Link as LinkWithoutRedirect } from "react-router-dom";
import { isMobileOnly, isMobile } from "react-device-detect";
import NavItem from "./nav-item";
import Headline from "@docspace/common/components/Headline";
import Nav from "./nav";
import NavLogoItem from "./nav-logo-item";
import Link from "@docspace/components/link";
import history from "@docspace/common/history";
import { useTranslation } from "react-i18next";
import HeaderNavigationIcon from "./header-navigation-icon";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import {
  desktop,
  isDesktop,
  tablet,
  mobile,
} from "@docspace/components/utils/device";
import i18n from "../i18n";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import { getLink, checkIfModuleOld, onItemClick } from "SRC_DIR/helpers/utils";
import StyledExternalLinkIcon from "@docspace/client/src/components/StyledExternalLinkIcon";
import HeaderCatalogBurger from "./header-catalog-burger";
import { Base } from "@docspace/components/themes";
import { ReactSVG } from "react-svg";

const { proxyURL } = AppServerConfig;

const Header = styled.header`
  display: flex;
  align-items: center;

  background-color: ${(props) => props.theme.header.backgroundColor};

  width: 100vw;
  height: 48px;

  .header-logo-wrapper {
    height: 24px;

    display: flex;
    align-items: center;
    justify-items: center;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    ${NoUserSelect}
  }

  .header-logo-icon {
    position: absolute;
    right: 50%;
    transform: translateX(50%);
    height: 24px;
    cursor: pointer;

    svg {
      path:last-child {
        fill: ${(props) => props.theme.client.home.logoColor};
      }
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
  //totalNotifications,
  onClick,
  onNavMouseEnter,
  onNavMouseLeave,
  defaultPage,
  //mainModules,
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
  //const isNavAvailable = mainModules.length > 0;

  // const onLogoClick = () => {
  //   history.push(defaultPage);
  //   backdropClick();
  // };

  // const onBadgeClick = React.useCallback((e) => {
  //   if (!e) return;
  //   const id = e.currentTarget.dataset.id;
  //   const item = mainModules.find((m) => m.id === id);
  //   toggleAside();

  //   if (item) item.onBadgeClick(e);
  // }, []);

  // const handleItemClick = React.useCallback((e) => {
  //   onItemClick(e);
  //   backdropClick();
  // }, []);

  //const numberOfModules = mainModules.filter((item) => !item.separator).length;
  //const needNavMenu = currentProductId !== "home";
  // const mainModulesWithoutSettings = mainModules.filter(
  //   (module) => module.id !== "settings"
  // );

  /*const navItems = mainModulesWithoutSettings.map(
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
  );*/

  const [isDesktopView, setIsDesktopView] = useState(isDesktop());

  const onResize = () => {
    const isDesktopView = isDesktop();
    if (isDesktopView === isDesktopView) setIsDesktopView(isDesktopView);
  };

  useEffect(() => {
    window.addEventListener("resize", onResize);
    return () => window.removeEventListener("resize", onResize);
  });

  const [isFormGallery, setIsFormGallery] = useState(
    history.location.pathname.includes("/form-gallery")
  );
  useEffect(() => {
    return history.listen((location) => {
      setIsFormGallery(location.pathname.includes("/form-gallery"));
    });
  }, [history]);

  return (
    <>
      <Header
        module={currentProductName}
        isLoaded={isLoaded}
        isPersonal={isPersonal}
        isPreparationPortal={isPreparationPortal}
        isAuthenticated={isAuthenticated}
        className="navMenuHeader hidingHeader"
        needNavMenu={false}
        isDesktopView={isDesktopView}
      >
        {((isPersonal && location.pathname.includes("files")) ||
          (!isPersonal && currentProductId !== "home")) &&
          !isFormGallery && <HeaderCatalogBurger onClick={toggleArticleOpen} />}
        <LinkWithoutRedirect className="header-logo-wrapper" to={defaultPage}>
          {!isPersonal ? (
            props.logoUrl.includes(".svg") ? (
              <ReactSVG src={props.logoUrl} className="header-logo-icon" />
            ) : (
              <img
                alt="logo"
                src={props.logoUrl}
                className="header-logo-icon"
              />
            )
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
        {/* {isNavAvailable &&
          isDesktopView &&
          !isPersonal &&
          currentProductId !== "home" && (
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
          )} */}
      </Header>

      {/* {!isDesktopView && (
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
      )} */}
    </>
  );
};

HeaderComponent.displayName = "Header";

HeaderComponent.propTypes = {
  //totalNotifications: PropTypes.number,
  onClick: PropTypes.func,

  defaultPage: PropTypes.string,

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

    isLoaded,
    isAuthenticated,
    isAdmin,

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

  //TODO: restore when chat will complete -> const mainModules = availableModules.filter((m) => !m.isolateMode);

  return {
    theme,
    isPersonal,
    isAdmin,
    defaultPage,
    logoUrl,

    //totalNotifications,
    isLoaded,
    version,
    isAuthenticated,
    currentProductId,
    toggleArticleOpen,
    //currentProductName: (product && product.title) || "",
  };
})(observer(HeaderComponent));
