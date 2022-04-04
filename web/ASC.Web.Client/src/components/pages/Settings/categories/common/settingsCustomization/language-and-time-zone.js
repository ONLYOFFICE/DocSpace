import React from "react";
import { withTranslation } from "react-i18next";
import FieldContainer from "@appserver/components/field-container";
import ToggleButton from "@appserver/components/toggle-button";
import ComboBox from "@appserver/components/combobox";
import Loader from "@appserver/components/loader";
import toastr from "@appserver/components/toast/toastr";
import HelpButton from "@appserver/components/help-button";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { inject, observer } from "mobx-react";
import { LANGUAGE } from "@appserver/common/constants";
import { convertLanguage } from "@appserver/common/utils";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import { LanguageTimeSettingsTooltip } from "../sub-components/common-tooltips";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../../package.json";
import history from "@appserver/common/history";
import { isMobileOnly } from "react-device-detect";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import { isSmallTablet } from "@appserver/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import {
  StyledSettingsComponent,
  StyledScrollbar,
  StyledArrowRightIcon,
} from "./StyledSettings";

const mapTimezonesToArray = (timezones) => {
  return timezones.map((timezone) => {
    return { key: timezone.id, label: timezone.displayName };
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

    const {
      portalLanguage,
      portalTimeZoneId,
      rawTimezones,
      cultureNames,
      /*organizationName,*/
      t,
      //i18n,
    } = props;

    const timezones = mapTimezonesToArray(rawTimezones);
    const language = findSelectedItemByKey(
      cultureNames,
      convertLanguage(portalLanguage || cultureNames[0])
    );
    const timezone = findSelectedItemByKey(
      timezones,
      portalTimeZoneId || timezones[0]
    );

    languageFromSessionStorage = getFromSessionStorage("language");
    languageDefaultFromSessionStorage = getFromSessionStorage(
      "languageDefault"
    );
    timezoneFromSessionStorage = getFromSessionStorage("timezone");
    timezoneDefaultFromSessionStorage = getFromSessionStorage(
      "timezoneDefault"
    );

    setDocumentTitle(t("Customization"));

    this.state = {
      isLoadedData: false,
      isLoading: false,
      timezones,
      timezone: timezoneFromSessionStorage || timezone,
      timezoneDefault: timezoneDefaultFromSessionStorage || timezone,
      language: languageFromSessionStorage || language,
      languageDefault: languageDefaultFromSessionStorage || language,
      isLoadingGreetingSave: false,
      isLoadingGreetingRestore: false,
      hasChanged: false,
      showReminder: false,
      hasScroll: false,
    };
  }

  componentDidMount() {
    const {
      cultureNames,
      portalLanguage,
      portalTimeZoneId,
      getPortalTimezones,
    } = this.props;
    const { timezones, isLoadedData } = this.state;

    window.addEventListener("resize", this.checkInnerWidth);

    if (!timezones.length) {
      getPortalTimezones().then(() => {
        const timezones = mapTimezonesToArray(this.props.rawTimezones);

        const language =
          languageFromSessionStorage ||
          findSelectedItemByKey(cultureNames, portalLanguage) ||
          cultureNames[0];
        const timezone =
          timezoneFromSessionStorage ||
          findSelectedItemByKey(timezones, portalTimeZoneId) ||
          timezones[0];

        const languageDefault =
          findSelectedItemByKey(cultureNames, portalLanguage) ||
          cultureNames[0];
        const timezoneDefault =
          findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];

        this.setState({
          language,
          timezones,
          timezone,
          languageDefault,
          timezoneDefault,
        });

        if (!timezoneDefault) {
          this.setState({
            timezoneDefault: timezone,
          });
        }
        if (!languageDefault) {
          this.setState({
            languageDefault: language,
          });
        }
      });
    }
    if (timezones.length && !isLoadedData) {
      this.setState({ isLoadedData: true });
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const {
      timezones,
      timezoneDefault,
      languageDefault,
      hasScroll,
    } = this.state;

    const {
      i18n,
      language,
      nameSchemaId,
      getCurrentCustomSchema,
      cultureNames,
    } = this.props;

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

    if (timezones.length && !prevState.isLoadedData) {
      this.setState({ isLoadedData: true });
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
    window.removeEventListener(
      "resize",
      this.checkInnerWidth,
      checkScrollSettingsBlock
    );
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
            localStorage.setItem(LANGUAGE, language.key || "en")
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
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          "/settings/common/customization"
        )
      );
      return true;
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
      cultureNames,
      isMobileView,
      helpUrlCommonSettings,
    } = this.props;
    const {
      isLoadedData,
      language,
      isLoading,
      timezones,
      timezone,
      showReminder,
      hasScroll,
    } = this.state;

    const tooltipLanguageTimeSettings = (
      <LanguageTimeSettingsTooltip theme={theme} t={t} />
    );

    const isMobileViewLanguageTimeSettings = (
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            className="inherit-title-link header"
            onClick={this.onClickLink}
            truncate={true}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/settings/common/customization/language-and-time-zone"
            )}
          >
            {t("StudioTimeLanguageSettings")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("LanguageAndTimeZoneSettingsDescription")}
        </Text>
        <Box paddingProp="10px 0 3px 0">
          <Link
            color={theme.studio.settings.common.linkColorHelp}
            target="_blank"
            isHovered={true}
            href={helpUrlCommonSettings}
          >
            {t("Common:LearnMore")}
          </Link>
        </Box>
      </div>
    );
    const settingsBlock = (
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerLanguage"
          labelText={`${t("Common:Language")}:`}
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
            className="dropdown-item-width"
          />
        </FieldContainer>
        <div className="field-container-flex">
          <div className="field-title">{`${t("Automatic time zone")}`}</div>
          <ToggleButton
            className="toggle"
            onChange={() => toastr.info(<>Not implemented</>)}
          />
        </div>
        <FieldContainer
          id="fieldContainerTimezone"
          labelText={`${t("TimeZone")}:`}
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
            className="dropdown-item-width"
          />
        </FieldContainer>
      </div>
    );

    return !isLoadedData ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : isMobileView ? (
      isMobileViewLanguageTimeSettings
    ) : (
      <StyledSettingsComponent
        hasScroll={hasScroll}
        className="category-item-wrapper"
      >
        {this.checkInnerWidth() && !isMobileView && (
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
          <StyledScrollbar stype="smallBlack">{settingsBlock}</StyledScrollbar>
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

export default inject(({ auth, setup }) => {
  const {
    culture,
    timezone,
    timezones,
    //cultures,
    nameSchemaId,
    organizationName,
    greetingSettings,
    //getPortalCultures,
    getPortalTimezones,
    getCurrentCustomSchema,
    helpUrlCommonSettings,
  } = auth.settingsStore;

  const { user } = auth.userStore;

  const { setLanguageAndTime } = setup;

  return {
    theme: auth.settingsStore.theme,
    user,
    portalLanguage: culture,
    portalTimeZoneId: timezone,
    language: culture,
    rawTimezones: timezones,
    //rawCultures: cultures,
    greetingSettings,
    nameSchemaId,
    organizationName,
    //getPortalCultures,
    setLanguageAndTime,
    getCurrentCustomSchema,
    getPortalTimezones,
    helpUrlCommonSettings,
  };
})(
  withCultureNames(
    withTranslation(["Settings", "Common"])(observer(LanguageAndTimeZone))
  )
);
