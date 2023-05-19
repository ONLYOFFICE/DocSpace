import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { observer, inject } from "mobx-react";
import { Trans } from "react-i18next";

import Text from "@docspace/components/text";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

import BenefitsContainer from "./BenefitsContainer";
import { StyledComponent } from "./StyledComponent";
import OfficialDocumentation from "./sub-components/OfficialDocumentation";
import ContactContainer from "./ContactContainer";

const Bonus = ({ helpUrl, getSettingsPayment }) => {
  const { t } = useTranslation("PaymentsEnterprise");

  useEffect(() => {
    const fetch = async () => {
      await getSettingsPayment();
    };

    fetch();
  }, []);

  return (
    <StyledComponent>
      <BenefitsContainer />
      <Text fontWeight={600}>{t("UpgradeToProBannerInstructionHeader")}</Text>
      <Text>{t("UpgradeToProBannerInstructionDescr")}</Text>

      <OfficialDocumentation />

      <Text fontWeight={600} className="upgrade-info">
        <Trans
          i18nKey="UpgradeToProBannerInstructionNote"
          ns="PaymentsEnterprise"
          t={t}
        >
          Please note that the editors will be unavailable during the upgrade.
          We also recommend to
          <ColorTheme
            tag="a"
            themeId={ThemeType.Link}
            fontWeight="600"
            href={helpUrl}
          >
            backup your data
          </ColorTheme>
          before you start.
        </Trans>
      </Text>

      <ContactContainer />
    </StyledComponent>
  );
};

export default inject(({ auth, payments }) => {
  const { settingsStore } = auth;
  const { theme } = settingsStore;
  const { getSettingsPayment } = payments;

  const helpUrl = "portal-settings/";

  return { helpUrl, theme, getSettingsPayment };
})(observer(Bonus));
