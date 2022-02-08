import { inject, observer } from "mobx-react";
import React from "react";

const SubNotificationsBody = ({ children }) => {
    console.log("SNB - ", children);
    return <span>{children}</span>;
};

export default inject(() => {
    return {};
})(observer(SubNotificationsBody));
