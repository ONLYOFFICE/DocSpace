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
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import history from "@docspace/common/history";
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

class LanguageAndTimeZone extends React.Component {
  constructor(props) {
    super(props);

    const { t } = props;

    languageFromSessionStorage = getFromSessionStorage("language");
    languageDefaultFromSessionStorage = getFromSessionStorage(
      "languageDefault"
    );
    timezoneFromSessionStorage = getFromSessionStorage("timezone");
    timezoneDefaultFromSessionStorage = getFromSessionStorage(
      "timezoneDefault"
    );

    setDocumentTitle(t("StudioTimeLanguageSettings"));

    this.state = {
      isLoading: false,
      timezone: timezoneFromSessionStorage || "",
      timezoneDefault: timezoneDefaultFromSessionStorage || "",
      language: languageFromSessionStorage || "",
      languageDefault: languageDefaultFromSessionStorage || "",
      hasChanged: false,
      showReminder: false,
      hasScroll: false,
      isCustomizationView: false,
    };
  }

  componentDidMount() {
    const { languageDefault, timezoneDefault } = this.state;

    const {
      i18n,
      language,
      rawTimezones,
      portalTimeZoneId,
      isLoaded,
      cultures,
      portalLanguage,
      tReady,
      setIsLoadedLngTZSettings,
    } = this.props;

    const isLoadedSetting = isLoaded && tReady;

    if (isLoadedSetting) {
      setIsLoadedLngTZSettings(isLoadedSetting);
    }
    this.checkInnerWidth();
    window.addEventListener("resize", this.checkInnerWidth);

    if (
      rawTimezones.length > 0 &&
      isLoaded === true &&
      tReady === true &&
      this.state.timezone === ""
    ) {
      const timezones = mapTimezonesToArray(rawTimezones);

      const timezone =
        timezoneFromSessionStorage ||
        findSelectedItemByKey(timezones, portalTimeZoneId) ||
        rawTimezones[0];

      const timezoneDefault =
        findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];

      this.setState({
        timezone,
        timezoneDefault,
      });
    }

    if (
      cultures.length > 0 &&
      isLoaded === true &&
      tReady === true &&
      this.state.language === ""
    ) {
      const cultureNames = mapCulturesToArray(cultures, i18n);

      const language =
        languageFromSessionStorage ||
        findSelectedItemByKey(cultureNames, portalLanguage) ||
        cultureNames[0];

      const languageDefault =
        findSelectedItemByKey(cultureNames, portalLanguage) || cultureNames[0];

      this.setState({
        language,
        languageDefault,
      });
    }

    if (!languageDefault) {
      this.setState({
        languageDefault: language,
      });
    }

    if (timezoneDefault && languageDefault) {
      this.checkChanges();
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const { timezoneDefault, languageDefault, hasScroll } = this.state;

    const {
      i18n,
      language,
      nameSchemaId,
      getCurrentCustomSchema,
      cultureNames,
      rawTimezones,
      portalTimeZoneId,
      isLoaded,
      cultures,
      portalLanguage,
      tReady,
      setIsLoadedLngTZSettings,
    } = this.props;

    if (isLoaded !== prevProps.isLoaded || tReady !== prevProps.tReady) {
      const isLoadedSetting = isLoaded && tReady;

      if (isLoadedSetting) {
        setIsLoadedLngTZSettings(isLoadedSetting);
      }
    }

    if (
      rawTimezones.length > 0 &&
      isLoaded === true &&
      tReady === true &&
      this.state.timezone === ""
    ) {
      const timezones = mapTimezonesToArray(rawTimezones);

      const timezone =
        timezoneFromSessionStorage ||
        findSelectedItemByKey(timezones, portalTimeZoneId) ||
        rawTimezones[0];

      const timezoneDefault =
        findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];

      this.setState({
        timezone,
        timezoneDefault,
      });
    }

    if (
      cultures.length > 0 &&
      isLoaded === true &&
      tReady === true &&
      this.state.language === ""
    ) {
      const cultureNames = mapCulturesToArray(cultures, i18n);
      const language =
        languageFromSessionStorage ||
        findSelectedItemByKey(cultureNames, portalLanguage) ||
        cultureNames[0];

      const languageDefault =
        findSelectedItemByKey(cultureNames, portalLanguage) || cultureNames[0];

      this.setState({
        language,
        languageDefault,
      });
    }

    const checkScroll = checkScrollSettingsBlock();

    window.addEventListener("resize", checkScroll);
    const scrollLngTZSettings = checkScroll();

    if (scrollLngTZSettings !== hasScroll) {
      this.setState({
        hasScroll: scrollLngTZSettings,
      });
    }

    // TODO: Remove div with height 64 and remove settings-mobile class
    const settingsMobile = document.getElementsByClassName(
      "settings-mobile"
    )[0];

    if (settingsMobile) {
      settingsMobile.style.display = "none";
    }

