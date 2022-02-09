import { inject, observer } from "mobx-react";
import React from "react";

const SubRoomInfoHeader = ({ children }) => {
    console.log("Header children - ", { children });
    return (
        <div>
            <div>ROOM INFO HEADER</div>
            <div>Children - {children}</div>
        </div>
    );
};

export default inject(() => {
    return {};
})(observer(SubRoomInfoHeader));
