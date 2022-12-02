import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

import RoomType from "./RoomType";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";
import { RoomsType } from "./../../../../../../common/constants/index";

const StyledRoomTypeList = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
`;

const RoomTypeList = ({ t, setRoomType }) => {
  console.log(Object.values(RoomsType));
  return (
    <StyledRoomTypeList>
      {Object.values(RoomsType).map((roomType) => (
        <RoomType
          id={roomType}
          t={t}
          key={roomType}
          roomType={roomType}
          type={"listItem"}
          onClick={() => setRoomType(roomType)}
        />
      ))}
    </StyledRoomTypeList>
  );
};

export default withTranslation(["CreateEditRoomDialog"])(
  withLoader(RoomTypeList)(<Loaders.RoomTypeListLoader />)
);
