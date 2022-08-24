import React from "react";
import styled from "styled-components";
import { roomTypes } from "../data";
import { withTranslation } from "react-i18next";

import RoomType from "./RoomType";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

const StyledRoomTypeList = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
`;

const RoomTypeList = ({ t, setRoomType }) => {
  return (
    <StyledRoomTypeList>
      {roomTypes.map((room) => (
        <RoomType
          t={t}
          key={room.type}
          room={room}
          type={"listItem"}
          onClick={() => setRoomType(room.type)}
        />
      ))}
    </StyledRoomTypeList>
  );
};

export default withTranslation(["CreateEditRoomDialog"])(
  withLoader(RoomTypeList)(<Loaders.RoomTypeListLoader />)
);
