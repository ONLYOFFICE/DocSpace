import React from "react";
import { inject, observer } from "mobx-react";
import ComboBox from "@docspace/components/combobox";
import { convertLanguage } from "@docspace/common/utils";
import { isMobileOnly } from "react-device-detect";
import toastr from "client/toastr";

const LanguagesCombo = (props) => {
  const {
    profile,
    updateProfileCulture,
    setIsLoading,
    cultureName,
    currentCulture,
    culture,
    cultureNames,
  } = props;

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

  return (
    <ComboBox
      className="lang-combo"
      directionY="both"
      options={cultureNames}
      selectedOption={selectedLanguage}
      onSelect={onLanguageSelect}
      isDisabled={false}
      noBorder={true}
      scaled={false}
      scaledOptions={false}
      size="content"
      showDisabledItems={true}
      dropDownMaxHeight={364}
      manualWidth="320px"
      isDefaultMode={!isMobileOnly}
      withBlur={isMobileOnly}
      fillIcon={false}
    />
  );
};

export default inject(({ auth, peopleStore }) => {
  const { loadingStore, targetUserStore } = peopleStore;
  const { settingsStore } = auth;

  const { setIsLoading } = loadingStore;
  const { updateProfileCulture } = targetUserStore;

  const { culture } = settingsStore;

  return {
    setIsLoading,
    updateProfileCulture,
    culture,
  };
})(observer(LanguagesCombo));
