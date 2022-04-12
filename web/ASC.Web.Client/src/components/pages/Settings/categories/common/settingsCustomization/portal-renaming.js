import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import Loader from "@appserver/components/loader";
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

const PortalRenaming = ({ t, setPortalRename, isMobileView }) => {
  // TODO: Change false
  const [isLoadedData, setIsLoadedData] = useState(true);
  const [isLoadingPortalNameSave, setIsLoadingPortalNameSave] = useState(false);

  const portalNameFromSessionStorage = getFromSessionStorage("portalName");
  const portalNameDefaultFromSessionStorage = getFromSessionStorage(
    "portalNameDefault"
  );

  const errorValueFromSessionStorage = getFromSessionStorage("errorValue");

  const [portalName, setPortalName] = useState(portalNameFromSessionStorage);
  const [portalNameDefault, setPortalNameDefault] = useState(
    portalNameDefaultFromSessionStorage || portalName
  );

  const [showReminder, setShowReminder] = useState(false);
  const [hasScroll, setHasScroll] = useState(false);
  const [errorValue, setErrorValue] = useState(errorValueFromSessionStorage);

  //TODO: Add translation
  const accountNameError = "Account name is empty";
  const lengthNameError =
    "The account name must be between 6 and 50 characters long";

  useEffect(() => {
    setDocumentTitle(t("PortalRenaming"));

    const checkScroll = checkScrollSettingsBlock();

    window.addEventListener("resize", checkInnerWidth);
    window.addEventListener("resize", checkScroll);

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

  //TODO: Need a method to get the portal name
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
      !settingIsEqualInitialValue("portalName", portalNameFromSessionStorage)
    ) {
      setPortalName(portalNameDefault);
      saveToSessionStorage("portalName", "");
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

  const onChangePortalName = (e) => {
    const value = e.target.value;

    onValidateInput(value);

    setPortalName(value);

    if (settingIsEqualInitialValue("portalName", value)) {
      saveToSessionStorage("portalName", "");
      saveToSessionStorage("portalNameDefault", "");
    } else {
      saveToSessionStorage("portalName", value);
      setShowReminder(true);
    }
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

  //return <LoaderCustomization portalRenaming={true} />;
  return !isLoadedData ? (
    <Loader className="pageLoader" type="rombs" size="40px" />
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

export default inject(({ auth, setup }) => {
  const { theme } = auth.settingsStore;
  const { setPortalRename } = setup;

  return {
    theme,
    setPortalRename,
  };
})(withTranslation(["Settings", "Common"])(observer(PortalRenaming)));
