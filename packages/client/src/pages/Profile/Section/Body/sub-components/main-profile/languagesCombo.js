import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { Trans } from "react-i18next";

import ComboBox from "@docspace/components/combobox";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import HelpButton from "@docspace/components/help-button";
import toastr from "@docspace/components/toast/toastr";

import { convertLanguage } from "@docspace/common/utils";
import withCultureNames from "@docspace/common/hoc/withCultureNames";

import { isMobileOnly } from "react-device-detect";

import { StyledRow } from "./styled";

const LanguagesCombo = (props) => {
  const {
    t,
    profile,
    updateProfileCulture,
    setIsLoading,
    culture,
    cultureNames,
    helpLink,
    theme,
  } = props;
  const { cultureName, currentCulture } = profile;

  const language = convertLanguage(cultureName || currentCulture || culture);
  const selectedLanguage = cultureNames.find((item) => item.key === language) ||
    cultureNames.find((item) => item.key === culture) || {
      key: language,
      label: "",
    };

  const onLanguageSelect = (language) => {
    console.log("onLanguageSelect", language);

    if (profile.cultureName === language.key) return;

    setIsLoading(true);
    updateProfileCulture(profile.id, language.key)
      .then(() => setIsLoading(false))
      .then(() => location.reload())
      .catch((error) => {
        toastr.error(error && error.message ? error.message : error);
        setIsLoading(false);
      });
  };

  const supportEmail = "documentation@onlyoffice.com";

  const tooltipLanguage = (
    <Text fontSize="13px">
      <Trans t={t} i18nKey="NotFoundLanguage" ns="Common">
        "In case you cannot find your language in the list of the available
        ones, feel free to write to us at
        <Link
          href={`mailto:${supportEmail}`}
          isHovered={true}
          color={theme.profileInfo.tooltipLinkColor}
        >
          {{ supportEmail }}
        </Link>
        to take part in the translation and get up to 1 year free of charge."
      </Trans>{" "}
      <Link
        color={theme.profileInfo.tooltipLinkColor}
        isHovered={true}
        href={`${helpLink}/guides/become-translator.aspx`}
        target="_blank"
      >
        {t("Common:LearnMore")}
      </Link>
    </Text>
  );

  return (
    <StyledRow>
      <Text as="div" color="#A3A9AE" className="label">
        {t("Common:Language")}
        <HelpButton
          size={12}
          offsetRight={0}
          place="right"
          tooltipContent={tooltipLanguage}
        />
      </Text>
      <ComboBox
        className="combo"
        directionY="both"
        options={cultureNames}
        selectedOption={selectedLanguage}
        onSelect={onLanguageSelect}
        isDisabled={false}
        noBorder={!isMobileOnly}
        scaled={isMobileOnly}
        scaledOptions={false}
        size="content"
        showDisabledItems={true}
        dropDownMaxHeight={364}
        manualWidth="208px"
        isDefaultMode={!isMobileOnly}
        withBlur={isMobileOnly}
        fillIcon={false}
      />
    </StyledRow>
  );
};

export default withCultureNames(
  inject(({ auth, peopleStore }) => {
    const { loadingStore, targetUserStore } = peopleStore;
    const { settingsStore } = auth;

    const { setIsLoading } = loadingStore;
    const { updateProfileCulture, targetUser: profile } = targetUserStore;

    const { culture, helpLink, theme } = settingsStore;

    return {
      setIsLoading,
      updateProfileCulture,
      culture,
      profile,
      helpLink,
      theme,
    };
  })(observer(LanguagesCombo))
);
