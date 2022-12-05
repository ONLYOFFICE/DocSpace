import React from "react";
import { useTranslation } from "react-i18next";

import ComboBox from "@docspace/components/combobox";
import Text from "@docspace/components/text";
import toastr from "@docspace/components/toast/toastr";

import { isMobileOnly } from "react-device-detect";

import { StyledRow } from "./styled-main-profile";

const TimezoneCombo = ({title}) => {
  const { t } = useTranslation("Wizard");

  const timezones = [{ key: "03", label: "(UTC) +03 Moscow" }];
  const selectedTimezone = { key: "03", label: "(UTC) +03 Moscow" };

  return (
    <StyledRow title={title}>
      <Text as="div" color="#A3A9AE" className="label">
        {t("Wizard:Timezone")}
      </Text>
      <ComboBox
        onClick={() => toastr.warning("Work in progress (timezones)")}
        className="combo"
        directionY="both"
        options={timezones}
        selectedOption={selectedTimezone}
        //onSelect={onTimezoneSelect}
        isDisabled={false}
        noBorder={!isMobileOnly}
        scaled={isMobileOnly}
        scaledOptions={false}
        size="content"
        showDisabledItems={true}
        dropDownMaxHeight={364}
        manualWidth="250px"
        isDefaultMode={!isMobileOnly}
        withBlur={isMobileOnly}
        fillIcon={false}
      />
    </StyledRow>
  );
};

export default TimezoneCombo;
