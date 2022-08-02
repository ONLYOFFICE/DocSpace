import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
//import FormStore from "@docspace/studio/src/store/SsoFormStore";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";

import DisableSsoConfirmationModal from "./DisableSsoConfirmationModal";

const borderProp = { radius: "6px" };

const ToggleSSO = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    theme,
    enableSso,
    isSsoEnabled,
    openConfirmationDisableModal,
    ssoToggle,
    confirmationDisableModal,
  } = props;

  return (
    <>
      <Text className="intro-text" lineHeight="20px" color="#657077" noSelect>
        {t("SsoIntro")}
      </Text>

      <Box
        backgroundProp={
          theme.client.settings.integration.sso.toggleContentBackground
        }
        borderProp={borderProp}
        displayProp="flex"
        flexDirection="row"
        paddingProp="12px"
      >
        <ToggleButton
          className="toggle"
          isChecked={enableSso}
          onChange={
            isSsoEnabled && enableSso ? openConfirmationDisableModal : ssoToggle
          }
        />

        <Box>
          <Text as="span" fontWeight={600} lineHeight="20px" noSelect>
            {t("TurnOnSSO")}
          </Text>
          <Text lineHeight="16px" noSelect>
            {t("TurnOnSSOCaption")}
          </Text>
        </Box>
      </Box>

      {confirmationDisableModal && <DisableSsoConfirmationModal />}
    </>
  );
};

export default inject(({ auth, ssoStore }) => {
  const { theme } = auth.settingsStore;
  const {
    enableSso,
    isSsoEnabled,
    openConfirmationDisableModal,
    ssoToggle,
    confirmationDisableModal,
  } = ssoStore;

  return {
    theme,
    enableSso,
    isSsoEnabled,
    openConfirmationDisableModal,
    ssoToggle,
    confirmationDisableModal,
  };
})(observer(ToggleSSO));
