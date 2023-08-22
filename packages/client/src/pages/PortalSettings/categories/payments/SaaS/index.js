import React, { useEffect } from "react";

import { inject, observer } from "mobx-react";
import moment from "moment";
import { useTranslation } from "react-i18next";

import Loaders from "@docspace/common/components/Loaders";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";

import PaymentContainer from "./PaymentContainer";

const SaaSPage = ({
  language,
  isLoadedTariffStatus,
  isLoadedCurrentQuota,
  isInitPaymentPage,
  init,
  isUpdatingTariff,
  isUpdatingBasicSettings,
  resetTariffContainerToBasic,
}) => {
  const { t, ready } = useTranslation(["Payments", "Common", "Settings"]);

  useEffect(() => {
    moment.locale(language);
    return () => resetTariffContainerToBasic();
  }, []);

  useEffect(() => {
    setDocumentTitle(t("Common:PaymentsTitle"));
  }, [ready]);

  useEffect(() => {
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota || !ready) return;

    init(t);
  }, [isLoadedTariffStatus, isLoadedCurrentQuota, ready]);

  return !isInitPaymentPage ||
    !ready ||
    isUpdatingTariff ||
    isUpdatingBasicSettings ? (
    <Loaders.PaymentsLoader />
  ) : (
    <PaymentContainer t={t} />
  );
};

export default inject(({ auth, payments }) => {
  const {
    language,
    currentQuotaStore,
    currentTariffStatusStore,
    isUpdatingTariff,
  } = auth;

  const { isLoaded: isLoadedCurrentQuota } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;
  const {
    isInitPaymentPage,
    init,
    isUpdatingBasicSettings,
    resetTariffContainerToBasic,
  } = payments;

  return {
    resetTariffContainerToBasic,
    isUpdatingTariff,
    init,
    isInitPaymentPage,
    language,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
    isUpdatingBasicSettings,
  };
})(observer(SaaSPage));
