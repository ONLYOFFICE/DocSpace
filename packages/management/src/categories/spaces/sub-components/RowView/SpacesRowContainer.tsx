import React from "react";
import RowContainer from "@docspace/components/row-container";
import SpacesRoomRow from "./SpacesRoomRow";
import styled from "styled-components";

const StyledRowContainer = styled(RowContainer)`
  max-width: 620px;
  border-left: 1px solid #eceef1;
  border-right: 1px solid #eceef1;
  border-top: 1px solid #eceef1;
  border-radius: 6px;
  margin-top: 20px;
`;

export const SpacesRowContainer = ({ portals }) => {
  return (
    <StyledRowContainer useReactWindow={false}>
      {portals.map((item) => (
        <SpacesRoomRow key={item.tenantId} item={item} />
      ))}
    </StyledRowContainer>
  );
};
