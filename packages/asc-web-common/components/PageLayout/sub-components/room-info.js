import { observer } from "mobx-react";
import React from "react";
import styled from "styled-components";

const RoomInfo = ({ children }) => {
    const StyledRoomInfo = styled.div`
        height: 100%;
        width: 400px;
        background-color: #ffffff;
        border-left: 1px solid #eceef1;
        padding: 12px 16px;
    `;

    return <StyledRoomInfo>{children}</StyledRoomInfo>;
};

export default observer(RoomInfo);
