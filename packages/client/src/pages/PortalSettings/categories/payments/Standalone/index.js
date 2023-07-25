import React, { useEffect } from "react";

import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import Loaders from "@docspace/common/components/Loaders";

import LicenseContainer from "./LicenseContainer";
import { StyledComponent } from "./StyledComponent";
import ContactContainer from "SRC_DIR/components/StandaloneComponents/ContactContainer";
import EnterpriseContainer from "./EnterpriseContainer";
import TrialContainer from "./TrialContainer";

const StandalonePage = (props) => {
  const {
    standaloneInit,
    isInitPaymentPage,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
    isTrial,
    isUpdatingBasicSettings,
  } = props;

  const { t, ready } = useTranslation(["PaymentsEnterprise", "Common"]);

  useEffect(() => {
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota || !ready) return;

    setDocumentTitle(t("Common:PaymentsTitle"));

    standaloneInit(t);
  }, [isLoadedTariffStatus, isLoadedCurrentQuota, ready]);

  if (
    !isInitPaymentPage ||
    !isLoadedTariffStatus ||
    !isLoadedCurrentQuota ||
    !ready ||
    isUpdatingBasicSettings
  )
    return <Loaders.PaymentsStandaloneLoader isEnterprise={!isTrial} />;

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

  const { standaloneInit, isInitPaymentPage, isUpdatingBasicSettings } =
    payments;
  const { isLoaded: isLoadedCurrentQuota, isTrial } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;

  return {
    isTrial,
    standaloneInit,
    isInitPaymentPage,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
    isUpdatingBasicSettings,
  };
})(observer(StandalonePage));
