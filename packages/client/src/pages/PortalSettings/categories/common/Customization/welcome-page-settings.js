import CombinedShapeSvgUrl from "PUBLIC_DIR/images/combined.shape.svg?url";
import React from "react";
import { withTranslation } from "react-i18next";
import FieldContainer from "@docspace/components/field-container";
import toastr from "@docspace/components/toast/toastr";
import TextInput from "@docspace/components/text-input";
import HelpButton from "@docspace/components/help-button";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import { inject, observer } from "mobx-react";
import { CustomTitlesTooltip } from "../sub-components/common-tooltips";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import history from "@docspace/common/history";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@docspace/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";

let greetingTitleFromSessionStorage = "";
let greetingTitleDefaultFromSessionStorage = "";
const settingNames = ["greetingTitle"];

class WelcomePageSettings extends React.Component {
  constructor(props) {
    super(props);

    const { t, greetingSettings /*, organizationName*/ } = props;

    greetingTitleFromSessionStorage = getFromSessionStorage("greetingTitle");

    greetingTitleDefaultFromSessionStorage = getFromSessionStorage(
      "greetingTitleDefault"
    );

    setDocumentTitle(t("CustomTitlesWelcome"));

    const greetingTitle =
      greetingTitleFromSessionStorage === null ||
      greetingTitleFromSessionStorage === "none"
        ? greetingSettings
        : greetingTitleFromSessionStorage;

    const greetingTitleDefault =
      greetingTitleDefaultFromSessionStorage === null ||
      greetingTitleDefaultFromSessionStorage === "none"
        ? greetingSettings
        : greetingTitleDefaultFromSessionStorage;

    this.state = {
      isLoading: false,
      greetingTitle,
      greetingTitleDefault,
      isLoadingGreetingSave: false,
      isLoadingGreetingRestore: false,
      hasChanged: false,
      showReminder: false,
      hasScroll: false,
      isCustomizationView: false,
    };
  }

  componentDidMount() {
    const {
      isLoaded,
      setIsLoadedWelcomePageSettings,
      tReady,
      initSettings,
      setIsLoaded,
    } = this.props;
    const { greetingTitleDefault, greetingTitle } = this.state;

    if (!isLoaded) initSettings().then(() => setIsLoaded(true));

    this.checkInnerWidth();
    window.addEventListener("resize", this.checkInnerWidth);

    const isLoadedSetting = isLoaded && tReady;

    if (isLoadedSetting) setIsLoadedWelcomePageSettings(isLoadedSetting);

    if (greetingTitleDefault || greetingTitle) {
      this.checkChanges();
    }
  }

  componentDidUpdate(prevProps, prevState) {
    const {
      isLoaded,
      setIsLoadedWelcomePageSettings,
      tReady,
      greetingSettings,
      getSettings,
      getGreetingSettingsIsDefault,
    } = this.props;

    const {
      hasScroll,
      greetingTitle,
      isLoadingGreetingSave,
      isLoadingGreetingRestore,
    } = this.state;

    if (isLoaded !== prevProps.isLoaded || tReady !== prevProps.tReady) {
      const isLoadedSetting = isLoaded && tReady;

      if (isLoadedSetting) {
        setIsLoadedWelcomePageSettings(isLoadedSetting);
      }
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

    if (greetingSettings !== prevProps.greetingSettings) {
      this.setState({
        greetingTitle: greetingSettings,
      });
    }

    if (this.state.greetingTitleDefault || greetingTitle) {
      this.checkChanges();
    }

    if (
      (isLoadingGreetingSave !== prevState.isLoadingGreetingSave &&
        isLoadingGreetingSave === false) ||
      (isLoadingGreetingRestore !== prevState.isLoadingGreetingRestore &&
        isLoadingGreetingRestore === false)
    ) {
      getSettings();
      getGreetingSettingsIsDefault();
    }
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.checkInnerWidth);
  }

  onChangeGreetingTitle = (e) => {
    this.setState({ greetingTitle: e.target.value });

    if (this.settingIsEqualInitialValue("greetingTitle", e.target.value)) {
      saveToSessionStorage("greetingTitle", "none");
      saveToSessionStorage("greetingTitleDefault", "none");
    } else {
      saveToSessionStorage("greetingTitle", e.target.value);
      this.setState({
        showReminder: true,
      });
    }

    this.checkChanges();
  };

  onSaveGreetingSettings = () => {
    const { setGreetingTitle, t } = this.props;
    const { greetingTitle } = this.state;
    this.setState({ isLoadingGreetingSave: true }, function () {
      setGreetingTitle(greetingTitle)
        .then(() => {
          toastr.success(t("SuccessfullySaveGreetingSettingsMessage"));
        })
        .catch((error) => toastr.error(error))
        .finally(() => this.setState({ isLoadingGreetingSave: false }));
    });

    this.setState({
      showReminder: false,
    });

    saveToSessionStorage("greetingTitle", greetingTitle);
    saveToSessionStorage("greetingTitleDefault", greetingTitle);
  };

