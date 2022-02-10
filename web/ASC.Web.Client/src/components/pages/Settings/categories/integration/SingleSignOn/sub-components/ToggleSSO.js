import React from "react";
import Box from "@appserver/components/box";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";
import ToggleButton from "@appserver/components/toggle-button";
import { observer } from "mobx-react";

const ToggleSSO = ({ FormStore, t }) => {
  return (
    <Box
      backgroundProp="#F8F9F9 "
      borderProp={{ radius: "4px" }}
      displayProp="flex"
      flexDirection="row"
      paddingProp="12px"
    >
      <ToggleButton
        className="toggle"
        isChecked={FormStore.enableSso}
        onChange={FormStore.onSsoToggle}
      />

      <Box>
        <Box alignItems="center" displayProp="flex" flexDirection="row">
          <Text as="span" fontWeight={600} lineHeight="20px">
            {t("TurnOnSSO")}
            <HelpButton
              offsetRight={0}
              style={{ display: "inline-flex" }}
              tooltipContent={t("TurnOnSSOTooltip")}
            />
          </Text>
        </Box>

        <Text lineHeight="16px">{t("TurnOnSSOCaption")}</Text>
      </Box>
    </Box>
  );
};

export default observer(ToggleSSO);
