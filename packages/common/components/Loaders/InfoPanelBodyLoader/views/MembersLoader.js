import React from "react";
import styled from "styled-components";

import RectangleLoader from "../../RectangleLoader/RectangleLoader";

const StyledMembersLoader = styled.div`
  width: 100%;

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: start;
`;

const StyledSubtitleLoader = styled.div`
  width: 100%;
  padding: 8px 0 12px 0;
  .pending_users {
    padding-top: 20px;
  }

  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
`;

const StyledMemberLoader = styled.div`
  width: 100%;
  height: 48px;

  display: flex;
  flex-direction: row;
  justify-content: start;
  align-items: center;
  gap: 8px;

  .avatar {
    min-height: 32px;
    min-width: 32px;
  }

  .role-selector {
    margin-left: auto;
  }
`;

const MembersLoader = ({ data }) => {
  return (
    <StyledMembersLoader>
      <StyledSubtitleLoader>
        <RectangleLoader width={"111px"} height={"16px"} borderRadius={"3px"} />
        <RectangleLoader width={"16px"} height={"16px"} borderRadius={"3px"} />
      </StyledSubtitleLoader>

      {[...Array(data.membersCount).keys()].map((i) => (
        <StyledMemberLoader key={i}>
          <RectangleLoader
            className="avatar"
            width={"32px"}
            height={"32px"}
            borderRadius={"50%"}
          />
          <RectangleLoader
            width={"212px"}
            height={"16px"}
            borderRadius={"3px"}
          />
          <RectangleLoader
            className="role-selector"
            width={"64px"}
            height={"20px"}
            borderRadius={"3px"}
          />
        </StyledMemberLoader>
      ))}

      {!!data.pendingMembersCount && (
        <StyledSubtitleLoader className="pending_users">
          <RectangleLoader
            width={"111px"}
            height={"16px"}
            borderRadius={"3px"}
          />
          <RectangleLoader
            width={"16px"}
            height={"16px"}
            borderRadius={"3px"}
          />
        </StyledSubtitleLoader>
      )}

      {[...Array(data.pendingMembersCount).keys()].map((i) => (
        <StyledMemberLoader key={i}>
          <RectangleLoader
            className="avatar"
            width={"32px"}
            height={"32px"}
            borderRadius={"50%"}
          />
          <RectangleLoader
            width={"212px"}
            height={"16px"}
            borderRadius={"3px"}
          />
          <RectangleLoader
            className="role-selector"
            width={"64px"}
            height={"20px"}
            borderRadius={"3px"}
          />
        </StyledMemberLoader>
      ))}
    </StyledMembersLoader>
  );
};

export default MembersLoader;
