import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";

import PaymentsEnterprise from "./Standalone";
import PaymentsSaaS from "./SaaS";
import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";

const PaymentsPage = (props) => {
  const { standalone, isEnterpriseEdition } = props;

  useEffect(() => {
    if (!isEnterpriseEdition) {
      history.push(
        combineUrl(window.DocSpaceConfig?.proxy?.url, "/portal-settings/")
      );
    }
  }, []);

  if (standalone && !isEnterpriseEdition) return <></>;

  return standalone ? <PaymentsEnterprise /> : <PaymentsSaaS />;
};

export default inject(({ auth }) => {
  const { settingsStore, isEnterpriseEdition } = auth;
  const { standalone } = settingsStore;

  return {
    isEnterpriseEdition,
    standalone,
  };
})(withRouter(observer(PaymentsPage)));
