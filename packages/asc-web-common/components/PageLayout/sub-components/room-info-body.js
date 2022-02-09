import { inject, observer } from "mobx-react";
import React from "react";

const SubRoomInfoBody = ({ children }) => {
    console.log("SRIB children - ", children);
    return (
        <div>
            <div>ROOM INFO BODY</div>
            <div>Children - {children}</div>
        </div>
    );
};

export default inject(() => {
    return {};
})(observer(SubRoomInfoBody));
