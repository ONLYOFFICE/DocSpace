import React from "react";
import styled from "styled-components";

import Avatar from "../../avatar";
import Text from "../../text";

import { hugeMobile } from "../../utils/device";

const StyledWrapper = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 24px;

  @media ${hugeMobile} {
    display: none;
  }

  .avatar {
    height: 124px;
    width: 124px;
    align-self: center;
  }

  .name-preview {
    max-width: 198px;
    display: flex;
    gap: 8px;
    align-items: center;
    padding: 8px;
    border: 1px solid #eceef1;
    border-radius: 6px;
  }
`;

const AvatarPreview = ({ avatar, userName }) => {
  return (
    <StyledWrapper>
      <Avatar
        className="avatar"
        size="max"
        role="user"
        source={avatar}
        userName={userName}
        editing={false}
      />
      <div className="name-preview">
        <Avatar
          size="min"
          role="user"
          source={avatar}
          userName={userName}
          editing={false}
        />
        <Text fontWeight={600} fontSize="15px" truncate={true}>
          {userName}
        </Text>
      </div>
    </StyledWrapper>
  );
};

export default AvatarPreview;
