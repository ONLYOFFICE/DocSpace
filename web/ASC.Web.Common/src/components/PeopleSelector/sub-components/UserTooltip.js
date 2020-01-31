import React from 'react';
import PropTypes from "prop-types";
import { Avatar, Text } from "asc-web-components";
import StyledUserTooltip from "./StyledUserTooltip";

const UserTooltip = ({ avatarUrl, label, email, position }) => (

  <StyledUserTooltip>
    <div className='block-avatar-name'>
      <Avatar
        className='user-avatar'
        size="small"
        role="user"
        source={avatarUrl}
        userName=""
        editing={false}
      />
      <Text isBold={true} fontSize="13px" fontWeight={700}>
        {label}
      </Text>
    </div>
    <div>

      <Text color="#A3A9AE" fontSize="13px" className="email-text">
        {email}
      </Text>
      <Text fontSize="13px" fontWeight={600}>
        {position}
      </Text>
    </div>
  </StyledUserTooltip>
);

UserTooltip.propTypes = {
  avatarUrl: PropTypes.string,
  label: PropTypes.string,
  email: PropTypes.string,
  position: PropTypes.string
}

export default UserTooltip;