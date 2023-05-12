import React from "react";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import { StyledTitleComponent } from "../StyledComponent";

const TariffTitleContainer = ({
  t,
  limitedWidth = true,
  isSubscriptionExpired,
  isTrial,
}) => {
  const alertComponent = () => {
    if (isTrial) {
      return isSubscriptionExpired ? (
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
          {t("FreeDaysLeft", { count: "12" })}
        </Text>
      );
    }

    return (
      isSubscriptionExpired && (
        <Text className="payments_subscription-expired" isBold fontSize="14px">
          {t("EnterpriseSubscriptionExpired")}
        </Text>
      )
    );
  };

  const expiresDate = () => {
    if (isTrial) {
      return t("TrialExpiresDate", {
        finalDate: "Monday, May 15, 2023",
      });
    }
    return t("EnterpriseExpiresDate", {
      finalDate: "Tuesday, December 19, 2023.",
    });
  };
  return (
    <StyledTitleComponent
      isSubscriptionExpired={isSubscriptionExpired}
      limitedWidth={limitedWidth}
    >
      <div className="payments_subscription">
        <div className="title">
          <Text fontWeight={600} fontSize="14px" as="span">
            {t("EnterpriseEdition")}{" "}
          </Text>
          {!isSubscriptionExpired && (
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

export default inject(({ payments }) => {
  const {} = payments;

  return {};
})(observer(TariffTitleContainer));
