import CombinedShapeSvgUrl from "PUBLIC_DIR/images/combined.shape.svg?url";
import React from "react";
import { withTranslation } from "react-i18next";
import FieldContainer from "@docspace/components/field-container";
import ComboBox from "@docspace/components/combobox";
import toastr from "@docspace/components/toast/toastr";
import HelpButton from "@docspace/components/help-button";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { inject, observer } from "mobx-react";
import { LANGUAGE, COOKIE_EXPIRATION_YEAR } from "@docspace/common/constants";
import { LanguageTimeSettingsTooltip } from "../sub-components/common-tooltips";
import { combineUrl, setCookie } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import { useNavigate } from "react-router-dom";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@docspace/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";

const mapTimezonesToArray = (timezones) => {
  return timezones.map((timezone) => {
    return { key: timezone.id, label: timezone.displayName };
  });
};

const mapCulturesToArray = (cultures, i18n) => {
  const t = i18n.getFixedT(null, "Common");
  return cultures.map((culture) => {
    return { key: culture, label: t(`Culture_${culture}`) };
  });
};

const findSelectedItemByKey = (items, selectedItemKey) => {
  return items.find((item) => item.key === selectedItemKey);
};

let languageFromSessionStorage = "";
let languageDefaultFromSessionStorage = "";
let timezoneFromSessionStorage = "";
let timezoneDefaultFromSessionStorage = "";

const settingNames = ["language", "timezone"];

