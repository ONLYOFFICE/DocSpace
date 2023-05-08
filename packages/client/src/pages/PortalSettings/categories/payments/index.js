import React from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";

import PaymentsEnterprise from "./Standalone";
import PaymentsSaaS from "./SaaS";

const PaymentsPage = (props) => {
  const { standalone } = props;
  return standalone ? <PaymentsEnterprise /> : <PaymentsSaaS />;
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { standalone } = settingsStore;

  return {
    standalone,
  };
})(withRouter(observer(PaymentsPage)));
