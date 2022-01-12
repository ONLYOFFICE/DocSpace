import React from "react";
import { ReactSVG } from "react-svg";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

import Badge from "@appserver/components/badge";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { tablet } from "@appserver/components/utils/device";
import MenuIcon from "../../../../../../public/images/menu.react.svg";

const baseColor = "#7A95B0",
  activeColor = "#FFFFFF",
  separatorColor = "#3E668D";

const NavItemSeparator = styled.div`
  border-bottom: 1px ${(props) => (props.dashed ? "dashed" : "solid")}
    ${separatorColor};
  margin: 0 16px;
`;

const NavItemWrapper = styled(Link)`
  display: flex;
  min-width: 48px;
  min-height: 50px;
  align-items: center;
  padding: 0 16px 0 20px;
  cursor: pointer;
  position: relative;
  box-sizing: border-box;

  ${(props) =>
    !props.noHover &&
    css`
      &:hover {
        background: #0d3760;
        text-decoration: none;
      }
    `}

  .injected-svg {
    margin-top: 3px;
    path {
      fill: ${(props) => props.iconColor};
    }
  }

  @media ${tablet} {
    padding: 0 16px 0 16px;
  }
`;

const NavItemLabel = styled(Text)`
  margin: 0 auto 0 16px;
  display: ${(props) => (props.opened ? "block" : "none")};
`;

const badgeCss = css`
  position: absolute;
  top: 2px;
  right: 4px;
  overflow: inherit;
`;

const NavItemBadge = styled(Badge)`
  ${(props) => (props.opened ? "" : badgeCss)}
`;

const VersionBadge = styled.div`
  background-color: #3cb55b;
  border-radius: 5px;
  color: #ffffff;
  display: inline-block;
  font-size: 10px;
  line-height: 8px;
  padding: 3px 6px;
  position: absolute;
  top: -5px;
  left: 10px;
`;

const StyledMenuIcon = styled(MenuIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;
const NavItem = React.memo((props) => {
  //console.log("NavItem render");
  const {
    separator,
    opened,
    active,
    iconName,
    iconUrl,
    children,
    badgeNumber,
    onClick,
    onBadgeClick,
    url,
    noHover,
    ...rest
  } = props;
  const color = active ? activeColor : baseColor;

  return separator ? (
    <NavItemSeparator {...rest} />
  ) : (
    <NavItemWrapper
      noHover={noHover}
      href={url}
      onClick={onClick}
      iconColor={color}
      {...rest}
    >
      {iconUrl ? (
        <ReactSVG
          src={iconUrl}
          beforeInjection={(svg) => {
            svg.setAttribute("fill", color);
          }}
        />
      ) : (
        <>
          {iconName === "MenuIcon" && <VersionBadge>BETA</VersionBadge>}
          <StyledMenuIcon color={color} size="big" />
        </>
      )}
      {children && (
        <NavItemLabel
          opened={opened}
          color={color}
          fontSize="16px"
          fontWeight="bold"
          truncate
        >
          {children}
        </NavItemLabel>
      )}
      <NavItemBadge
        opened={opened}
        label={badgeNumber}
        onClick={onBadgeClick}
      />
    </NavItemWrapper>
  );
});

NavItem.displayName = "NavItem";

NavItem.propTypes = {
  active: PropTypes.bool,
  badgeNumber: PropTypes.number,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  url: PropTypes.string,
  iconName: PropTypes.string,
  iconUrl: PropTypes.string,
  onBadgeClick: PropTypes.func,
  onClick: PropTypes.func,
  opened: PropTypes.bool,
  separator: PropTypes.bool,
  noHover: PropTypes.bool,
  dashed: PropTypes.bool,
};

export default NavItem;
