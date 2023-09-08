import React, { memo } from "react";
import PropTypes from "prop-types";
import styled, { useTheme } from "styled-components";
import PencilReactSvgUrl from "PUBLIC_DIR/images/pencil.react.svg?url";

import { GuestReactSvg, AdministratorReactSvg, OwnerReactSvg } from "./svg";
import {
  EmptyIcon,
  EditContainer,
  AvatarWrapper,
  RoleWrapper,
  NamedAvatar,
  StyledImage,
  StyledAvatar,
  StyledIconWrapper,
} from "./styled-avatar";
import IconButton from "../icon-button";
import commonIconsStyles from "../utils/common-icons-style";

import Text from "../text";
import Tooltip from "../tooltip";

const StyledGuestIcon = styled(GuestReactSvg)`
  ${commonIconsStyles}
`;
const StyledAdministratorIcon = styled(AdministratorReactSvg)`
  ${commonIconsStyles}
`;
const StyledOwnerIcon = styled(OwnerReactSvg)`
  ${commonIconsStyles}
`;
const getRoleIcon = (role) => {
  switch (role) {
    case "admin":
      return <StyledAdministratorIcon size="scale" className="admin_icon" />;
    case "owner":
      return <StyledOwnerIcon size="scale" className="owner_icon" />;
    default:
      return null;
  }
};

const getInitials = (userName) =>
  userName
    .split(/\s/)
    .reduce((response, word) => (response += word.slice(0, 1)), "")
    .substring(0, 2);

const Initials = (props) => (
  <NamedAvatar {...props}>{getInitials(props.userName)}</NamedAvatar>
);

Initials.propTypes = {
  userName: PropTypes.string,
};

// eslint-disable-next-line react/display-name
const Avatar = (props) => {
  //console.log("Avatar render");
  const {
    size,
    source,
    userName,
    role,
    editing,
    editAction,
    isDefaultSource = false,
    hideRoleIcon,
    tooltipContent,
    withTooltip,
  } = props;

  const { interfaceDirection } = useTheme();

  let isDefault = false,
    isIcon = false;

  if (source?.includes("default_user_photo")) isDefault = true;
  else if (source?.includes(".svg")) isIcon = true;

  const avatarContent = source ? (
    isIcon ? (
      <StyledIconWrapper>
        <IconButton iconName={source} className="icon" isDisabled={true} />
      </StyledIconWrapper>
    ) : (
      <StyledImage src={source} isDefault={isDefault} />
    )
  ) : userName ? (
    <Initials userName={userName} size={size} />
  ) : isDefaultSource ? (
    <StyledImage isDefault />
  ) : (
    <EmptyIcon size="scale" />
  );

  const roleIcon = getRoleIcon(role);

  const uniqueTooltipId = withTooltip ? `roleTooltip_${Math.random()}` : "";
  const tooltipPlace = interfaceDirection === "rtl" ? "left" : "right";

  return (
    <StyledAvatar {...props}>
      <AvatarWrapper source={source} userName={userName}>
        {avatarContent}
      </AvatarWrapper>
      {editing && size === "max" ? (
        <EditContainer>
          <IconButton
            className="edit_icon"
            iconName={PencilReactSvgUrl}
            onClick={editAction}
            size={16}
          />
        </EditContainer>
      ) : (
        <>
          {!hideRoleIcon && (
            <>
              <RoleWrapper
                size={size}
                data-tooltip-id={uniqueTooltipId}
                data-tooltip-content={tooltipContent}
                className="avatar_role-wrapper"
              >
                {props.roleIcon ? props.roleIcon : roleIcon}
              </RoleWrapper>
              {withTooltip && (
                <Tooltip
                  float
                  id={uniqueTooltipId}
                  getContent={({ content }) => (
                    <Text fontSize="12px">{content}</Text>
                  )}
                  place={tooltipPlace}
                />
              )}
            </>
          )}
        </>
      )}
    </StyledAvatar>
  );
};

Avatar.propTypes = {
  /** Size of avatar */
  size: PropTypes.oneOf(["max", "big", "medium", "base", "small", "min"]),
  /** Adds a table of user roles */
  role: PropTypes.oneOf([
    "owner",
    "admin",
    "guest",
    "user",
    "manager",
    "collaborator",
    "",
  ]),
  /** Displays as `Picture` in case the url is specified and as `Icon` in case the path to the .svg file is specified */
  source: PropTypes.string,
  /** Allows to display a user name as initials when `source` is set to blank */
  userName: PropTypes.string,
  /** Enables avatar editing */
  editing: PropTypes.bool,
  /** Allows to display as a default icon when `source` is set to blank */
  isDefaultSource: PropTypes.bool,
  /** Function called when the avatar change button is pressed */
  editAction: PropTypes.func,
  /** Hides user role */
  hideRoleIcon: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Show tooltip on hover role icon */
  withTooltip: PropTypes.bool,
  /** Tooltip content */
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
};

Avatar.defaultProps = {
  size: "medium",
  role: "",
  source: "",
  userName: "",
  editing: false,
  hideRoleIcon: false,
  withTooltip: false,
  tooltipContent: "",
};

export default memo(Avatar);
