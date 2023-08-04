import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import Box from "@docspace/components/box";
//import FormStore from "@docspace/studio/src/store/SsoFormStore";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import Badge from "@docspace/components/badge";
import DisableSsoConfirmationModal from "./DisableSsoConfirmationModal";
import SSOLoader from "../../sub-components/ssoLoader";
const borderProp = { radius: "6px" };

const ToggleSSO = (props) => {
  const {
    theme,
    enableSso,
    isSsoEnabled,
    openConfirmationDisableModal,
    ssoToggle,
    confirmationDisableModal,
    isSSOAvailable,
    tReady,
    t,
  } = props;

  if (!tReady) {
    return <SSOLoader isToggleSSO={true} />;
  }

  return (
    <>
      <Text
        className="intro-text settings_unavailable"
        lineHeight="20px"
        noSelect
      >
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
          className="enable-sso toggle"
          isChecked={enableSso}
          onChange={
            isSsoEnabled && enableSso ? openConfirmationDisableModal : ssoToggle
          }
          isDisabled={!isSSOAvailable}
        />

        <div className="toggle-caption">
          <div className="toggle-caption_title">
            <Text
              fontWeight={600}
              lineHeight="20px"
              noSelect
              className="settings_unavailable"
            >
              {t("TurnOnSSO")}
            </Text>
            {!isSSOAvailable && (
              <Badge
                backgroundColor="#EDC409"
                label={t("Common:Paid")}
                className="toggle-caption_title_badge"
                isPaidBadge={true}
              />
            )}
          </div>
          <Text
            fontSize="12px"
            fontWeight={400}
            lineHeight="16px"
            className="settings_unavailable"
            noSelect
          >
            {t("TurnOnSSOCaption")}
          </Text>
        </div>
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
})(withTranslation(["SingleSignOn"])(observer(ToggleSSO)));
