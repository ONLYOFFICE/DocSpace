import React from "react";
import styled from "styled-components";
import { useTranslation } from "react-i18next";

import ComboBox from "@docspace/components/combobox";
import Text from "@docspace/components/text";
import toastr from "@docspace/components/toast/toastr";

import { isMobileOnly } from "react-device-detect";

const StyledRow = styled.div`
  display: flex;
  gap: 24px;

  .label {
    min-width: 75px;
    max-width: 75px;
    white-space: nowrap;
  }

  .lang-combo {
    & > div {
      padding: 0 !important;
    }
  }
`;

const TimezoneCombo = () => {
  const { t } = useTranslation("Settings");

  const timezones = [{ key: "03", label: "(UTC) +03 Moscow" }];
  const selectedTimezone = { key: "03", label: "(UTC) +03 Moscow" };

  return (
    <StyledRow>
      <Text as="div" color="#A3A9AE" className="label">
        {t("Settings:Timezone")}
      </Text>
      <ComboBox
        onClick={() => toastr.warning("Work in progress (timezones)")}
        className="lang-combo"
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
        manualWidth="320px"
        isDefaultMode={!isMobileOnly}
        withBlur={isMobileOnly}
        fillIcon={false}
      />
    </StyledRow>
  );
};

export default TimezoneCombo;
