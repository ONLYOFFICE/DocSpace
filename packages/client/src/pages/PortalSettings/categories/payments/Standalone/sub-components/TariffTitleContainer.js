import React from "react";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import { StyledTitleComponent } from "../StyledComponent";

const TariffTitleContainer = ({
  t,
  isLicenseDateExpires,
  isTrial,
  trialDaysLeft,
  paymentDate,
}) => {
  const alertComponent = () => {
    if (isTrial) {
      return isLicenseDateExpires ? (
        <Text
          className="payments_subscription-expired"
          fontWeight={600}
          fontSize="14px"
        >
          {t("TrialExpired")}
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
      isLicenseDateExpires && (
        <Text className="payments_subscription-expired" isBold fontSize="14px">
          {t("EnterpriseSubscriptionExpired")}
        </Text>
      )
    );
  };

  const expiresDate = () => {
    if (isTrial) {
      return t("TrialExpiresDate", {
        finalDate: paymentDate,
      });
    }
    return t("EnterpriseExpiresDate", {
      finalDate: paymentDate,
    });
  };

  return (
    <StyledTitleComponent
      isLicenseDateExpires={isLicenseDateExpires}
      limitedWidth={isTrial ? true : isLicenseDateExpires}
    >
      <div className="payments_subscription">
        <div className="title">
          <Text fontWeight={600} fontSize="14px" as="span">
            {t("EnterpriseEdition")}{" "}
          </Text>
          {!isLicenseDateExpires && (
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
    isLicenseDateExpires,
  } = currentTariffStatusStore;
  const { isTrial } = currentQuotaStore;
  return { isTrial, trialDaysLeft, paymentDate, isLicenseDateExpires };
})(observer(TariffTitleContainer));
