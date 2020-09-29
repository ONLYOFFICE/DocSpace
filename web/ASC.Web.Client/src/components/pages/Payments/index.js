import React from "react";

import { withRouter } from "react-router";
import { connect } from "react-redux";
import PaymentsEnterprise from "../PaymentsEnterprise";
import PaymentsSaaS from "../PaymentsSaaS";
class Payments extends React.Component {
  render() {
    const { standaloneMode } = this.props;
    return standaloneMode ? <PaymentsEnterprise /> : <PaymentsSaaS />;
  }
}

function mapStateToProps(state) {
  return {
    standaloneMode: state.payments.standaloneMode,
  };
}

export default connect(mapStateToProps)(withRouter(Payments));
