import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";

const InfoPanelHeaderContent = () => {
    return <>Info</>;
};

export default inject(({}) => {
    return {};
})(observer(InfoPanelHeaderContent));
