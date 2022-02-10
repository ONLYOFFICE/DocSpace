import { inject, observer } from "mobx-react";
import React from "react";

const SubRoomInfoBody = ({ children }) => {
    console.log("Body children - ", children);

    return (
        <div>
            <div>ROOM INFO BODY</div>
            <div>Children: </div>
            {children}
        </div>
    );
};

export default inject(({ roomInfoStore }) => {
    return {};
})(observer(SubRoomInfoBody));
