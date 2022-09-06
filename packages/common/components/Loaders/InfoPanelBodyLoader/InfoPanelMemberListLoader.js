import React from "react";
import styled from "styled-components";

import RectangleLoader from "../RectangleLoader";

const StyledInfoPanelMemberListLoader = styled.div`
  width: 100%;

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: start;
`;

const StyledSubtitleLoader = styled.div`
  width: 100%;
  padding: 8px 0 16px 0;

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
`;

const InfoPanelMemberListLoader = () => {
  return (
    <StyledInfoPanelMemberListLoader>
      <StyledSubtitleLoader>
        <RectangleLoader width={"100px"} height={"20px"} borderRadius={"3px"} />
        <RectangleLoader width={"16px"} height={"16px"} borderRadius={"3px"} />
      </StyledSubtitleLoader>

      {[...Array(4).keys()].map((i) => (
        <StyledMemberLoader key={i}>
          <RectangleLoader
            className="avatar"
            width={"32px"}
            height={"32px"}
            borderRadius={"50%"}
          />
          <RectangleLoader
            width={"100%"}
            height={"20px"}
            borderRadius={"3px"}
          />
        </StyledMemberLoader>
      ))}

      <StyledSubtitleLoader>
        <RectangleLoader width={"100px"} height={"20px"} borderRadius={"3px"} />
        <RectangleLoader width={"16px"} height={"16px"} borderRadius={"3px"} />
      </StyledSubtitleLoader>

      {[...Array(2).keys()].map((i) => (
        <StyledMemberLoader key={i}>
          <RectangleLoader
            className="avatar"
            width={"32px"}
            height={"32px"}
            borderRadius={"50%"}
          />
          <RectangleLoader
            width={"100%"}
            height={"20px"}
            borderRadius={"3px"}
          />
        </StyledMemberLoader>
      ))}
    </StyledInfoPanelMemberListLoader>
  );
};

export default InfoPanelMemberListLoader;
