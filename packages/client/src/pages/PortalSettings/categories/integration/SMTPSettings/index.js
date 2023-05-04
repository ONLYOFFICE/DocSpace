import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";

import CustomSettings from "./sub-components/CustomSettings";
import { StyledComponent } from "./StyledComponent";

const SMTPSettings = (props) => {
  const { setInitSMTPSettings } = props;

  const { t, ready } = useTranslation(["SMTPSettings", "Settings", "Common"]);
  const [isInit, setIsInit] = useState(false);

  const init = async () => {
    await setInitSMTPSettings();
    setIsInit(true);
  };
  useEffect(() => {
    if (!ready) return;

    init();
  }, [ready]);

  if (!isInit) return <></>;

  return (
    <StyledComponent>
      <div className="smtp-settings_main-title">
        <Text fontWeight={700} fontSize={"16px"}>
          {t("Settings:SMTPSettings")}
        </Text>
        <HelpButton
          className="smtp-settings_help-button"
          tooltipContent={t("HelpText")}
          place="bottom"
        />
      </div>
      <Text className="smtp-settings_description">
        {t("Settings:SMTPSettingsDescription")}
      </Text>

      <CustomSettings t={t} />
    </StyledComponent>
  );
};

export default inject(({ setup }) => {
  const { setInitSMTPSettings } = setup;

  return { setInitSMTPSettings };
})(observer(SMTPSettings));
