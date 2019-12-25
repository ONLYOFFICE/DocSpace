import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Badge, Icons, Link, Text } from "asc-web-components";

const baseColor = "#7A95B0",
  activeColor = "#FFFFFF",
  separatorColor = "#3E668D";

const NavItemSeparator = styled.div`
  border-bottom: 1px solid ${separatorColor};
  margin: 0 16px;
`;

const NavItemWrapper = styled(Link)`
  display: flex;
  min-width: 56px;
  min-height: 50px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;
  position: relative;
  box-sizing: border-box;

  &:hover {
    background: #0D3760;
  }
`;

const NavItemLabel = styled(Text)`
  margin: 0 auto 0 16px;
  display: ${props => (props.opened ? "block" : "none")};
`;

const badgeCss = css`
  position: absolute;
  top: 10px;
  right: 10px;
`;

const NavItemBadge = styled(Badge)`
  ${props => (props.opened ? "" : badgeCss)}
`;

const NavItem = React.memo(props => {
  //console.log("NavItem render");
  const {
    separator,
    opened,
    active,
    iconName,
    children,
    badgeNumber,
    onClick,
    onBadgeClick,
    url
  } = props;
  const color = active ? activeColor : baseColor;

  return separator ? (
    <NavItemSeparator />
  ) : (
      <NavItemWrapper 
      href={url} 
      onClick={onClick}>
        {React.createElement(Icons[iconName], {
          size: "big",
          isfill: true,
          color: color
        })}
        {children && (
          <NavItemLabel opened={opened} color={color} fontSize="16px" fontWeight="bold" truncate>
            {children}
          </NavItemLabel>
        )}
        <NavItemBadge
          opened={opened}
          number={badgeNumber}
          onClick={onBadgeClick}
        />
      </NavItemWrapper>
    );
});

NavItem.displayName = "NavItem";

NavItem.propTypes = {
  active: PropTypes.bool,
  badgeNumber: PropTypes.number,
  children: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  url: PropTypes.string,
  iconName: PropTypes.string,
  onBadgeClick: PropTypes.func,
  onClick: PropTypes.func,
  opened: PropTypes.bool,
  separator: PropTypes.bool,
};

export default NavItem;
