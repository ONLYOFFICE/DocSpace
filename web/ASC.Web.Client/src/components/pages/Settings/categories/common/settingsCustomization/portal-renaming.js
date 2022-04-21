import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import toastr from "@appserver/components/toast/toastr";
import HelpButton from "@appserver/components/help-button";
import FieldContainer from "@appserver/components/field-container";
import TextInput from "@appserver/components/text-input";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../../package.json";
import history from "@appserver/common/history";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@appserver/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { PortalRenamingTooltip } from "../sub-components/common-tooltips";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "../../../../../../HOCs/withLoading";
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

  const [isLoadingPortalNameSave, setIsLoadingPortalNameSave] = useState(false);

  const portalNameFromSessionStorage = getFromSessionStorage("portalName");
  const portalNameDefaultFromSessionStorage = getFromSessionStorage(
    "portalNameDefault"
  );

  const errorValueFromSessionStorage = getFromSessionStorage("errorValue");

  const [portalName, setPortalName] = useState(
    portalNameFromSessionStorage || tenantAlias
  );
  const [portalNameDefault, setPortalNameDefault] = useState(
    portalNameDefaultFromSessionStorage || tenantAlias
  );

  const [showReminder, setShowReminder] = useState(false);
  const [hasScroll, setHasScroll] = useState(false);
  const [errorValue, setErrorValue] = useState(errorValueFromSessionStorage);

  //TODO: Add translation
  const accountNameError = "Account name is empty";
  const lengthNameError =
    "The account name must be between 6 and 50 characters long";

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
    if (portalNameDefault) {
      checkChanges();
    }
  }, [isLoadedSetting, portalNameDefault]);

  const onSavePortalRename = () => {
    setIsLoadingPortalNameSave(true);

    setPortalRename(portalName)
      .then(() => toastr.success(t("SuccessfullySavePortalNameMessage")))
      .catch((error) => {
        //TODO: Add translation
        setErrorValue("Incorrect account name");
        saveToSessionStorage("errorValue", "Incorrect account name");
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

    if (
      portalNameFromSessionStorage &&
      !settingIsEqualInitialValue(portalNameFromSessionStorage)
    ) {
      setPortalName(portalNameDefault);
      saveToSessionStorage("portalName", "");
      setShowReminder(false);
    }
  };

  const onValidateInput = (value) => {
    switch (true) {
      case value === "":
        setErrorValue(accountNameError);
        saveToSessionStorage("errorValue", accountNameError);
        break;
      case value.length < 6 || value.length > 50:
        setErrorValue(lengthNameError);
        saveToSessionStorage("errorValue", lengthNameError);
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
      valueFromSessionStorage &&
      !settingIsEqualInitialValue(valueFromSessionStorage)
    ) {
      hasChanged = true;
    }

    if (hasChanged !== showReminder) {
      setShowReminder(hasChanged);
    }
  };

  const onChangePortalName = (e) => {
    const value = e.target.value;

    onValidateInput(value);

    setPortalName(value);

    if (settingIsEqualInitialValue(value)) {
      saveToSessionStorage("portalName", "");
      saveToSessionStorage("portalNameDefault", "");
    } else {
      saveToSessionStorage("portalName", value);
    }

    checkChanges();
  };

  const checkInnerWidth = () => {
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

  const tooltipPortalRenamingTooltip = <PortalRenamingTooltip t={t} />;
  const hasError = errorValue === null ? false : true;

  const settingsBlock = (
    <div className="settings-block">
      <div className="settings-block-description">
        {t("PortalRenamingMobile")}
      </div>
      <FieldContainer
        id="fieldContainerWelcomePage"
        className="field-container-width"
        labelText={`${t("PortalRenamingLabelText")}:`}
        isVertical={true}
      >
        <TextInput
          scale={true}
          value={portalName}
          onChange={onChangePortalName}
          isDisabled={isLoadingPortalNameSave}
          hasError={hasError}
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
        <StyledScrollbar stype="smallBlack">{settingsBlock}</StyledScrollbar>
      ) : (
        <> {settingsBlock}</>
      )}
      <SaveCancelButtons
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
