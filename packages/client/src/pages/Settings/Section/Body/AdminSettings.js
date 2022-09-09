import React from "react";
import { inject, observer } from "mobx-react";
import ToggleButton from "@docspace/components/toggle-button";
import StyledSettings from "./StyledSettings";

const AdminSettings = ({
  storeForceSave,
  setStoreForceSave,
  enableThirdParty,
  setEnableThirdParty,
  t,
}) => {
  const onChangeStoreForceSave = React.useCallback(() => {
    setStoreForceSave(!storeForceSave);
  }, [setStoreForceSave, storeForceSave]);

  const onChangeThirdParty = React.useCallback(() => {
    setEnableThirdParty(!enableThirdParty, "enableThirdParty");
  }, [setEnableThirdParty, enableThirdParty]);

  return (
    <StyledSettings>
      <ToggleButton
        className="toggle-btn"
        label={t("IntermediateVersion")}
        onChange={onChangeStoreForceSave}
        isChecked={storeForceSave}
      />
      <ToggleButton
        className="toggle-btn"
        label={t("ThirdPartyBtn")}
        onChange={onChangeThirdParty}
        isChecked={enableThirdParty}
      />
    </StyledSettings>
  );
};

export default inject(({ settingsStore }) => {
  const {
    enableThirdParty,
    setEnableThirdParty,
    storeForcesave,
    setStoreForceSave,
  } = settingsStore;

  return {
    storeForceSave: storeForcesave,
    setStoreForceSave,
    enableThirdParty,
    setEnableThirdParty,
  };
})(observer(AdminSettings));
