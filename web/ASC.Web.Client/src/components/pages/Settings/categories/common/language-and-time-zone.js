import React from "react";
import { withTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import FieldContainer from "@appserver/components/field-container";
import ToggleButton from "@appserver/components/toggle-button";
import ComboBox from "@appserver/components/combobox";
import Loader from "@appserver/components/loader";
import toastr from "@appserver/components/toast/toastr";
import HelpButton from "@appserver/components/help-button";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";
import { setDocumentTitle } from "../../../../../helpers/utils";
import { inject, observer } from "mobx-react";
import { LANGUAGE } from "@appserver/common/constants";
import { convertLanguage } from "@appserver/common/utils";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import { LanguageTimeSettingsTooltip } from "./sub-components/common-tooltips";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../package.json";
import history from "@appserver/common/history";
import { isMobileOnly } from "react-device-detect";
import Scrollbar from "@appserver/components/scrollbar";

const mapTimezonesToArray = (timezones) => {
  return timezones.map((timezone) => {
    return { key: timezone.id, label: timezone.displayName };
  });
};

const findSelectedItemByKey = (items, selectedItemKey) => {
  return items.find((item) => item.key === selectedItemKey);
};

const menuHeight = "48px";
const sectionHeight = "50px";
const paddingSectionWrapperContent = "22px";
const saveCancelButtons = "56px";
const flex = "4px";

const StyledScrollbar = styled(Scrollbar)`
  height: calc(
    100vh -
      (
        ${menuHeight} + ${sectionHeight} + ${paddingSectionWrapperContent} +
          ${saveCancelButtons} + ${flex}
      )
  ) !important;
  width: 100% !important;
`;

const StyledComponent = styled.div`
  .combo-button-label {
    max-width: 100%;
    font-weight: 400;
  }

  .field-container-flex {
    display: flex;
    justify-content: space-between;
    margin-top: 8px;
    margin-bottom: 12px;
  }

  .toggle {
    position: inherit;
    grid-gap: inherit;
  }

  .field-title {
    font-weight: 600;
    line-height: 20px;
  }

  @media (max-width: 599px) {
    ${(props) =>
      props.hasScroll &&
      css`
        width: ${isMobileOnly ? "100vw" : "calc(100vw - 52px)"};
        left: -16px;
        position: relative;

        .settings-block {
          width: ${isMobileOnly ? "calc(100vw - 32px)" : "calc(100vw - 84px)"};
          max-width: none;
          padding-left: 16px;
        }
      `}
  }

  @media (min-width: 600px) {
    .settings-block {
      max-width: 350px;
      height: auto;
    }
  }

  @media (orientation: landscape) and (max-width: 600px) {
    ${isMobileOnly &&
    css`
      .settings-block {
        height: auto;
      }
    `}
  }
`;

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
      sectionWidth: null,
      hasScroll: false,
      heightSettingsBlock: null,
      heightScrollBody: null,
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
    const { timezones, timezoneDefault, languageDefault } = this.state;
    const {
      i18n,
      language,
      nameSchemaId,
      getCurrentCustomSchema,
      cultureNames,
    } = this.props;

    this.checkHeightSettingsBlock();
    window.addEventListener("resize", this.checkHeightSettingsBlock);

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
      this.checkHeightSettingsBlock
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
    if (window.innerWidth > 600) {
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

  checkHeightSettingsBlock = () => {
    if (this.settingsDiv && this.scrollBody) return;

    this.settingsDiv = document.getElementsByClassName("settings-block")[0];

    if (!this.settingsDiv) return;

    this.scrollBody = this.settingsDiv.closest(".scroll-body");

    if (!this.scrollBody) return;

    const height = getComputedStyle(this.settingsDiv).height.slice(0, -2);
    const heightScrollBody = getComputedStyle(this.scrollBody).height.slice(
      0,
      -2
    );

    if (
      this.state.heightSettingsBlock === height &&
      this.state.heightScrollBody === heightScrollBody
    ) {
      return;
    }

    this.setState({
      heightSettingsBlock: height,
    });
    this.setState({
      heightScrollBody: heightScrollBody,
    });

    if (parseInt(height, 10) > parseInt(heightScrollBody, 10)) {
      this.setState({
        hasScroll: true,
      });
    } else {
      this.setState({
        hasScroll: false,
      });
    }
  };

  render() {
    const { t, theme, cultureNames } = this.props;
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
    ) : (
      <StyledComponent hasScroll={hasScroll}>
        {this.checkInnerWidth() && (
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
        {(isMobileOnly && window.innerWidth < 600) ||
        window.innerWidth < 600 ? (
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
      </StyledComponent>
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
  };
})(
  withCultureNames(
    withTranslation(["Settings", "Common"])(observer(LanguageAndTimeZone))
  )
);
