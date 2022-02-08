import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";

const NotificationsBodyContent = () => {
    return <div>NOTIFICATION BODY CONTENT</div>;
};

export default inject(({}) => {
    return {};
})(withTranslation()(withRouter(observer(NotificationsBodyContent))));
