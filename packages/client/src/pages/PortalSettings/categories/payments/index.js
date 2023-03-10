import React, { useEffect } from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import moment from "moment";
import { useTranslation } from "react-i18next";

import Loaders from "@docspace/common/components/Loaders";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";

import PaymentContainer from "./PaymentContainer";

const PaymentsPage = ({
  language,
  isLoadedTariffStatus,
  isLoadedCurrentQuota,
  isInitPaymentPage,
  init,
}) => {
  const { t, ready } = useTranslation(["Payments", "Common", "Settings"]);

  useEffect(() => {
    moment.locale(language);
  }, []);

  useEffect(() => {
    setDocumentTitle(t("Settings:Payments"));
  }, [ready]);

  useEffect(() => {
    if (!isLoadedTariffStatus || !isLoadedCurrentQuota) return;

    init();
  }, [isLoadedTariffStatus, isLoadedCurrentQuota]);

  return !isInitPaymentPage || !ready ? (
    <Loaders.PaymentsLoader />
  ) : (
    <PaymentContainer t={t} />
  );
};

PaymentsPage.propTypes = {
  isLoaded: PropTypes.bool,
};

export default inject(({ auth, payments }) => {
  const { language, currentQuotaStore, currentTariffStatusStore } = auth;

  const { isLoaded: isLoadedCurrentQuota } = currentQuotaStore;
  const { isLoaded: isLoadedTariffStatus } = currentTariffStatusStore;
  const { isInitPaymentPage, init } = payments;

  return {
    init,
    isInitPaymentPage,
    language,
    isLoadedTariffStatus,
    isLoadedCurrentQuota,
  };
})(withRouter(observer(PaymentsPage)));
