import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";
import ToggleButton from "@appserver/components/toggle-button";

const borderProp = { radius: "4px" };
const displayProp = { display: "inline-flex" };

const ToggleSSO = ({ FormStore, t }) => {
  return (
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
        onChange={FormStore.onSsoToggle}
      />

      <Box>
        <Box alignItems="center" displayProp="flex" flexDirection="row">
          <Text as="span" fontWeight={600} lineHeight="20px">
            {t("TurnOnSSO")}
            <HelpButton
              offsetRight={0}
              style={displayProp}
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
