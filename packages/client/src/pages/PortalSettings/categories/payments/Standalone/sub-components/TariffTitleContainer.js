import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import { StyledTitleComponent } from "../StyledComponent";

const TariffTitleContainer = ({
  isLicenseDateExpired,
  isTrial,
  trialDaysLeft,
  paymentDate,
}) => {
  const { t } = useTranslation(["PaymentsEnterprise", "Common"]);
  const alertComponent = () => {
    if (isTrial) {
      return isLicenseDateExpired ? (
        <Text
          className="payments_subscription-expired"
          fontWeight={600}
          fontSize="14px"
        >
          {t("Common:TrialExpired")}
        </Text>
      ) : (
        <Text
          className="payments_subscription-expired"
          fontWeight={600}
          fontSize="14px"
        >
          {t("FreeDaysLeft", { count: trialDaysLeft })}
        </Text>
      );
    }

    return (
      isLicenseDateExpired && (
        <Text className="payments_subscription-expired" isBold fontSize="14px">
          {t("TopBottonsEnterpriseExpired")}
        </Text>
      )
    );
  };

  const expiresDate = () => {
    if (isTrial) {
      return t("ActivateTariffEnterpriseTrialExpiration", {
        date: paymentDate,
      });
    }
    return t("ActivateTariffEnterpriseExpiration", {
      date: paymentDate,
    });
  };

  return (
    <StyledTitleComponent
      isLicenseDateExpired={isLicenseDateExpired}
      limitedWidth={isTrial ? true : isLicenseDateExpired}
    >
      <div className="payments_subscription">
        <div className="title">
          <Text fontWeight={600} fontSize="14px" as="span">
            {t("ActivateTariffDescr")}{" "}
          </Text>
          {!isLicenseDateExpired && (
            <Text fontSize="14px" as="span">
              {expiresDate()}
            </Text>
          )}
        </div>
        {alertComponent()}
      </div>
    </StyledTitleComponent>
  );
};

export default inject(({ auth }) => {
  const { currentTariffStatusStore, currentQuotaStore } = auth;
  const {
    trialDaysLeft,
    paymentDate,
    isLicenseDateExpired,
  } = currentTariffStatusStore;
  const { isTrial } = currentQuotaStore;
  return { isTrial, trialDaysLeft, paymentDate, isLicenseDateExpired };
})(observer(TariffTitleContainer));
