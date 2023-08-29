import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { observer, inject } from "mobx-react";

import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";

import BenefitsContainer from "SRC_DIR/components/StandaloneComponents/BenefitsContainer";
import StyledComponent from "./StyledComponent";
import OfficialDocumentation from "./sub-components/OfficialDocumentation";
import ContactContainer from "SRC_DIR/components/StandaloneComponents/ContactContainer";

const Bonus = ({ standaloneInit, isInitPaymentPage }) => {
  const { t, ready } = useTranslation("PaymentsEnterprise");

  useEffect(() => {
    standaloneInit();
  }, []);

  if (!isInitPaymentPage || !ready) return <Loaders.PaymentsStandaloneLoader />;

  return (
    <StyledComponent>
      <BenefitsContainer />
      <Text fontWeight={600}>{t("UpgradeToProBannerInstructionHeader")}</Text>
      <Text>{t("UpgradeToProBannerInstructionDescr")}</Text>

      <OfficialDocumentation />

      <ContactContainer />
    </StyledComponent>
  );
};

export default inject(({ auth, payments }) => {
  const { standaloneInit, isInitPaymentPage } = payments;

  return {
    standaloneInit,
    isInitPaymentPage,
  };
})(observer(Bonus));
