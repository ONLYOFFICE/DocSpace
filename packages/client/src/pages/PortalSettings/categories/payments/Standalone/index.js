import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";

import LicenseContainer from "./LicenseContainer";
import { StyledComponent } from "./StyledComponent";
import ContactContainer from "SRC_DIR/components/StandaloneComponents/ContactContainer";
import EnterpriseContainer from "./EnterpriseContainer";
import TrialContainer from "./TrialContainer";
import Loaders from "@docspace/common/components/Loaders";

const StandalonePage = (props) => {
  const {
    standaloneInit,
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
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota || !ready) return;

    standaloneInit();
  }, [isLoadedTariffStatus, isLoadedCurrentQuota, ready]);

  if (
    !isInitPaymentPage ||
    !isLoadedTariffStatus ||
    !isLoadedCurrentQuota ||
    !ready
  )
    return isTrial ? (
      <Loaders.PaymentsStandaloneLoader />
    ) : (
      <Loaders.PaymentsStandaloneLoader isEnterprise />
    );

  return (
    <StyledComponent>
      {isTrial ? <TrialContainer t={t} /> : <EnterpriseContainer t={t} />}
      <LicenseContainer t={t} />
      <ContactContainer />
    </StyledComponent>
  );
};

export default inject(({ auth, payments }) => {
  const { currentQuotaStore, currentTariffStatusStore } = auth;

  const { standaloneInit, isInitPaymentPage } = payments;
  const { isLoaded: isLoadedCurrentQuota, isTrial } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;

  return {
    isTrial,
    standaloneInit,
    isInitPaymentPage,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
  };
})(withRouter(observer(StandalonePage)));
