import React, { memo } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { GuestIcon, AdministratorIcon, OwnerIcon } from "./svg";
import {
  EditLink,
  EmptyIcon,
  EditContainer,
  AvatarWrapper,
  RoleWrapper,
  NamedAvatar,
  StyledImage,
  StyledAvatar,
} from "./styled-avatar";
import Link from "../link";
import commonIconsStyles from "../utils/common-icons-style";

const whiteColor = "#FFFFFF";

const StyledGuestIcon = styled(GuestIcon)`
  ${commonIconsStyles}
`;
const StyledAdministratorIcon = styled(AdministratorIcon)`
  ${commonIconsStyles}
`;
const StyledOwnerIcon = styled(OwnerIcon)`
  ${commonIconsStyles}
`;
const getRoleIcon = (role) => {
  switch (role) {
    case "guest":
      return <StyledGuestIcon size="scale" />;
    case "admin":
      return <StyledAdministratorIcon size="scale" />;
    case "owner":
      return <StyledOwnerIcon size="scale" />;
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
    editLabel,
    editAction,
  } = props;

  const onMouseDown = (e) => {
    e.preventDefault();
  };

  const avatarContent = source ? (
    <StyledImage src={source} onMouseDown={onMouseDown} />
  ) : userName ? (
    <Initials userName={userName} size={size} />
  ) : (
    <EmptyIcon size="scale" onMouseDown={onMouseDown} />
  );

  const roleIcon = getRoleIcon(role);

  return (
    <StyledAvatar {...props}>
      <AvatarWrapper source={source} userName={userName}>
        {avatarContent}
      </AvatarWrapper>
      {editing && size === "max" && (
        <EditContainer gradient={!!source}>
          <EditLink>
            <Link
              type="action"
              title={editLabel}
              isTextOverflow={true}
              isHovered={true}
              fontSize="14px"
              fontWeight={600}
              color={whiteColor}
              onClick={editAction}
            >
              {editLabel}
            </Link>
          </EditLink>
        </EditContainer>
      )}
      <RoleWrapper size={size}>{roleIcon}</RoleWrapper>
    </StyledAvatar>
  );
};

Avatar.propTypes = {
  /** Size of avatar */
  size: PropTypes.oneOf(["max", "big", "medium", "small", "min"]),
  /** Adds a user role table */
  role: PropTypes.oneOf(["owner", "admin", "guest", "user"]),
  /** The address of the image for an image avatar */
  source: PropTypes.string,
  /** Displays avatar edit layer */
  editLabel: PropTypes.string,
  userName: PropTypes.string,
  editing: PropTypes.bool,
  /** Function called when the avatar change button is pressed */
  editAction: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

Avatar.defaultProps = {
  size: "medium",
  role: "",
  source: "",
  editLabel: "Edit photo",
  userName: "",
  editing: false,
};

export default memo(Avatar);
