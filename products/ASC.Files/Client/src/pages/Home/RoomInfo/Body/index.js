import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { observer, inject } from "mobx-react";

const RoomInfoBodyContent = () => {
    return <div>ROOM INFO BODY CONTENT</div>;
};

export default inject(({}) => {
    return {};
})(
    withRouter(
        withTranslation(["Home", "Common", "Translations"])(
            observer(RoomInfoBodyContent)
        )
    )
);
