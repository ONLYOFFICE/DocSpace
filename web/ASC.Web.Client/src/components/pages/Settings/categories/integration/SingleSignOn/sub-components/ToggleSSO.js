import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import Text from "@appserver/components/text";
import ToggleButton from "@appserver/components/toggle-button";

import DisableSsoConfirmationModal from "./DisableSsoConfirmationModal";

const borderProp = { radius: "4px" };

const ToggleSSO = () => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <>
      <Text className="intro-text" lineHeight="20px" color="#657077">
        {t("SsoIntro")}
      </Text>

      <Box
        backgroundProp="#F8F9F9 "
        borderProp={borderProp}
        displayProp="flex"
        flexDirection="row"
        paddingProp="12px"
      >
        <ToggleButton
          className="toggle"
          isChecked={FormStore.enableSso}
          onChange={
            FormStore.isSsoEnabled && FormStore.enableSso
              ? FormStore.openConfirmationDisableModal
              : FormStore.onSsoToggle
          }
        />

        <Box>
          <Text as="span" fontWeight={600} lineHeight="20px">
            {t("TurnOnSSO")}
          </Text>
          <Text lineHeight="16px">{t("TurnOnSSOCaption")}</Text>
        </Box>
      </Box>

      {FormStore.confirmationDisableModal && <DisableSsoConfirmationModal />}
    </>
  );
};

export default observer(ToggleSSO);
