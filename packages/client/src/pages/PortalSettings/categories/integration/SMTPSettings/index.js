import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import RadioButtonGroup from "@docspace/components/radio-button-group";
import CustomSettings from "./sub-components/CustomSettings";
import { getSMTPSettings } from "@docspace/common/api/settings";
import StyledComponent from "./StyledComponent";

const SMTPSettings = (props) => {
  const { standalone, setSMTPSettings } = props;

  const { t, ready } = useTranslation(["SMTPSettings", "Settings"]);

  const [isInit, setIsInit] = useState(false);

  const init = async () => {
    const res = await getSMTPSettings();
    setSMTPSettings(res);
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
      <div>{<CustomSettings t={t} />}</div>
    </StyledComponent>
  );
};

export default inject(({ auth, setup }) => {
  const { standalone } = auth.settingsStore;
  const { setSMTPSettings } = setup;

  return { standalone, setSMTPSettings };
})(observer(SMTPSettings));
