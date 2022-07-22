import React from "react";
import PropTypes from "prop-types";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import StyledUserTooltip from "./StyledUserTooltip";

const UserTooltip = ({ avatarUrl, label, email, position, theme }) => (
  <StyledUserTooltip theme={theme}>
    <div className="block-avatar">
      <Avatar
        theme={theme}
        className="user-avatar"
        size="min"
        role="user"
        source={avatarUrl}
        userName=""
        editing={false}
      />
    </div>

    <div className="block-info">
      <Text
        theme={theme}
        isBold={true}
        fontSize="13px"
        fontWeight={600}
        truncate={true}
        title={label}
      >
        {label}
      </Text>
      <Text
        theme={theme}
        color={theme.peopleSelector.textColor}
        fontSize="13px"
        className="email-text"
        truncate={true}
        title={email}
      >
        {email}
      </Text>
      <Text
        theme={theme}
        fontSize="13px"
        fontWeight={600}
        truncate={true}
        title={position}
      >
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