    if (language !== prevProps.language) {
      i18n
        .changeLanguage(language)
        .then(() => {
          const newLocaleSelectedLanguage =
            findSelectedItemByKey(cultureNames, this.state.language.key) ||
            cultureNames[0];
          this.setState({
            language: languageFromSessionStorage || newLocaleSelectedLanguage,
          });
        })
        //.then(() => getModules(clientStore.dispatch))
        .then(() => getCurrentCustomSchema(nameSchemaId));
    }
    if (timezoneDefault && languageDefault) {
      this.checkChanges();
    }
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.checkInnerWidth);
  }

  onLanguageSelect = (language) => {
    this.setState({ language });
    if (this.settingIsEqualInitialValue("language", language)) {
      saveToSessionStorage("language", "");
      saveToSessionStorage("languageDefault", "");
    } else {
      saveToSessionStorage("language", language);
    }
    this.checkChanges();
  };

  onTimezoneSelect = (timezone) => {
    this.setState({ timezone });
    if (this.settingIsEqualInitialValue("timezone", timezone)) {
      saveToSessionStorage("timezone", "");
      saveToSessionStorage("timezoneDefault", "");
    } else {
      saveToSessionStorage("timezone", timezone);
    }

    this.checkChanges();
  };

  onSaveLngTZSettings = () => {
    const { t, setLanguageAndTime, user, language: lng } = this.props;
    const { language, timezone } = this.state;

    this.setState({ isLoading: true }, function () {
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
        .finally(() => this.setState({ isLoading: false }));
    });

    this.setState({
      showReminder: false,
      timezoneDefault: this.state.timezone,
      languageDefault: this.state.language,
    });

    saveToSessionStorage("languageDefault", language);
    saveToSessionStorage("timezoneDefault", timezone);
  };

  onCancelClick = () => {
    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);

      if (
        valueFromSessionStorage &&
        !this.settingIsEqualInitialValue(settingName, valueFromSessionStorage)
      ) {
        const defaultValue = this.state[settingName + "Default"];

        this.setState({ [settingName]: defaultValue });
        saveToSessionStorage(settingName, "");
      }
    });

    this.setState({
      showReminder: false,
    });

    this.checkChanges();
  };

  settingIsEqualInitialValue = (settingName, value) => {
    const defaultValue = JSON.stringify(this.state[settingName + "Default"]);
    const currentValue = JSON.stringify(value);
    return defaultValue === currentValue;
  };

  checkChanges = () => {
    let hasChanged = false;

    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage &&
        !this.settingIsEqualInitialValue(settingName, valueFromSessionStorage)
      )
        hasChanged = true;
    });

    if (hasChanged !== this.state.hasChanged) {
      this.setState({
        hasChanged: hasChanged,
        showReminder: hasChanged,
      });
    }
  };

  checkInnerWidth = () => {
    if (!isSmallTablet()) {
      this.setState({
        isCustomizationView: true,
      });

      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          "/portal-settings/common/customization"
        )
      );
    } else {
      this.setState({
        isCustomizationView: false,
      });
    }
  };

  onClickLink = (e) => {
    e.preventDefault();
    history.push(e.target.pathname);
  };

  render() {
    const {
      t,
      theme,
      isMobileView,
      rawTimezones,
      cultures,
      i18n,
      isLoadedPage,
      helpLink,
    } = this.props;

    const {
      language,
      isLoading,
      timezone,
      showReminder,
      hasScroll,
      isCustomizationView,
    } = this.state;

    const timezones = mapTimezonesToArray(rawTimezones);
    const cultureNames = mapCulturesToArray(cultures, i18n);

    const tooltipLanguageTimeSettings = (
      <LanguageTimeSettingsTooltip theme={theme} t={t} helpLink={helpLink} />
    );

    const settingsBlock = !(language && timezone) ? null : (
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerLanguage"
          labelText={`${t("Common:Language")}`}
          isVertical={true}
        >
          <ComboBox
            id="comboBoxLanguage"
            options={cultureNames}
            selectedOption={language}
            onSelect={this.onLanguageSelect}
            isDisabled={isLoading}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            className="dropdown-item-width combo-box-settings"
          />
        </FieldContainer>
        <FieldContainer
          id="fieldContainerTimezone"
          labelText={`${t("TimeZone")}`}
          isVertical={true}
        >
          <ComboBox
            id="comboBoxTimezone"
            options={timezones}
            selectedOption={timezone}
            onSelect={this.onTimezoneSelect}
            isDisabled={isLoading}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            className="dropdown-item-width combo-box-settings"
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
              iconName="static/images/combined.shape.svg"
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
          className="save-cancel-buttons"
          onSaveClick={this.onSaveLngTZSettings}
          onCancelClick={this.onCancelClick}
          showReminder={showReminder}
          reminderTest={t("YouHaveUnsavedChanges")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Common:CancelButton")}
          displaySettings={true}
          hasScroll={hasScroll}
        />
      </StyledSettingsComponent>
    );
  }
}

export default inject(({ auth, setup, common }) => {
  const {
    culture,
    timezone,
    timezones,
    nameSchemaId,
    organizationName,
    greetingSettings,
    getPortalTimezones,
    getCurrentCustomSchema,
    cultures,
    helpLink,
  } = auth.settingsStore;

  const { user } = auth.userStore;

  const { setLanguageAndTime } = setup;
  const { isLoaded, setIsLoadedLngTZSettings } = common;
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
    getCurrentCustomSchema,
    getPortalTimezones,
    isLoaded,
    setIsLoadedLngTZSettings,
    cultures,
    helpLink,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(LanguageAndTimeZone))
  )
);
