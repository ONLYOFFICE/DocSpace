import { inject, observer } from "mobx-react";
import React from "react";

const SubRoomInfoBody = ({ children, isVisible }) => {
    console.log("Body children - ", children);

    return (
        <div>
            <p>Room Info Is {isVisible ? "Visible" : "Hidden"}</p>
            <div>ROOM INFO BODY</div>
            <div>Children - {children}</div>
        </div>
    );
};

export default inject(({ roomInfoStore }) => {
    let isVisible = false;
    if (roomInfoStore) isVisible = roomInfoStore.isVisible;
    return {
        isVisible,
    };
})(observer(SubRoomInfoBody));
