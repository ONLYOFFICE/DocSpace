import { inject, observer } from "mobx-react";
import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";

const RoomInfoHeaderContent = () => {
    return <>Room</>;
};

export default inject(({}) => {
    return {};
})(observer(RoomInfoHeaderContent));
