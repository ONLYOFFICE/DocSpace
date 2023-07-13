import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";

import CustomSettings from "./sub-components/CustomSettings";
import { StyledComponent } from "./StyledComponent";
import Loaders from "@docspace/common/components/Loaders";

let timerId = null;
const SMTPSettings = (props) => {
  const { setInitSMTPSettings, organizationName } = props;

  const { t, ready } = useTranslation(["SMTPSettings", "Settings", "Common"]);
  const [isInit, setIsInit] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const init = async () => {
    await setInitSMTPSettings();

    setIsLoading(false);
    setIsInit(true);
  };
  useEffect(() => {
    timerId = setTimeout(() => {
      setIsLoading(true);
    }, 400);

    init();

    () => {
      clearTimeout(timerId);
      timerId = null;
    };
  }, []);

  const isLoadingContent = isLoading || !ready;

  if (!isLoading && !isInit) return <></>;

  if (isLoadingContent && !isInit) return <Loaders.SettingsSMTP />;

  return (
    <StyledComponent>
      <div className="smtp-settings_main-title">
        <Text fontWeight={700} fontSize="16px">
          {t("Settings:SMTPSettings")}
        </Text>
        <HelpButton
          place="bottom"
          offsetBottom={0}
          className="smtp-settings_help-button"
          place="bottom"
          offsetBottom={0}
          tooltipContent={
            <Text fontSize="12px">{t("HelpText", { organizationName })}</Text>
          }
        />
      </div>
      <Text className="smtp-settings_description">
        {t("Settings:SMTPSettingsDescription", { organizationName })}
      </Text>

      <CustomSettings t={t} />
    </StyledComponent>
  );
};

export default inject(({ auth, setup }) => {
  const { settingsStore } = auth;
  const { organizationName } = settingsStore;
  const { setInitSMTPSettings } = setup;

  return { setInitSMTPSettings, organizationName };
})(observer(SMTPSettings));
