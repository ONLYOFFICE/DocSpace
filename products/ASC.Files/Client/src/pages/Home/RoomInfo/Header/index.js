import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";

const RoomInfoHeaderContent = () => {
    return <div>NOTIFICATION HEADER CONTENT</div>;
};

export default inject(({}) => {
    return {};
})(observer(RoomInfoHeaderContent));
