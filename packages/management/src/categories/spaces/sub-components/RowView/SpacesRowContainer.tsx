import React from "react";
import RowContainer from "@docspace/components/row-container";
import SpacesRoomRow from "./SpacesRoomRow";
import styled from "styled-components";
import Base from "@docspace/components/themes/base";
import { TPortals } from "SRC_DIR/types/spaces";

const StyledRowContainer = styled(RowContainer)`
  max-width: 620px;
  border-width: 1px;
  border-style: solid;
  border-color: ${(props) => props.theme.rowContainer.borderColor};
  border-bottom: none;
  border-radius: 6px;
  margin-top: 20px;
`;

type TRowContainer = {
  portals: TPortals[];
};

StyledRowContainer.defaultProps = { theme: Base };
export const SpacesRowContainer = ({ portals }: TRowContainer) => {
  return (
    <StyledRowContainer useReactWindow={false}>
      {portals.map((item: TPortals) => (
        <SpacesRoomRow key={item.tenantId} item={item} />
      ))}
    </StyledRowContainer>
  );
};
