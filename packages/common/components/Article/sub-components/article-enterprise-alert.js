import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";

import { combineUrl } from "@docspace/common/utils";
import history from "@docspace/common/history";

import AlertComponent from "../../AlertComponent";
import Loaders from "../../Loaders";

const PROXY_BASE_URL = combineUrl(
  window.DocSpaceConfig?.proxy?.url,
  "/portal-settings"
);

const ArticleEnterpriseAlert = ({
  theme,
  toggleArticleOpen,
  isLicenseDateExpired,
  trialDaysLeft,
  isTrial,
  paymentDate,
  isEnterprise,
}) => {
  const { t, ready } = useTranslation("Common");

  const [isClose, setIsClose] = useState(
    localStorage.getItem("enterpriseAlertClose")
  );

  const onCloseClick = () => {
    localStorage.setItem("enterpriseAlertClose", true);
    setIsClose(true);
  };

  const onAlertClick = () => {
    const paymentPageUrl = combineUrl(
      PROXY_BASE_URL,
      "/payments/portal-payments"
    );
    history.push(paymentPageUrl);
    toggleArticleOpen();
  };

  const titleFunction = () => {
    if (isTrial) {
      if (isLicenseDateExpired) return t("Common:TrialExpired");
      return t("Common:TrialDaysLeft", { count: trialDaysLeft });
    }

    return t("TariffEnterprise");
  };

  const descriptionFunction = () => {
    if (isLicenseDateExpired) {
      if (isTrial) return;

      return t("Common:SubscriptionExpired");
    }

    return t("Common:SubscriptionIsExpiring", { date: paymentDate });
  };

  const title = titleFunction();

  const additionalDescription = t("Common:RenewSubscription");

  const description = descriptionFunction();

  const color = isLicenseDateExpired
    ? theme.catalog.paymentAlert.warningColor
    : theme.catalog.paymentAlert.color;

  const isShowLoader = !ready;

  if (isEnterprise && isClose) return <></>;

  return isShowLoader ? (
    <Loaders.Rectangle width="210px" height="88px" />
  ) : (
    <AlertComponent
      id="document_catalog-payment-alert"
      borderColor={color}
      titleColor={color}
      onAlertClick={onAlertClick}
      onCloseClick={onCloseClick}
      title={title}
      titleFontSize="11px"
      description={description}
      additionalDescription={additionalDescription}
      needArrowIcon={isTrial}
      needCloseIcon={!isTrial}
    />
  );
};

export default withRouter(
  inject(({ auth }) => {
    const {
      currentQuotaStore,
      settingsStore,
      currentTariffStatusStore,
      isEnterprise,
    } = auth;
    const { isTrial } = currentQuotaStore;
    const { theme } = settingsStore;
    const {
      isLicenseDateExpired,
      trialDaysLeft,
      paymentDate,
    } = currentTariffStatusStore;
    return {
      isEnterprise,
      isTrial,
      isLicenseDateExpired,
      trialDaysLeft,
      paymentDate,
      theme,
    };
  })(observer(ArticleEnterpriseAlert))
);
