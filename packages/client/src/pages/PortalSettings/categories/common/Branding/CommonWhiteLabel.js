import React from "react";
import { observer } from "mobx-react";

import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import Button from "@docspace/components/button";
import Badge from "@docspace/components/badge";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";

import WhiteLabelWrapper from "./StyledWhitelabel";
import Logo from "./sub-components/logo";

const CommonWhiteLabel = ({
  isSettingPaid,
  logoTextWhiteLabel,
  onChangeCompanyName,
  onUseTextAsLogo,
  logoUrlsWhiteLabel,
  onChangeLogo,
  onSave,
  onRestoreDefault,
  isEqualLogo,
  isEqualText,
  isSaving,
}) => {
  const { t } = useTranslation(["Settings", "Profile", "Common"]);

  return (
    <WhiteLabelWrapper>
      <Text className="subtitle">{t("BrandingSubtitle")}</Text>

      <div className="header-container">
        <Text fontSize="16px" fontWeight="700">
          {t("WhiteLabel")}
        </Text>
        {!isSettingPaid && (
          <Badge
            className="paid-badge"
            backgroundColor="#EDC409"
            label={t("Common:Paid")}
            isPaidBadge={true}
          />
        )}
      </div>
      <Text className="wl-subtitle settings_unavailable" fontSize="12px">
        {t("WhiteLabelSubtitle")}
      </Text>

      <div className="wl-helper">
        <Text className="settings_unavailable">{t("WhiteLabelHelper")}</Text>
        <HelpButton
          tooltipContent={<Text fontSize="12px">{t("WhiteLabelTooltip")}</Text>}
          place="right"
          offsetRight={0}
          className="settings_unavailable"
        />
      </div>
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerCompanyName"
          labelText={t("Common:CompanyName")}
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
            maxLength={30}
          />
          <Button
            id="btnUseAsLogo"
            className="use-as-logo"
            size="small"
            label={t("UseAsLogoButton")}
            onClick={onUseTextAsLogo}
            tabIndex={2}
            isDisabled={!isSettingPaid}
          />
        </FieldContainer>
      </div>

      <div className="logos-container">
        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoLightSmall")} ({logoUrlsWhiteLabel[0].size.width}x
            {logoUrlsWhiteLabel[0].size.height})
          </Text>
          <div className="logos-wrapper">
            <Logo
              title={t("Profile:LightTheme")}
              src={logoUrlsWhiteLabel[0].path.light}
              imageClass="logo-header background-light"
              inputId="logoUploader_1_light"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
            <Logo
              title={t("Profile:DarkTheme")}
              src={logoUrlsWhiteLabel[0].path.dark}
              imageClass="logo-header background-dark"
              inputId="logoUploader_1_dark"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
          </div>
        </div>

        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoCompact")} ({logoUrlsWhiteLabel[5].size.width}x
            {logoUrlsWhiteLabel[5].size.height})
          </Text>
          <div className="logos-wrapper">
            <Logo
              title={t("Profile:LightTheme")}
              src={logoUrlsWhiteLabel[5].path.light}
              imageClass="border-img logo-compact background-light"
              inputId="logoUploader_6_light"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
            <Logo
              title={t("Profile:DarkTheme")}
              src={logoUrlsWhiteLabel[5].path.dark}
              imageClass="border-img logo-compact background-dark"
              inputId="logoUploader_6_dark"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
          </div>
        </div>

        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoLogin")} ({logoUrlsWhiteLabel[1].size.width}x
            {logoUrlsWhiteLabel[1].size.height})
          </Text>
          <div className="logos-login-wrapper">
            <Logo
              title={t("Profile:LightTheme")}
              src={logoUrlsWhiteLabel[1].path.light}
              imageClass="border-img logo-big background-white"
              inputId="logoUploader_2_light"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
            <Logo
              title={t("Profile:DarkTheme")}
              src={logoUrlsWhiteLabel[1].path.dark}
              imageClass="border-img logo-big background-dark"
              inputId="logoUploader_2_dark"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
          </div>
        </div>

        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoAbout")} ({logoUrlsWhiteLabel[6].size.width}x
            {logoUrlsWhiteLabel[6].size.height})
          </Text>
          <div className="logos-wrapper">
            <Logo
              title={t("Profile:LightTheme")}
              src={logoUrlsWhiteLabel[6].path.light}
              imageClass="border-img logo-about background-white"
              inputId="logoUploader_7_light"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
            <Logo
              title={t("Profile:DarkTheme")}
              src={logoUrlsWhiteLabel[6].path.dark}
              imageClass="border-img logo-about background-dark"
              inputId="logoUploader_7_dark"
              onChangeText={t("ChangeLogoButton")}
              onChange={onChangeLogo}
              isSettingPaid={isSettingPaid}
            />
          </div>
        </div>

        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoFavicon")} ({logoUrlsWhiteLabel[2].size.width}x
            {logoUrlsWhiteLabel[2].size.height})
          </Text>
          <Logo
            src={logoUrlsWhiteLabel[2].path.light}
            imageClass="border-img logo-favicon"
            inputId="logoUploader_3_light"
            onChangeText={t("ChangeLogoButton")}
            onChange={onChangeLogo}
            isSettingPaid={isSettingPaid}
          />
        </div>

        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoDocsEditor")} ({logoUrlsWhiteLabel[3].size.width}x
            {logoUrlsWhiteLabel[3].size.height})
          </Text>
          <Logo
            isEditor={true}
            src={logoUrlsWhiteLabel[3].path.light}
            inputId="logoUploader_4_light"
            onChangeText={t("ChangeLogoButton")}
            onChange={onChangeLogo}
            isSettingPaid={isSettingPaid}
          />
        </div>

        <div className="logo-wrapper">
          <Text
            fontSize="15px"
            fontWeight="600"
            className="settings_unavailable"
          >
            {t("LogoDocsEditorEmbedded")} ({logoUrlsWhiteLabel[4].size.width}x
            {logoUrlsWhiteLabel[4].size.height})
          </Text>
          <Logo
            src={logoUrlsWhiteLabel[4].path.light}
            imageClass="border-img logo-embedded-editor background-white"
            inputId="logoUploader_5_light"
            onChangeText={t("ChangeLogoButton")}
            onChange={onChangeLogo}
            isSettingPaid={isSettingPaid}
          />
        </div>
      </div>

      <SaveCancelButtons
        tabIndex={3}
        className="save-cancel-buttons"
        onSaveClick={onSave}
        onCancelClick={onRestoreDefault}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("RestoreDefaultButton")}
        displaySettings={true}
        showReminder={isSettingPaid}
        saveButtonDisabled={isEqualLogo && isEqualText}
        isSaving={isSaving}
      />
    </WhiteLabelWrapper>
  );
};

export default observer(CommonWhiteLabel);
