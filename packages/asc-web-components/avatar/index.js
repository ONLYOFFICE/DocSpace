import React, { memo } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import { GuestIcon, AdministratorIcon, OwnerIcon } from "./svg";
import {
  EmptyIcon,
  EditContainer,
  AvatarWrapper,
  RoleWrapper,
  NamedAvatar,
  StyledImage,
  StyledAvatar,
} from "./styled-avatar";
import IconButton from "../icon-button";
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
  const { size, source, userName, role, editing, editAction } = props;

  const avatarContent = source ? (
    <StyledImage src={source} />
  ) : userName ? (
    <Initials userName={userName} size={size} />
  ) : (
    <EmptyIcon size="scale" />
  );

  const roleIcon = getRoleIcon(role);

  return (
    <StyledAvatar {...props}>
      <AvatarWrapper source={source} userName={userName}>
        {avatarContent}
      </AvatarWrapper>
      {editing && size === "max" && (
        <EditContainer>
          <IconButton
            color={whiteColor}
            iconName="/static/images/pencil.react.svg"
            onClick={editAction}
            size={16}
          />
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
  userName: "",
  editing: false,
};

export default memo(Avatar);
