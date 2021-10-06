import React from 'react';
import { inject, observer } from 'mobx-react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import { Link as LinkWithoutRedirect } from 'react-router-dom';
import NavItem from './nav-item';
import Nav from './nav';
import NavLogoItem from './nav-logo-item';
import Link from '@appserver/components/link';
import history from '@appserver/common/history';
import { useTranslation } from 'react-i18next';

import Box from '@appserver/components/box';
import Text from '@appserver/components/text';
import { desktop, mobile, tablet } from '@appserver/components/utils/device';
import { isMobileOnly, isMobile } from 'react-device-detect';
import i18n from '../i18n';
import { combineUrl } from '@appserver/common/utils';
import { AppServerConfig } from '@appserver/common/constants';
import NoUserSelect from '@appserver/components/utils/commonStyles';
import HeaderNavigationIcon from './header-navigation-icon';

const { proxyURL } = AppServerConfig;

const backgroundColor = '#0F4071';

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  width: 100vw;
  height: 48px;

  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    ${NoUserSelect}
  }

  .header-logo-icon {
    height: 24px;
    position: relative;
    margin-left: 20px;
    cursor: pointer;

    @media ${tablet} {
      margin-left: 16px;
    }
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
  color: '#7A95B0',
  fontWeight: '600',
  fontSize: '13px',
};

const StyledNavigationIconsWrapper = styled.div`
  height: 20px;
  position: absolute;
  left: ${isMobile ? '254px' : '280px'};
  display: ${isMobileOnly ? 'none' : 'flex'};
  justify-content: flex-start;

  @media ${tablet} {
    left: 254px;
  }

  @media ${mobile} {
    display: none;
  }

  .header-navigation__icon {
    cursor: pointer;
    margin-right: 22px;
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
  ...props
}) => {
  const { t } = useTranslation('Common');

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
        className="navMenuHeader hidingHeader">
        <LinkWithoutRedirect className="header-logo-wrapper" to={defaultPage}>
          {!isPersonal ? (
            <img alt="logo" src={props.logoUrl} className="header-logo-icon" />
          ) : (
            <img
              alt="logo"
              className="header-logo-icon"
              src={combineUrl(AppServerConfig.proxyURL, '/static/images/personal.logo.react.svg')}
            />
          )}
        </LinkWithoutRedirect>

        {!isPersonal && (
          <StyledNavigationIconsWrapper>
            {mainModules.map(
              ({ id, iconUrl, notifications, link, separator }) =>
                iconUrl &&
                !separator && (
                  <HeaderNavigationIcon
                    key={id}
                    id={id}
                    data-id={id}
                    data-link={link}
                    active={id == currentProductId}
                    iconUrl={iconUrl}
                    badgeNumber={notifications}
                    onClick={onItemClick}
                    onBadgeClick={onBadgeClick}
                    url={link}
                  />
                ),
            )}
          </StyledNavigationIconsWrapper>
        )}
      </Header>

      {isNavAvailable && (
        <Nav
          opened={isNavOpened}
          onMouseEnter={onNavMouseEnter}
          onMouseLeave={onNavMouseLeave}
          numberOfModules={numberOfModules}>
          <NavLogoItem opened={isNavOpened} onClick={onLogoClick} />
          <NavItem
            separator={true}
            key={'nav-products-separator'}
            data-id={'nav-products-separator'}
          />
          {mainModules.map((
            { id, separator, iconUrl, notifications, link, title, dashed }, //iconName,
          ) => (
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
              dashed={dashed}>
              {id === 'settings' ? i18n.t('Common:Settings') : title}
            </NavItem>
          ))}
          <Box className="version-box">
            <Link
              as="a"
              href={`https://github.com/ONLYOFFICE/AppServer/releases`}
              target="_blank"
              {...versionBadgeProps}>
              {t('Common:Version')} {version}
            </Link>
            <Text as="span" {...versionBadgeProps}>
              {' '}
              -{' '}
            </Text>
            <StyledLink>
              <LinkWithoutRedirect
                to={combineUrl(proxyURL, '/about')}
                className="nav-menu-header_link">
                {t('Common:About')}
              </LinkWithoutRedirect>
            </StyledLink>
          </Box>
        </Nav>
      )}
    </>
  );
};

HeaderComponent.displayName = 'Header';

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
  const { logoUrl, defaultPage, currentProductId, personal: isPersonal } = settingsStore;
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
    currentProductName: (product && product.title) || '',
  };
})(observer(HeaderComponent));
