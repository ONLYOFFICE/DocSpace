import React from "react";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Link as LinkWithoutRedirect } from "react-router-dom";
import { isMobileOnly } from "react-device-detect";
import NavItem from "./nav-item";
import Headline from "@appserver/common/components/Headline";
import Nav from "./nav";
import NavLogoItem from "./nav-logo-item";
import Link from "@appserver/components/link";
import history from "@appserver/common/history";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Text from "@appserver/components/text";
import { desktop, tablet } from "@appserver/components/utils/device";
import i18n from "../i18n";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import NoUserSelect from "@appserver/components/utils/commonStyles";

const { proxyURL } = AppServerConfig;

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  width: 100vw;
  height: 56px;

  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    ${NoUserSelect}
    ${(props) =>
      props.module &&
      !props.isPersonal &&
      css`
        @media ${tablet} {
          display: none;
        }
      `}
  }

  .header-module-title {
    display: block;
    font-size: 21px;
    line-height: 0;
    margin-top: -5px;
    cursor: pointer;

    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    -webkit-user-drag: none;
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;

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
    width: ${(props) => (props.isPersonal ? "220px" : "146px")};
    ${(props) => props.isPersonal && `margin-left: 20px;`}
    height: 24px;
    position: relative;
    padding: ${(props) => (!props.isPersonal ? "0 20px 0 6px" : "0")};
    cursor: pointer;

    @media ${tablet} {
      ${(props) => props.isPersonal && `margin-left: 16px;`}
    }

    @media (max-width: 620px) {
      ${(props) =>
        !props.isPersonal &&
        css`
          display: ${(props) => (props.module ? "none" : "block")};
          padding: 0px 20px 0 6px;
        `}
    }
  }
  .mobile-short-logo {
    width: 146px;
  }
`;

const StyledLink = styled.div`
  display: inline;
  .nav-menu-header_link {
    color: #7a95b0;
    font-size: 13px;
  }

  a {
    text-decoration: none;
  }
  :hover {
    color: #7a95b0;
    -webkit-text-decoration: underline;
    text-decoration: underline;
  }
`;

const versionBadgeProps = {
  color: "#7A95B0",
  fontWeight: "600",
  fontSize: "13px",
};

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
  ...props
}) => {
  const { t } = useTranslation("Common");

  const isNavAvailable = mainModules.length > 0;

  const onLogoClick = () => {
    history.push(defaultPage);
    backdropClick();
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
    history.push(link);
    backdropClick();
    e.preventDefault();
  };

  const numberOfModules = mainModules.filter((item) => !item.separator).length;

  return (
    <>
      <Header
        module={currentProductName}
        isLoaded={isLoaded}
        isPersonal={isPersonal}
        isAuthenticated={isAuthenticated}
        className="navMenuHeader hidingHeader"
      >
        {!isPersonal && (
          <NavItem
            badgeNumber={totalNotifications}
            onClick={onClick}
            noHover={true}
          />
        )}

        <LinkWithoutRedirect className="header-logo-wrapper" to={defaultPage}>
          {!isPersonal ? (
            <img alt="logo" src={props.logoUrl} className="header-logo-icon" />
          ) : !isMobileOnly ? (
            <img
              alt="logo"
              className="header-logo-icon"
              src={combineUrl(
                AppServerConfig.proxyURL,
                "/static/images/personal.logo.react.svg"
              )}
            />
          ) : (
            <img
              className="header-logo-icon mobile-short-logo"
              src={combineUrl(
                AppServerConfig.proxyURL,
                "/static/images/nav.logo.opened.react.svg"
              )}
            />
          )}
        </LinkWithoutRedirect>

        {!isPersonal && (
          <Headline
            className="header-module-title"
            type="header"
            color="#FFF"
            onClick={onClick}
          >
            {currentProductName}
          </Headline>
        )}
      </Header>

      {isNavAvailable && (
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
          {mainModules.map(
            ({
              id,
              separator, //iconName,
              iconUrl,
              notifications,
              link,
              title,
              dashed,
            }) => (
              <NavItem
                separator={!!separator}
                key={id}
                data-id={id}
                data-link={link}
                opened={isNavOpened}
                active={id == currentProductId}
                //iconName={iconName}
                iconUrl={iconUrl}
                badgeNumber={notifications}
                onClick={onItemClick}
                onBadgeClick={onBadgeClick}
                url={link}
                dashed={dashed}
              >
                {id === "settings" ? i18n.t("Common:Settings") : title}
              </NavItem>
            )
          )}
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
  } = settingsStore;
  const { totalNotifications } = moduleStore;

  //TODO: restore when chat will complete -> const mainModules = availableModules.filter((m) => !m.isolateMode);

  return {
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
    currentProductName: (product && product.title) || "",
  };
})(observer(HeaderComponent));