  onRestoreGreetingSettings = () => {
    const { restoreGreetingTitle, t, greetingSettings } = this.props;
    this.setState({ isLoadingGreetingRestore: true }, function () {
      restoreGreetingTitle()
        .then(() => {
          this.setState({
            greetingTitle: greetingSettings,
            greetingTitleDefault: greetingSettings,
            showReminder: false,
          });

          saveToSessionStorage("greetingTitle", "none");
          saveToSessionStorage("greetingTitleDefault", "none");

          toastr.success(t("SuccessfullySaveGreetingSettingsMessage"));
        })
        .catch((error) => toastr.error(error))
        .finally(() => this.setState({ isLoadingGreetingRestore: false }));
    });
  };

  settingIsEqualInitialValue = (stateName, value) => {
    const defaultValue = JSON.stringify(this.state[stateName + "Default"]);
    const currentValue = JSON.stringify(value);
    return defaultValue === currentValue;
  };

  checkChanges = () => {
    let hasChanged = false;

    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage !== "none" &&
        valueFromSessionStorage !== null &&
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

      const currentUrl = window.location.href.replace(
        window.location.origin,
        ""
      );

      const newUrl = combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        "/portal-settings/customization/general"
      );

      if (newUrl === currentUrl) return;

      history.push(newUrl);
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
      isMobileView,
      isLoadedPage,
      greetingSettingsIsDefault,
    } = this.props;
    const {
      greetingTitle,
      isLoadingGreetingSave,
      isLoadingGreetingRestore,
      showReminder,
      hasScroll,
      isCustomizationView,
    } = this.state;

    const tooltipCustomTitlesTooltip = <CustomTitlesTooltip t={t} />;

    const settingsBlock = (
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerWelcomePage"
          className="field-container-width"
          labelText={`${t("Common:Title")}`}
          isVertical={true}
        >
          <TextInput
            tabIndex={5}
            id="textInputContainerWelcomePage"
            scale={true}
            value={greetingTitle}
            onChange={this.onChangeGreetingTitle}
            isDisabled={isLoadingGreetingSave || isLoadingGreetingRestore}
            placeholder={t("EnterTitle")}
          />
        </FieldContainer>
      </div>
    );

    return !isLoadedPage ? (
      <LoaderCustomization welcomePage={true} />
    ) : (
      <StyledSettingsComponent
        hasScroll={hasScroll}
        className="category-item-wrapper"
      >
        {isCustomizationView && !isMobileView && (
          <div className="category-item-heading">
            <div className="category-item-title">
              {t("CustomTitlesWelcome")}
            </div>
            <HelpButton
              offsetRight={0}
              iconName={CombinedShapeSvgUrl}
              size={12}
              tooltipContent={tooltipCustomTitlesTooltip}
            />
          </div>
        )}
        {(isMobileOnly && isSmallTablet()) || isSmallTablet() ? (
          <StyledScrollbar stype="mediumBlack">{settingsBlock}</StyledScrollbar>
        ) : (
          <> {settingsBlock}</>
        )}
        <SaveCancelButtons
          tabIndex={6}
          id="buttonsWelcomePage"
          className="save-cancel-buttons"
          onSaveClick={this.onSaveGreetingSettings}
          onCancelClick={this.onRestoreGreetingSettings}
          showReminder={showReminder}
          reminderTest={t("YouHaveUnsavedChanges")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Common:Restore")}
          displaySettings={true}
          hasScroll={hasScroll}
          disableRestoreToDefault={greetingSettingsIsDefault}
        />
      </StyledSettingsComponent>
    );
  }
}

export default inject(({ auth, setup, common }) => {
  const {
    greetingSettings,
    organizationName,
    theme,
    getSettings,
  } = auth.settingsStore;
  const { setGreetingTitle, restoreGreetingTitle } = setup;
  const {
    isLoaded,
    setIsLoadedWelcomePageSettings,
    initSettings,
    setIsLoaded,
    greetingSettingsIsDefault,
    getGreetingSettingsIsDefault,
  } = common;
  return {
    theme,
    greetingSettings,
    organizationName,
    setGreetingTitle,
    restoreGreetingTitle,
    isLoaded,
    setIsLoadedWelcomePageSettings,
    greetingSettingsIsDefault,
    getGreetingSettingsIsDefault,
    getSettings,
    initSettings,
    setIsLoaded,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(WelcomePageSettings))
  )
);
