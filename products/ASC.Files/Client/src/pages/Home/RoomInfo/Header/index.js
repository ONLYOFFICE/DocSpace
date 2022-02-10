import { inject, observer } from "mobx-react";
import React from "react";
import { styled } from "styled-components";

const RoomInfoHeaderContent = () => {
    const StyledRoomInfoHeader = styled.div`
        width: 100%;
        height: 53px;
        display: flex;
        justify-content: space-between;
        align-item: center;
        padding: 8px 16px;

        //styleName: H2 21-28 - Bold;
        font-family: Open Sans;
        font-size: 21px;
        font-style: normal;
        font-weight: 700;
        line-height: 28px;
        letter-spacing: 0px;
        text-align: left;
    `;

    return (
        <StyledRoomInfoHeader>
            Room <button>X</button>
        </StyledRoomInfoHeader>
    );
};

export default inject(({}) => {
    return {};
})(observer(RoomInfoHeaderContent));
