import React from "react";
import { useTranslation } from "react-i18next";
import { observer, inject } from "mobx-react";

import Text from "@docspace/components/text";

import { StyledComponent } from "../StyledComponent";

const OfficialDocumentation = () => {
  const { t } = useTranslation("PaymentsEnterprise");

  return (
    <StyledComponent>
      <div className="official-documentation">
        <Text fontWeight={600}>
          {"—" + " " + t("UpgradeToProBannerInstructionItemDocker")}
        </Text>
        <Text fontWeight={600}>
          {"—" + " " + t("UpgradeToProBannerInstructionItemLinux")}
        </Text>
        <Text fontWeight={600}>
          {"—" + " " + t("UpgradeToProBannerInstructionItemWindows")}
        </Text>
      </div>
    </StyledComponent>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { theme } = settingsStore;

  return {};
})(observer(OfficialDocumentation));
