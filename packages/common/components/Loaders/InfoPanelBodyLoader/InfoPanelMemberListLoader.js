import React from "react";
import styled from "styled-components";

import RectangleLoader from "../RectangleLoader";

const StyledInfoPanelMemberListLoader = styled.div`
  display: flex;
  flex-direction: column;

  justify-content: center;
  align-items: start;
`;

const StyledInfoPanelMemberLoader = styled.div`
  height: 48px;

  display: flex;
  flex-direction: row;
  justify-content: start;
  align-items: center;
  gap: 8px;
`;

const InfoPanelMemberListLoader = () => {
  return (
    <StyledInfoPanelMemberListLoader>
      {[...Array(5).keys()].map((i) => (
        <StyledInfoPanelMemberLoader key={i}>
          <RectangleLoader
            width={"32px"}
            height={"32px"}
            borderRadius={"50%"}
          />
          <RectangleLoader
            width={"128px"}
            height={"16px"}
            borderRadius={"3px"}
          />
        </StyledInfoPanelMemberLoader>
      ))}
    </StyledInfoPanelMemberListLoader>
  );
};

export default InfoPanelMemberListLoader;
