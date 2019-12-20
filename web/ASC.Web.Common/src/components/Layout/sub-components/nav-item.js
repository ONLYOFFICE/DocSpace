import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Badge, Icons, Text} from "asc-web-components";

const baseColor = "#7A95B0",
  activeColor = "#FFFFFF";

const NavItemSeparator = styled.div`
  border-bottom: 1px solid ${baseColor};
  margin: 0 16px;
`;

const NavItemWrapper = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;
  position: relative;
  box-sizing: border-box;
`;

const NavItemLabel = styled.div`
  margin: 0 auto 0 16px;
  overflow: hidden;
  text-overflow: ellipsis;
  color: ${props => props.color};
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
    onBadgeClick
  } = props;
  const color = active ? activeColor : baseColor;

  return separator ? (
    <NavItemSeparator />
  ) : (
    <NavItemWrapper onClick={onClick}>
      {React.createElement(Icons[iconName], {
        size: "big",
        isfill: true,
        color: color
      })}
      {children && (
        <NavItemLabel opened={opened}>
          <Text color={color} isBold fontSize='16px'>
            {children}
          </Text>
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
  separator: PropTypes.bool,
  opened: PropTypes.bool,
  active: PropTypes.bool,
  iconName: PropTypes.string,
  badgeNumber: PropTypes.number,
  onClick: PropTypes.func,
  onBadgeClick: PropTypes.func,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ])
};

export default NavItem;
