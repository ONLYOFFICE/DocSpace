import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import HelpButton from "@docspace/components/help-button";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import history from "@docspace/common/history";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@docspace/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { PortalRenamingTooltip } from "../sub-components/common-tooltips";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";
const PortalRenaming = (props) => {
  const {
    t,
    setPortalRename,
    isMobileView,
    tReady,
    isLoaded,
    setIsLoadedPortalRenaming,
    isLoadedPage,
    tenantAlias,
  } = props;

  const portalNameFromSessionStorage = getFromSessionStorage("portalName");

  const portalNameDefaultFromSessionStorage = getFromSessionStorage(
    "portalNameDefault"
  );

  const portalNameInitially =
    portalNameFromSessionStorage === null ||
    portalNameFromSessionStorage === "none"
      ? tenantAlias
      : portalNameFromSessionStorage;

  const portalNameDefaultInitially =
    portalNameDefaultFromSessionStorage === null ||
    portalNameDefaultFromSessionStorage === "none"
      ? tenantAlias
      : portalNameDefaultFromSessionStorage;

  const [portalName, setPortalName] = useState(portalNameInitially);

  const [portalNameDefault, setPortalNameDefault] = useState(
    portalNameDefaultInitially
  );

  const [isLoadingPortalNameSave, setIsLoadingPortalNameSave] = useState(false);

  const [showReminder, setShowReminder] = useState(false);

  const [hasScroll, setHasScroll] = useState(false);

  const errorValueFromSessionStorage = getFromSessionStorage("errorValue");

  const [errorValue, setErrorValue] = useState(errorValueFromSessionStorage);

  const isLoadedSetting = isLoaded && tReady;

  useEffect(() => {
    setDocumentTitle(t("PortalRenaming"));

    const checkScroll = checkScrollSettingsBlock();

    window.addEventListener("resize", checkInnerWidth, checkScroll);

    const scrollPortalName = checkScroll();

    if (scrollPortalName !== hasScroll) {
      setHasScroll(scrollPortalName);
    }

    // TODO: Remove div with height 64 and remove settings-mobile class
    const settingsMobile = document.getElementsByClassName(
      "settings-mobile"
    )[0];

    if (settingsMobile) {
      settingsMobile.style.display = "none";
    }

    return () =>
      window.removeEventListener(
        "resize",
        checkInnerWidth,
        checkScrollSettingsBlock
      );
  }, []);

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedPortalRenaming(isLoadedSetting);

    if (portalNameDefault || portalName) {
      checkChanges();
    }
  }, [isLoadedSetting, portalNameDefault, portalName]);

  const onSavePortalRename = () => {
    if (errorValue) return;

    setIsLoadingPortalNameSave(true);

    setPortalRename(portalName)
      .then(() => toastr.success(t("SuccessfullySavePortalNameMessage")))
      .catch((error) => {
        setErrorValue(error);
        saveToSessionStorage("errorValue", error);
      })
      .finally(() => setIsLoadingPortalNameSave(false));

    setShowReminder(false);
    setPortalName(portalName);
    setPortalNameDefault(portalName);

    saveToSessionStorage("portalName", portalName);
    saveToSessionStorage("portalNameDefault", portalName);
  };

  const onCancelPortalName = () => {
    const portalNameFromSessionStorage = getFromSessionStorage("portalName");

    saveToSessionStorage("errorValue", null);

    setErrorValue(null);

    if (
      portalNameFromSessionStorage !== "none" &&
      portalNameFromSessionStorage !== null &&
      !settingIsEqualInitialValue(portalNameFromSessionStorage)
    ) {
      setPortalName(portalNameDefault);
      saveToSessionStorage("portalName", "none");
      setShowReminder(false);
    }
  };

  const onChangePortalName = (e) => {
    const value = e.target.value;

    onValidateInput(value);

    setPortalName(value);

    if (settingIsEqualInitialValue(value)) {
      saveToSessionStorage("portalName", "none");
      saveToSessionStorage("portalNameDefault", "none");
    } else {
      saveToSessionStorage("portalName", value);
    }

    checkChanges();
  };

  const onValidateInput = (value) => {
    const validDomain = new RegExp("^[a-z0-9]([a-z0-9-]){1,98}[a-z0-9]$", "i");

    switch (true) {
      case value === "":
        setErrorValue(t("PortalNameEmpty"));
        saveToSessionStorage("errorValue", t("PortalNameEmpty"));
        break;
      case value.length < 6 || value.length > 50:
        setErrorValue(t("PortalNameLength"));
        saveToSessionStorage("errorValue", t("PortalNameLength"));
        break;
      case !validDomain.test(value):
        setErrorValue(t("PortalNameIncorrect"));
        saveToSessionStorage("errorValue", t("PortalNameIncorrect"));
        break;
      default:
        saveToSessionStorage("errorValue", null);
        setErrorValue(null);
    }
  };

  const settingIsEqualInitialValue = (value) => {
    const defaultValue = JSON.stringify(portalNameDefault);
    const currentValue = JSON.stringify(value);
    return defaultValue === currentValue;
  };

  const checkChanges = () => {
    let hasChanged = false;

    const valueFromSessionStorage = getFromSessionStorage("portalName");
    if (
      valueFromSessionStorage !== "none" &&
      valueFromSessionStorage !== null &&
      !settingIsEqualInitialValue(valueFromSessionStorage)
    ) {
      hasChanged = true;
    }

    if (hasChanged !== showReminder) {
      setShowReminder(hasChanged);
    }
  };

  const checkInnerWidth = () => {
    if (!isSmallTablet()) {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          "/portal-settings/common/customization"
        )
      );
      return true;
    }
  };

  const tooltipPortalRenamingTooltip = <PortalRenamingTooltip t={t} />;
  const hasError = errorValue === null ? false : true;

  const settingsBlock = (
    <div className="settings-block">
      <div className="settings-block-description">
        {t("PortalRenamingMobile")}
      </div>
      <FieldContainer
        id="fieldContainerPortalRenaming"
        className="field-container-width"
        labelText={`${t("PortalRenamingLabelText")}`}
        isVertical={true}
      >
        <TextInput
          id="textInputContainerPortalRenaming"
          scale={true}
          value={portalName}
          onChange={onChangePortalName}
          isDisabled={isLoadingPortalNameSave}
          hasError={hasError}
          placeholder={`${t("EnterName")}`}
        />
        <div className="errorText">{errorValue}</div>
      </FieldContainer>
    </div>
  );

  return !isLoadedPage ? (
    <LoaderCustomization portalRenaming={true} />
  ) : (
    <StyledSettingsComponent
      hasScroll={hasScroll}
      className="category-item-wrapper"
    >
      {checkInnerWidth() && !isMobileView && (
        <div className="category-item-heading">
          <div className="category-item-title">{t("PortalRenaming")}</div>
          <HelpButton
            iconName="static/images/combined.shape.svg"
            size={12}
            tooltipContent={tooltipPortalRenamingTooltip}
          />
        </div>
      )}
      {(isMobileOnly && isSmallTablet()) || isSmallTablet() ? (
        <StyledScrollbar stype="mediumBlack">{settingsBlock}</StyledScrollbar>
      ) : (
        <> {settingsBlock}</>
      )}
      <SaveCancelButtons
        id="buttonsPortalRenaming"
        className="save-cancel-buttons"
        onSaveClick={onSavePortalRename}
        onCancelClick={onCancelPortalName}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Common:CancelButton")}
        showReminder={showReminder}
        reminderTest={t("YouHaveUnsavedChanges")}
        displaySettings={true}
        hasScroll={hasScroll}
      />
    </StyledSettingsComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const { theme, tenantAlias } = auth.settingsStore;
  const { setPortalRename } = setup;
  const { isLoaded, setIsLoadedPortalRenaming } = common;
  return {
    theme,
    setPortalRename,
    isLoaded,
    setIsLoadedPortalRenaming,
    tenantAlias,
  };
})(
  withLoading(withTranslation(["Settings", "Common"])(observer(PortalRenaming)))
);
