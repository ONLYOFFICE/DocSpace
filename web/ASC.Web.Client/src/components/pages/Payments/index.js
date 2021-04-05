import React from "react";

import { withRouter } from "react-router";
import PaymentsEnterprise from "../PaymentsEnterprise";
import PaymentsSaaS from "../PaymentsSaaS";
import { inject, observer } from "mobx-react";
class Payments extends React.Component {
  render() {
    const { standaloneMode } = this.props;
    return standaloneMode ? <PaymentsEnterprise /> : <PaymentsSaaS />;
  }
}

export default inject(({ payments }) => ({
  standaloneMode: payments.standaloneMode,
}))(withRouter(observer(Payments)));
