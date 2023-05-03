import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import { getSMTPSettings } from "@docspace/common/api/settings";

import CustomSettings from "./sub-components/CustomSettings";
import { StyledComponent } from "./StyledComponent";

const SMTPSettings = (props) => {
  const { standalone, setInitSMTPSettings } = props;

  const { t, ready } = useTranslation(["SMTPSettings", "Settings", "Common"]);

  const [isInit, setIsInit] = useState(false);

  const init = async () => {
    const res = await getSMTPSettings();

    setInitSMTPSettings(res);
    setIsInit(true);
  };
  useEffect(() => {
    if (!ready) return;

    init();
  }, [ready]);

  if (!isInit) return <></>;

  return (
    <StyledComponent>
      <Text className="smtp-settings_description">
        {t("Settings:SMTPSettingsDescription")}
      </Text>

      <CustomSettings t={t} />
    </StyledComponent>
  );
};

export default inject(({ auth, setup }) => {
  const { standalone } = auth.settingsStore;
  const { setInitSMTPSettings } = setup;

  return { standalone, setInitSMTPSettings };
})(observer(SMTPSettings));
