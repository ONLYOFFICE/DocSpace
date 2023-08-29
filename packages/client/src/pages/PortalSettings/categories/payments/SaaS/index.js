import React, { useEffect } from "react";

import { inject, observer } from "mobx-react";
import moment from "moment";
import { useTranslation } from "react-i18next";

import { regDesktop } from "@docspace/common/desktop";
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
  user,
  encryptionKeys,
  isEncryption,
  setEncryptionKeys,
  isDesktop,
  isDesktopClientInit,
  setIsDesktopClientInit,
}) => {
  const { t, ready } = useTranslation(["Payments", "Common", "Settings"]);

  useEffect(() => {
    moment.locale(language);

    if (isDesktop && !isDesktopClientInit) {
      setIsDesktopClientInit(true);

      regDesktop(
        user,
        isEncryption,
        encryptionKeys,
        setEncryptionKeys,
        false,
        null,
        t
      );
    }
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
    userStore,
    settingsStore,
  } = auth;
  const { user } = userStore;
  const { isLoaded: isLoadedCurrentQuota } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;
  const {
    isInitPaymentPage,
    init,
    isUpdatingBasicSettings,
    resetTariffContainerToBasic,
  } = payments;
  const {
    isEncryptionSupport,
    setEncryptionKeys,
    encryptionKeys,
    isDesktopClient,
    isDesktopClientInit,
    setIsDesktopClientInit,
  } = settingsStore;
  return {
    isDesktopClientInit,
    setIsDesktopClientInit,
    isDesktop: isDesktopClient,
    user,
    encryptionKeys,
    isEncryption: isEncryptionSupport,
    setEncryptionKeys,
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
