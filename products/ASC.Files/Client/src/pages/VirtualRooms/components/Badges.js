import React from "react";
import styled, { css } from "styled-components";
import { ReactSVG } from "react-svg";

import Badge from "@appserver/components/badge";

const StyledBadgesContainer = styled.div`
  display: flex;
  align-items: center;

  .badge {
    p {
      line-height: 16px;
    }
  }

  .pin-icon {
    height: 16px;

    margin: 0 0 0 12px;
  }
`;

const RoomsBadges = ({ badge, onBadgeClick, pinned, onClickPinRoom }) => {
  return (
    <StyledBadgesContainer>
      {!!badge && (
        <Badge className="badge" label={badge} onClick={onBadgeClick} />
      )}

      {pinned && (
        <ReactSVG
          className="pin-icon"
          onClick={onClickPinRoom}
          src="images/unpin.react.svg"
        />
      )}
    </StyledBadgesContainer>
  );
};

export default RoomsBadges;
