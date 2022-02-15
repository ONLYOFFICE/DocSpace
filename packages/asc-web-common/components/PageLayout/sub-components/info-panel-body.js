import Scrollbar from "@appserver/components/scrollbar";
import { inject, observer } from "mobx-react";
import React from "react";

const SubInfoPanelBody = ({ children }) => {
    const content = children.props.children;

    return (
        <Scrollbar scrollclass="section-scroll" stype="mediumBlack">
            {content}
        </Scrollbar>
    );
};

SubInfoPanelBody.displayName = "SubInfoPanelBody";

export default inject(() => {
    return {};
})(observer(SubInfoPanelBody));
