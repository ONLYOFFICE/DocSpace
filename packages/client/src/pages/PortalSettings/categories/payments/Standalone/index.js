import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";

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
    isEnterpriseEdition,
    isTrial,
  } = props;
  const { t, ready } = useTranslation([
    "Payments",
    "Common",
    "Settings",
    "ChangePasswordDialog",
  ]);

  useEffect(() => {
    enterpriseInit();
  }, []);

  useEffect(() => {
    setDocumentTitle(t("Common:PaymentsTitle"));
  }, [ready]);

  useEffect(() => {
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota) return;

    if (!isEnterpriseEdition) {
      history.push(
        combineUrl(window.DocSpaceConfig?.proxy?.url, "/portal-settings/")
      );
    }
  }, [isLoadedTariffStatus, isLoadedCurrentQuota]);

  if (
    !isInitPaymentPage ||
    !isLoadedTariffStatus ||
    !isLoadedCurrentQuota ||
    !ready ||
    !isEnterpriseEdition
  )
    return <></>;

  return (
    <StyledComponent>
      {true ? <TrialContainer t={t} /> : <EnterpriseContainer t={t} />}
      <LicenseContainer t={t} />
      <ContactContainer t={t} />
    </StyledComponent>
  );
};

export default inject(({ auth, payments }) => {
  const {
    currentQuotaStore,
    currentTariffStatusStore,
    isEnterpriseEdition,
  } = auth;

  const { enterpriseInit, isInitPaymentPage } = payments;
  const { isLoaded: isLoadedCurrentQuota } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;

  const isTrial = true,
    isEnterprise = false;

  return {
    isTrial,
    enterpriseInit,
    isInitPaymentPage,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
    isEnterpriseEdition,
  };
})(withRouter(observer(StandalonePage)));
