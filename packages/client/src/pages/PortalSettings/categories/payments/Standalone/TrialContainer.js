import React from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";

import { StyledEnterpriseComponent } from "./StyledComponent";
import BenefitsContainer from "./sub-components/BenefitsContainer";
import ButtonContainer from "./sub-components/ButtonContainer";
import TariffTitleContainer from "./sub-components/TariffTitleContainer";

const TrialContainer = (props) => {
  const { salesEmail, t, isSubscriptionExpired, theme } = props;

  return (
    <StyledEnterpriseComponent theme={theme}>
      <Text fontWeight={700} fontSize={"16px"}>
        {t("SwitchFullEnterprise")}
      </Text>
      <TariffTitleContainer
        isSubscriptionExpired={isSubscriptionExpired}
        t={t}
        isTrial
      />

      <BenefitsContainer t={t} />
      <Text fontSize="14px" className="payments_renew-subscription">
        {t("BuyLicense")}
      </Text>
      <ButtonContainer t={t} />
    </StyledEnterpriseComponent>
  );
};

export default inject(({ auth, payments }) => {
  const { buyUrl, salesEmail } = payments;
  const { settingsStore } = auth;
  const { theme } = settingsStore;
  const isSubscriptionExpired = true;

  return { theme, buyUrl, salesEmail, isSubscriptionExpired };
})(withRouter(observer(TrialContainer)));
