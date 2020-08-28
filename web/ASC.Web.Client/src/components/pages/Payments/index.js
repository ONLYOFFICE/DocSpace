import React from "react";

import { withRouter } from "react-router";
import { connect } from "react-redux";
import PaymentsEnterprise from "../PaymentsEnterprise";
import PaymentsSaaS from "../PaymentsSaaS";
class Payments extends React.Component {
  render() {
    const { standAloneMode } = this.props;
    return standAloneMode ? <PaymentsEnterprise /> : <PaymentsSaaS />;
  }
}

function mapStateToProps(state) {
  return {
    standAloneMode: state.payments.standAloneMode,
  };
}

export default connect(mapStateToProps)(withRouter(Payments));
