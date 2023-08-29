import React from "react";

import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";

import { StyledEnterpriseComponent } from "./StyledComponent";
import BenefitsContainer from "SRC_DIR/components/StandaloneComponents/BenefitsContainer";
import ButtonContainer from "./sub-components/ButtonContainer";
import TariffTitleContainer from "./sub-components/TariffTitleContainer";

const TrialContainer = (props) => {
  const { t } = props;

  return (
    <StyledEnterpriseComponent>
      <Text fontWeight={700} fontSize={"16px"}>
        {t("ActivateSwithToProHeader")}
      </Text>
      <TariffTitleContainer t={t} />

      <BenefitsContainer t={t} />
      <Text fontSize="14px" className="payments_renew-subscription">
        {t("ActivatePurchaseBuyLicense")}
      </Text>
      <ButtonContainer t={t} />
    </StyledEnterpriseComponent>
  );
};

export default inject(({ auth }) => {
  const { currentTariffStatusStore } = auth;
  const { isLicenseDateExpired } = currentTariffStatusStore;

  return { isLicenseDateExpired };
})(observer(TrialContainer));
