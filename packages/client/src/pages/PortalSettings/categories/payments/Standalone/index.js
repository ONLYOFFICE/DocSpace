import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

const StandalonePage = ({}) => {
  return <></>;
};

export default inject(({ auth, payments }) => {
  const { currentQuotaStore, currentTariffStatusStore } = auth;

  const {} = payments;

  return {};
})(withRouter(observer(StandalonePage)));
