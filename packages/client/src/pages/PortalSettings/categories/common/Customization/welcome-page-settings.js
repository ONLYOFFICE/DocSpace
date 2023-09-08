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
import config from "PACKAGE_FILE";
import { useNavigate } from "react-router-dom";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@docspace/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

let greetingTitleFromSessionStorage = "";
let greetingTitleDefaultFromSessionStorage = "";
const settingNames = ["greetingTitle"];

const WelcomePageSettings = (props) => {
  const {
    t,
    greetingSettings,
    isLoaded,
    setIsLoadedWelcomePageSettings,
    tReady,
    initSettings,
    setIsLoaded,
    setGreetingTitle,
    restoreGreetingTitle,
    isMobileView,
    isLoadedPage,
    greetingSettingsIsDefault,

    getSettings,
    getGreetingSettingsIsDefault,
    currentColorScheme,
    welcomePageSettingsUrl,
  } = props;

  const navigate = useNavigate();

  const [state, setState] = React.useState({
    isLoading: false,
    greetingTitle: "",
    greetingTitleDefault: "",
    isLoadingGreetingSave: false,
    isLoadingGreetingRestore: false,
    hasChanged: false,
    showReminder: false,
    hasScroll: false,
    isCustomizationView: false,
  });

  const prevState = React.useRef({
    isLoadingGreetingSave: false,
    isLoadingGreetingRestore: false,
  });
  const prevProps = React.useRef({
    isLoaded: "",
    tReady: "",
    greetingSettings: "",
  });

  React.useEffect(() => {
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

    if (!isLoaded) initSettings().then(() => setIsLoaded(true));

    checkInnerWidth();
    window.addEventListener("resize", checkInnerWidth);

    const isLoadedSetting = isLoaded && tReady;

    if (isLoadedSetting) setIsLoadedWelcomePageSettings(isLoadedSetting);

    if (greetingTitleDefault || greetingTitle) {
      checkChanges();
    }

    setState((val) => ({
      ...val,
      greetingTitle,
      greetingTitleDefault,
    }));

    return () => {
      window.removeEventListener("resize", checkInnerWidth);
    };
  }, []);

  React.useEffect(() => {
    if (
      isLoaded !== prevProps.current.isLoaded ||
      tReady !== prevProps.current.tReady
    ) {
      const isLoadedSetting = isLoaded && tReady;

      if (isLoadedSetting) {
        setIsLoadedWelcomePageSettings(isLoadedSetting);
      }
    }

    const checkScroll = checkScrollSettingsBlock();

    window.addEventListener("resize", checkScroll);
    const scrollLngTZSettings = checkScroll();

    if (scrollLngTZSettings !== state.hasScroll) {
      setState((val) => ({ ...val, hasScroll: scrollLngTZSettings }));
    }

    // TODO: Remove div with height 64 and remove settings-mobile class
    const settingsMobile =
      document.getElementsByClassName("settings-mobile")[0];

    if (settingsMobile) {
      settingsMobile.style.display = "none";
    }

    if (greetingSettings !== prevProps.greetingSettings) {
      setState((val) => ({ ...val, greetingTitle: greetingSettings }));
    }

    if (state.greetingTitleDefault || state.greetingTitle) {
      checkChanges();
    }

    if (
      (state.isLoadingGreetingSave !== prevState.isLoadingGreetingSave &&
        state.isLoadingGreetingSave === false) ||
      (state.isLoadingGreetingRestore !== prevState.isLoadingGreetingRestore &&
        state.isLoadingGreetingRestore === false)
    ) {
      getSettings();
      getGreetingSettingsIsDefault();
    }
  }, [
    isLoaded,
    setIsLoadedWelcomePageSettings,
    tReady,
    greetingSettings,
    getSettings,
    getGreetingSettingsIsDefault,
    state.hasScroll,
    state.greetingTitle,
    state.isLoadingGreetingSave,
    state.isLoadingGreetingRestore,
  ]);

  React.useEffect(() => {
    prevProps.current = { isLoaded, tReady, greetingSettings };
  }, [isLoaded, tReady, greetingSettings]);

  React.useEffect(() => {
    prevState.current = {
      isLoadingGreetingSave: state.isLoadingGreetingSave,
      isLoadingGreetingRestore: state.isLoadingGreetingRestore,
    };
  }, [state.isLoadingGreetingSave, state.isLoadingGreetingRestore]);

  const onChangeGreetingTitle = (e) => {
    setState((val) => ({ ...val, greetingTitle: e.target.value }));

    if (settingIsEqualInitialValue("greetingTitle", e.target.value)) {
      saveToSessionStorage("greetingTitle", "none");
      saveToSessionStorage("greetingTitleDefault", "none");
    } else {
      saveToSessionStorage("greetingTitle", e.target.value);
      setState((val) => ({
        ...val,
        showReminder: true,
      }));
    }

    checkChanges();
  };

  const onSaveGreetingSettings = () => {
    const { greetingTitle } = state;
    setState((val) => ({ ...val, isLoadingGreetingSave: true }));
    setGreetingTitle(greetingTitle)
      .then(() => {
        toastr.success(t("SuccessfullySaveGreetingSettingsMessage"));
      })
      .catch((error) => toastr.error(error))
      .finally(() =>
        setState((val) => ({ ...val, isLoadingGreetingSave: false }))
      );

    setState((val) => ({ ...val, showReminder: false }));

    saveToSessionStorage("greetingTitle", greetingTitle);
    saveToSessionStorage("greetingTitleDefault", greetingTitle);
  };

  const onRestoreGreetingSettings = () => {
    setState((val) => ({ ...val, isLoadingGreetingRestore: true }));
    restoreGreetingTitle()
      .then(() => {
        setState((val) => ({
          ...val,
          greetingTitle: greetingSettings,
          greetingTitleDefault: greetingSettings,
          showReminder: false,
        }));

        saveToSessionStorage("greetingTitle", "none");
        saveToSessionStorage("greetingTitleDefault", "none");

        toastr.success(t("SuccessfullySaveGreetingSettingsMessage"));
      })
      .catch((error) => toastr.error(error))
      .finally(() =>
        setState((val) => ({ ...val, isLoadingGreetingRestore: false }))
      );
  };

  const settingIsEqualInitialValue = (stateName, value) => {
    const defaultValue = JSON.stringify(state[stateName + "Default"]);
    const currentValue = JSON.stringify(value);
    return defaultValue === currentValue;
  };

  const checkChanges = () => {
    let hasChanged = false;

    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage !== "none" &&
        valueFromSessionStorage !== null &&
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
          value={state.greetingTitle}
          onChange={onChangeGreetingTitle}
          isDisabled={
            state.isLoadingGreetingSave || state.isLoadingGreetingRestore
          }
          placeholder={t("EnterTitle")}
        />
      </FieldContainer>
    </div>
  );

  return !isLoadedPage ? (
    <LoaderCustomization welcomePage={true} />
  ) : (
    <StyledSettingsComponent
      hasScroll={state.hasScroll}
      className="category-item-wrapper"
    >
      {state.isCustomizationView && !isMobileView && (
        <div className="category-item-heading">
          <div className="category-item-title">{t("CustomTitlesWelcome")}</div>
        </div>
      )}
      <div className="category-item-description">
        <Text fontSize="13px" fontWeight={400}>
          {t("CustomTitlesDescription")}
        </Text>
        <Link
          className="link-learn-more"
          color={currentColorScheme.main.accent}
          target="_blank"
          isHovered
          href={welcomePageSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>
      {(isMobileOnly && isSmallTablet()) || isSmallTablet() ? (
        <StyledScrollbar stype="mediumBlack">{settingsBlock}</StyledScrollbar>
      ) : (
        <> {settingsBlock}</>
      )}
      <SaveCancelButtons
        tabIndex={6}
        id="buttonsWelcomePage"
        className="save-cancel-buttons"
        onSaveClick={onSaveGreetingSettings}
        onCancelClick={onRestoreGreetingSettings}
        showReminder={state.showReminder}
        reminderTest={t("YouHaveUnsavedChanges")}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Common:Restore")}
        displaySettings={true}
        hasScroll={state.hasScroll}
        disableRestoreToDefault={greetingSettingsIsDefault}
        additionalClassSaveButton="welcome-page-save"
        additionalClassCancelButton="welcome-page-cancel"
      />
    </StyledSettingsComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const {
    greetingSettings,
    organizationName,
    theme,
    getSettings,
    currentColorScheme,
    welcomePageSettingsUrl,
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
    currentColorScheme,
    welcomePageSettingsUrl,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(WelcomePageSettings))
  )
);
