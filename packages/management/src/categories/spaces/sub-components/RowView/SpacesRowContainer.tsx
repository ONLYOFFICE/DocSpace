import React from "react";
import RowContainer from "@docspace/components/row-container";
import SpacesRoomRow from "./SpacesRoomRow";
import styled from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledRowContainer = styled(RowContainer)`
  max-width: 620px;
  border-width: 1px;
  border-style: solid;
  border-color: ${(props) => props.theme.rowContainer.borderColor};
  border-bottom: none;
  border-radius: 6px;
  margin-top: 20px;
`;

StyledRowContainer.defaultProps = { theme: Base };
export const SpacesRowContainer = ({ portals }) => {
  return (
    <StyledRowContainer useReactWindow={false}>
      {portals.map((item) => (
        <SpacesRoomRow key={item.tenantId} item={item} />
      ))}
    </StyledRowContainer>
  );
};
