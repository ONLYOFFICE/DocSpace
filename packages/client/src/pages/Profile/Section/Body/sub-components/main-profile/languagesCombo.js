import React from "react";
import styled from "styled-components";

import { inject, observer } from "mobx-react";
import ComboBox from "@docspace/components/combobox";
import Text from "@docspace/components/text";
import { convertLanguage } from "@docspace/common/utils";
import { isMobileOnly } from "react-device-detect";
import toastr from "client/toastr";

import { smallTablet } from "@docspace/components/utils/device";

const StyledRow = styled.div`
  display: flex;
  gap: 24px;

  .lang-combo {
    & > div {
      padding: 0 !important;
    }
  }

  @media ${smallTablet} {
    width: 100%;
    flex-direction: column;
    gap: 4px;

    .lang-combo {
      & > div {
        padding-left: 8px !important;
      }
    }
  }
`;

const LanguagesCombo = (props) => {
  const {
    t,
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
    <StyledRow>
      <Text as="div" color="#A3A9AE" className="label">
        {t("Common:Language")}
      </Text>
      <ComboBox
        className="lang-combo"
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
        manualWidth="320px"
        isDefaultMode={!isMobileOnly}
        withBlur={isMobileOnly}
        fillIcon={false}
      />
    </StyledRow>
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