const LanguageAndTimeZone = (props) => {
  const {
    i18n,
    language,
    cultureNames,
    rawTimezones,
    portalTimeZoneId,
    isLoaded,
    cultures,
    portalLanguage,
    tReady,
    setIsLoadedLngTZSettings,
    t,
    setIsLoaded,
    timezone,

    initSettings,
  } = props;

  const navigate = useNavigate();

  const [state, setState] = React.useState({
    isLoading: false,
    timezone: "",
    timezoneDefault: "",
    language: "",
    languageDefault: "",
    hasChanged: false,
    showReminder: false,
    hasScroll: false,
    isCustomizationView: false,
  });

  const prevProps = React.useRef({ language: "", tReady: "", isLoaded: "" });
  const prevState = React.useRef({ language: "", timezone: "" });

  React.useEffect(() => {
    languageFromSessionStorage = getFromSessionStorage("language");
    languageDefaultFromSessionStorage =
      getFromSessionStorage("languageDefault");
    timezoneFromSessionStorage = getFromSessionStorage("timezone");
    timezoneDefaultFromSessionStorage =
      getFromSessionStorage("timezoneDefault");

    setDocumentTitle(t("StudioTimeLanguageSettings"));

    if (!isLoaded) initSettings().then(() => setIsLoaded(true));

    const isLoadedSetting =
      isLoaded &&
      tReady &&
      timezoneFromSessionStorage &&
      languageFromSessionStorage;

    if (isLoadedSetting) {
      setIsLoadedLngTZSettings(isLoadedSetting);
    }
    checkInnerWidth();
    window.addEventListener("resize", checkInnerWidth);

    if (
      rawTimezones.length > 0 &&
      isLoaded &&
      tReady &&
      timezoneFromSessionStorage === ""
    ) {
      const timezones = mapTimezonesToArray(rawTimezones);

      const timezone =
        timezoneFromSessionStorage ||
        findSelectedItemByKey(timezones, portalTimeZoneId) ||
        rawTimezones[0];

      const timezoneDefault =
        findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];

      setState((val) => ({
        ...val,
        timezone,
        timezoneDefault,
      }));
    }

    if (
      cultures.length > 0 &&
      isLoaded &&
      tReady &&
      languageFromSessionStorage === ""
    ) {
      const language =
        languageFromSessionStorage ||
        findSelectedItemByKey(cultureNames, portalLanguage) ||
        cultureNames[0];

      const languageDefault =
        findSelectedItemByKey(cultureNames, portalLanguage) || cultureNames[0];
      setState((val) => ({
        ...val,
        language,
        languageDefault,
      }));
    }

    if (!languageDefaultFromSessionStorage) {
      setState((val) => ({
        ...val,
        languageDefault: languageFromSessionStorage,
      }));
    }

    if (timezoneDefaultFromSessionStorage || timezone) {
      checkChanges();
    }

    if (languageDefaultFromSessionStorage || language) {
      checkChanges();
    }
    return () => {
      window.removeEventListener("resize", checkInnerWidth);
    };
  }, []);

  React.useState(() => {
    prevProps.current = {
      language: language,
      tReady: tReady,
      isLoaded: isLoaded,
    };
  }, [language, tReady, isLoaded]);

  React.useState(() => {
    prevState.current = { language: state.language, timezone: state.timezone };
  }, [state.language, state.timezone]);

  React.useEffect(() => {
    const { timezoneDefault, languageDefault, hasScroll } = state;

    if (
      isLoaded !== prevProps.current.isLoaded ||
      tReady !== prevProps.current.tReady ||
      state.language !== prevState.current.language ||
      state.timezone !== prevState.current.timezone
    ) {
      const isLoadedSetting =
        isLoaded && tReady && state.timezone && state.language;

      if (isLoadedSetting) {
        setIsLoadedLngTZSettings(isLoadedSetting);
      }
    }

    if (
      rawTimezones.length > 0 &&
      isLoaded &&
      tReady &&
      state.timezone === ""
    ) {
      const timezones = mapTimezonesToArray(rawTimezones);

      const timezone =
        timezoneFromSessionStorage ||
        findSelectedItemByKey(timezones, portalTimeZoneId) ||
        rawTimezones[0];

      const timezoneDefault =
        findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];

      setState((val) => ({ ...val, timezone, timezoneDefault }));
    }

    if (cultures.length > 0 && isLoaded && tReady && state.language === "") {
      const cultureNames = mapCulturesToArray(cultures, i18n);
      const language =
        languageFromSessionStorage ||
        findSelectedItemByKey(cultureNames, portalLanguage) ||
        cultureNames[0];

      const languageDefault =
        findSelectedItemByKey(cultureNames, portalLanguage) || cultureNames[0];

      setState((val) => ({ ...val, language, languageDefault }));
    }

    const checkScroll = checkScrollSettingsBlock();

    window.addEventListener("resize", checkScroll);
    const scrollLngTZSettings = checkScroll();

    if (scrollLngTZSettings !== hasScroll) {
      setState((val) => ({ ...val, hasScroll: scrollLngTZSettings }));
    }

    // TODO: Remove div with height 64 and remove settings-mobile class
    const settingsMobile =
      document.getElementsByClassName("settings-mobile")[0];

    if (settingsMobile) {
      settingsMobile.style.display = "none";
    }

    if (language !== prevProps.current.language) {
      i18n.changeLanguage(language).then(() => {
        const newLocaleSelectedLanguage =
          findSelectedItemByKey(cultureNames, state.language.key) ||
          cultureNames[0];
        setState((val) => ({
          ...val,
          language: languageFromSessionStorage || newLocaleSelectedLanguage,
        }));
      });
    }
    if (timezoneDefault && languageDefault) {
      checkChanges();
    }
  }, [
    state.timezoneDefault,
    state.languageDefault,
    state.hasScroll,
    state.timezone,
    state.language,
    i18n,
    language,
    cultureNames,
    rawTimezones,
    portalTimeZoneId,
    isLoaded,
    cultures,
    portalLanguage,
    tReady,
    setIsLoadedLngTZSettings,

    timezone,

    initSettings,
  ]);

  const onLanguageSelect = (language) => {
    setState((val) => ({ ...val, language }));
    if (settingIsEqualInitialValue("language", language)) {
      saveToSessionStorage("language", "");
      saveToSessionStorage("languageDefault", "");
    } else {
      saveToSessionStorage("language", language);
    }
    checkChanges();
  };

  const onTimezoneSelect = (timezone) => {
    setState((val) => ({ ...val, timezone }));
    if (settingIsEqualInitialValue("timezone", timezone)) {
      saveToSessionStorage("timezone", "");
      saveToSessionStorage("timezoneDefault", "");
    } else {
      saveToSessionStorage("timezone", timezone);
    }

    checkChanges();
  };

  const onSaveLngTZSettings = () => {
    const { t, setLanguageAndTime, user, language: lng } = props;
    const { language, timezone } = state;

    setState((val) => ({ ...val, isLoading: true }));
    setLanguageAndTime(language.key, timezone.key)
      .then(
        () =>
          !user.cultureName &&
          setCookie(LANGUAGE, language.key || "en", {
            "max-age": COOKIE_EXPIRATION_YEAR,
          })
      )
      .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
      .then(
        () => !user.cultureName && lng !== language.key && location.reload()
      )
      .catch((error) => toastr.error(error))
      .finally(() => setState((val) => ({ ...val, isLoading: false })));

    setState((val) => ({
      ...val,
      showReminder: false,
      timezoneDefault: state.timezone,
      languageDefault: state.language,
    }));

    saveToSessionStorage("languageDefault", language);
    saveToSessionStorage("timezoneDefault", timezone);
  };

  const onCancelClick = () => {
    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage !== "none" &&
        valueFromSessionStorage !== null &&
        !settingIsEqualInitialValue(settingName, valueFromSessionStorage)
      ) {
        const defaultValue = state[settingName + "Default"];

        setState((val) => ({ ...val, [settingName]: defaultValue || null }));
        saveToSessionStorage(settingName, "");
      }
    });

    setState((val) => ({ ...val, showReminder: false }));

    checkChanges();
  };

  const settingIsEqualInitialValue = (settingName, value) => {
    const defaultValue = JSON.stringify(state[settingName + "Default"]);
    const currentValue = JSON.stringify(value);
    return defaultValue === currentValue;
  };

  const checkChanges = () => {
    let hasChanged = false;

    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage &&
        !settingIsEqualInitialValue(settingName, valueFromSessionStorage)
      )
        hasChanged = true;
    });

    if (hasChanged !== state.hasChanged) {
      setState((val) => ({
        ...val,
        hasChanged: hasChanged,
        showReminder: hasChanged,
      }));
    }
  };

  const checkInnerWidth = () => {
    if (!isSmallTablet()) {
      setState((val) => ({ ...val, isCustomizationView: true }));

      const currentUrl = window.location.href.replace(
        window.location.origin,
        ""
      );

      const newUrl = "/portal-settings/customization/general";

      if (newUrl === currentUrl) return;

      navigate(newUrl);
    } else {
      setState((val) => ({ ...val, isCustomizationView: false }));
    }
  };

  const onClickLink = (e) => {
    e.preventDefault();

    navigate(e.target.pathname);
  };

  const {
    theme,
    isMobileView,

    isLoadedPage,
    helpLink,
    organizationName,
    currentColorScheme,
  } = props;

  const {
    isLoading,

    showReminder,
    hasScroll,
    isCustomizationView,
  } = state;

  const timezones = mapTimezonesToArray(rawTimezones);
  const cultureNamesNew = mapCulturesToArray(cultures, i18n);

  const tooltipLanguageTimeSettings = (
    <LanguageTimeSettingsTooltip
      theme={theme}
      t={t}
      helpLink={helpLink}
      organizationName={organizationName}
      currentColorScheme={currentColorScheme}
    />
  );

  const settingsBlock = !(state.language && state.timezone) ? null : (
    <div className="settings-block">
      <FieldContainer
        id="fieldContainerLanguage"
        labelText={`${t("Common:Language")}`}
        isVertical={true}
      >
        <ComboBox
          tabIndex={1}
          id="comboBoxLanguage"
          options={cultureNamesNew}
          selectedOption={state.language}
          onSelect={onLanguageSelect}
          isDisabled={isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={300}
          className="dropdown-item-width combo-box-settings"
          showDisabledItems={true}
        />
      </FieldContainer>
      <FieldContainer
        id="fieldContainerTimezone"
        labelText={`${t("TimeZone")}`}
        isVertical={true}
      >
        <ComboBox
          tabIndex={2}
          id="comboBoxTimezone"
          options={timezones}
          selectedOption={state.timezone}
          onSelect={onTimezoneSelect}
          isDisabled={isLoading}
          noBorder={false}
          scaled={true}
          scaledOptions={true}
          dropDownMaxHeight={300}
          className="dropdown-item-width combo-box-settings"
          showDisabledItems={true}
        />
      </FieldContainer>
    </div>
  );

  return !isLoadedPage ? (
    <LoaderCustomization lngTZSettings={true} />
  ) : (
    <StyledSettingsComponent
      hasScroll={hasScroll}
      className="category-item-wrapper"
    >
      {isCustomizationView && !isMobileView && (
        <div className="category-item-heading">
          <div className="category-item-title">
            {t("StudioTimeLanguageSettings")}
          </div>
          <HelpButton
            className="language-time-zone-help-button"
            offsetRight={0}
            iconName={CombinedShapeSvgUrl}
            size={12}
            tooltipContent={tooltipLanguageTimeSettings}
          />
        </div>
      )}
      {(isMobileOnly && isSmallTablet()) || isSmallTablet() ? (
        <StyledScrollbar stype="mediumBlack">{settingsBlock}</StyledScrollbar>
      ) : (
        <> {settingsBlock}</>
      )}
      <SaveCancelButtons
        tabIndex={3}
        className="save-cancel-buttons"
        onSaveClick={onSaveLngTZSettings}
        onCancelClick={onCancelClick}
        showReminder={showReminder}
        reminderTest={t("YouHaveUnsavedChanges")}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Common:CancelButton")}
        displaySettings={true}
        hasScroll={hasScroll}
        additionalClassSaveButton="language-time-zone-save"
        additionalClassCancelButton="language-time-zone-cancel"
      />
    </StyledSettingsComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const {
    culture,
    timezone,
    timezones,
    nameSchemaId,
    organizationName,
    greetingSettings,
    getPortalTimezones,
    cultures,
    helpLink,
    currentColorScheme,
  } = auth.settingsStore;

  const { user } = auth.userStore;

  const { setLanguageAndTime } = setup;
  const { isLoaded, setIsLoadedLngTZSettings, initSettings, setIsLoaded } =
    common;
  return {
    theme: auth.settingsStore.theme,
    user,
    portalLanguage: culture,
    portalTimeZoneId: timezone,
    language: culture,
    rawTimezones: timezones,
    greetingSettings,
    nameSchemaId,
    organizationName,
    setLanguageAndTime,
    getPortalTimezones,
    isLoaded,
    setIsLoadedLngTZSettings,
    cultures,
    helpLink,
    initSettings,
    setIsLoaded,
    currentColorScheme,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(LanguageAndTimeZone))
  )
);
