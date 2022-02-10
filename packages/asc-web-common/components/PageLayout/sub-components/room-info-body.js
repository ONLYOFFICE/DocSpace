import { inject, observer } from "mobx-react";
import React from "react";

const SubRoomInfoBody = ({ children }) => {
    const content = children.props.children;

    return <>{content}</>;
};

SubRoomInfoBody.displayName = "SubRoomInfoBody";

export default inject(() => {
    return {};
})(observer(SubRoomInfoBody));
