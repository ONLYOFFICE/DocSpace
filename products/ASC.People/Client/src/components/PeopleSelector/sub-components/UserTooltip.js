import React from 'react';
import PropTypes from 'prop-types';
import Avatar from '@appserver/components/avatar';
import Text from '@appserver/components/text';
import StyledUserTooltip from './StyledUserTooltip';

const UserTooltip = ({ avatarUrl, label, email, position, theme }) => (
  <StyledUserTooltip>
    <div className="block-avatar">
      <Avatar
        className="user-avatar"
        size="min"
        role="user"
        source={avatarUrl}
        userName=""
        editing={false}
      />
    </div>

    <div className="block-info">
      <Text isBold={true} fontSize="13px" fontWeight={600} truncate={true} title={label}>
        {label}
      </Text>
      <Text
        color={theme.peopleSelector.textColor}
        fontSize="13px"
        className="email-text"
        truncate={true}
        title={email}>
        {email}
      </Text>
      <Text fontSize="13px" fontWeight={600} truncate={true} title={position}>
        {position}
      </Text>
    </div>
  </StyledUserTooltip>
);

UserTooltip.propTypes = {
  avatarUrl: PropTypes.string,
  label: PropTypes.string,
  email: PropTypes.string,
  position: PropTypes.string,
};

export default UserTooltip;
