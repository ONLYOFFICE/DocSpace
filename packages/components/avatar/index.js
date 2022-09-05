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
  StyledIconWrapper,
} from "./styled-avatar";
import IconButton from "../icon-button";
import commonIconsStyles from "../utils/common-icons-style";

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
      return <StyledGuestIcon size="scale" className="guest_icon" />;
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
  const { size, source, userName, role, editing, editAction } = props;
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
            className="edit_icon"
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
  size: PropTypes.oneOf(["max", "big", "medium", "base", "small", "min"]),
  /** Adds a user role table */
  role: PropTypes.oneOf(["owner", "admin", "guest", "user", ""]),
  /** Provide either a url to display as `Picture` or path to **.svg** file to display as `Icon` */
  source: PropTypes.string,
  /** Provide this and leave `source` empty to display as initials */
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
