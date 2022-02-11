import { inject, observer } from "mobx-react";
import React from "react";

const SubInfoPanelBody = ({ children }) => {
    const content = children.props.children;

    return <>{content}</>;
};

SubInfoPanelBody.displayName = "SubInfoPanelBody";

export default inject(() => {
    return {};
})(observer(SubInfoPanelBody));
