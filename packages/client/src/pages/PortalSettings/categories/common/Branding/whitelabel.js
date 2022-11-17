import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import Badge from "@docspace/components/badge";

import WhiteLabelWrapper from "./StyledWhitelabel";

const WhiteLabel = (props) => {
  const { t, isSettingPaid, logoText } = props;
  const [logoTextWhiteLabel, setLogoTextWhiteLabel] = useState("");

  useEffect(() => {
    if (logoText) {
      setLogoTextWhiteLabel(logoText);
    }
  }, [logoText]);

  const onChangeCompanyName = (e) => {
    const value = e.target.value;
    setLogoTextWhiteLabel(value);
  };

  return (
    <WhiteLabelWrapper>
      <Text className="subtitle" color="#657077">
        {t("BrandingSubtitle")}
      </Text>
      <div className="header-container">
        <Text fontSize="16px" fontWeight="700">
          {t("WhiteLabel")}
        </Text>
        {!isSettingPaid && <Badge backgroundColor="#EDC409" label="Paid" />}
      </div>
      <Text className="wl-subtitle settings_unavailable" fontSize="12px">
        {t("WhiteLabelSubtitle")}
      </Text>

      <div className="wl-helper">
        <Text className="settings_unavailable">{t("WhiteLabelHelper")}</Text>
        <HelpButton
          tooltipContent={t("WhiteLabelTooltip")}
          place="right"
          offsetRight={0}
          className="settings_unavailable"
        />
      </div>
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerCompanyName"
          labelText={t("CompanyNameForCanvasLogo")}
          isVertical={true}
          className="settings_unavailable"
        >
          <TextInput
            className="input"
            value={logoTextWhiteLabel}
            onChange={onChangeCompanyName}
            isDisabled={!isSettingPaid}
            isReadOnly={!isSettingPaid}
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
          />
          <Button
            id="btnUseAsLogo"
            className="use-as-logo"
            size="small"
            label={t("UseAsLogoButton")}
            //onClick={onUseTextAsLogo}
            tabIndex={2}
            isDisabled={!isSettingPaid}
          />
        </FieldContainer>
      </div>
    </WhiteLabelWrapper>
  );
};

export default inject(({ setup, auth, common }) => {
  const { setWhiteLabelSettings } = setup;

  const {
    whiteLabelLogoSizes,
    whiteLabelLogoText,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    whiteLabelLogoUrls,
    restoreWhiteLabelSettings,
  } = common;

  const { getWhiteLabelLogoUrls } = auth.settingsStore;

  return {
    theme: auth.settingsStore.theme,
    logoText: whiteLabelLogoText,
    logoSizes: whiteLabelLogoSizes,
    logoUrls: whiteLabelLogoUrls,
    getWhiteLabelLogoText,
    getWhiteLabelLogoSizes,
    getWhiteLabelLogoUrls,
    setWhiteLabelSettings,
    restoreWhiteLabelSettings,
  };
})(withTranslation(["Settings", "Common"])(observer(WhiteLabel)));
