import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";

import LicenseContainer from "./LicenseContainer";
import { StyledComponent } from "./StyledComponent";
import ContactContainer from "./ContactContainer";
import EnterpriseContainer from "./EnterpriseContainer";
import TrialContainer from "./TrialContainer";

const StandalonePage = (props) => {
  const {
    enterpriseInit,
    isInitPaymentPage,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
    isTrial,
  } = props;
  const { t, ready } = useTranslation(["PaymentsEnterprise", "Common"]);

  useEffect(() => {
    setDocumentTitle(t("Common:PaymentsTitle"));
  }, [ready]);

  useEffect(() => {
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota) return;

    enterpriseInit();
  }, [isLoadedTariffStatus, isLoadedCurrentQuota]);

  if (
    !isInitPaymentPage ||
    !isLoadedTariffStatus ||
    !isLoadedCurrentQuota ||
    !ready
  )
    return <></>;

  return (
    <StyledComponent>
      {isTrial ? <TrialContainer t={t} /> : <EnterpriseContainer t={t} />}
      <LicenseContainer t={t} />
      <ContactContainer t={t} />
    </StyledComponent>
  );
};

export default inject(({ auth, payments }) => {
  const { currentQuotaStore, currentTariffStatusStore } = auth;

  const { enterpriseInit, isInitPaymentPage } = payments;
  const { isLoaded: isLoadedCurrentQuota, isTrial } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;

  return {
    isTrial,
    enterpriseInit,
    isInitPaymentPage,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
  };
})(withRouter(observer(StandalonePage)));
