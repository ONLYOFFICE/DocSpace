﻿import React, { useState, useEffect, useCallback } from "react";
import { withTranslation, Trans } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import { useNavigate } from "react-router-dom";
import { isMobileOnly } from "react-device-detect";
import { isSmallTablet } from "@docspace/components/utils/device";
import checkScrollSettingsBlock from "../utils";
import { StyledSettingsComponent, StyledScrollbar } from "./StyledSettings";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "SRC_DIR/helpers/utils";
import LoaderCustomization from "../sub-components/loaderCustomization";
import withLoading from "SRC_DIR/HOCs/withLoading";
import { PortalRenamingDialog } from "SRC_DIR/components/dialogs";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

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
    initSettings,
    setIsLoaded,
    getAllSettings,
    domain,
    currentColorScheme,
    renamingSettingsUrl,
  } = props;

  const navigate = useNavigate();

  const portalNameFromSessionStorage = getFromSessionStorage("portalName");

  const portalNameDefaultFromSessionStorage =
    getFromSessionStorage("portalNameDefault");

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

  const [isCustomizationView, setIsCustomizationView] = useState(false);

  const [domainValidator, setDomainValidator] = useState(null);

  const [isShowModal, setIsShowModal] = useState(false);

  useEffect(() => {
    getAllSettings().then((res) => {
      setDomainValidator(res.domainValidator);
    });
  }, []);

  useEffect(() => {
    setDocumentTitle(t("PortalRenaming"));
    if (!isLoaded) initSettings().then(() => setIsLoaded(true));

    const checkScroll = checkScrollSettingsBlock();
    checkInnerWidth();

    window.addEventListener("resize", checkInnerWidth);

    const scrollPortalName = checkScroll();

    if (scrollPortalName !== hasScroll) {
      setHasScroll(scrollPortalName);
    }

    return () => window.removeEventListener("resize", checkInnerWidth);
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
      .then((res) => {
        onCloseModal();
        toastr.success(t("SuccessfullySavePortalNameMessage"));

        setPortalName(portalName);
        setPortalNameDefault(portalName);
        sessionStorage.clear();

        navigate(res);
      })
      .catch((error) => {
        let errorMessage = "";
        if (typeof error === "object") {
          errorMessage =
            error?.response?.data?.error?.message ||
            error?.statusText ||
            error?.message ||
            "";
        } else {
          errorMessage = error;
        }

        setErrorValue(errorMessage);
        saveToSessionStorage("errorValue", errorMessage);
      });

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
    const validDomain = new RegExp(domainValidator.regex);

    switch (true) {
      case value === "":
        setErrorValue(t("PortalNameEmpty"));
        saveToSessionStorage("errorValue", t("PortalNameEmpty"));
        break;
      case value.length < domainValidator.minLength ||
        value.length > domainValidator.maxLength:
        setErrorValue(
          t("PortalNameLength", {
            minLength: domainValidator.minLength,
            maxLength: domainValidator.maxLength,
          })
        );
        saveToSessionStorage(
          "errorValue",
          t("PortalNameLength", {
            minLength: domainValidator.minLength,
            maxLength: domainValidator.maxLength,
          })
        );
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

  const checkInnerWidth = useCallback(() => {
    if (!isSmallTablet()) {
      setIsCustomizationView(true);

      const currentUrl = window.location.href.replace(
        window.location.origin,
        ""
      );

      const newUrl = "/portal-settings/customization/general";
      if (newUrl === currentUrl) return;

      navigate(newUrl);
    } else {
      setIsCustomizationView(false);
    }
  }, [isSmallTablet, setIsCustomizationView]);

  const onOpenModal = () => {
    setIsShowModal(true);
  };

  const onCloseModal = () => {
    setIsShowModal(false);
  };

  const hasError = errorValue === null ? false : true;

  const settingsBlock = (
    <div className="settings-block">
      <FieldContainer
        id="fieldContainerPortalRenaming"
        className="field-container-width"
        labelText={`${t("PortalRenamingLabelText")}`}
        isVertical={true}
      >
        <TextInput
          tabIndex={10}
          id="textInputContainerPortalRenaming"
          scale={true}
          value={portalName}
          onChange={onChangePortalName}
          isDisabled={isLoadingPortalNameSave}
          hasError={hasError}
          placeholder={`${t("Common:EnterName")}`}
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
      {isCustomizationView && !isMobileView && (
        <div className="category-item-heading">
          <div className="category-item-title">{t("PortalRenaming")}</div>
        </div>
      )}
      <div className="category-item-description">
        <Text fontSize="13px" fontWeight={400}>
          {t("PortalRenamingDescriptionText", { domain })}
        </Text>
        <Text fontSize="13px" fontWeight={400}>
          <Trans t={t} i18nKey="PortalRenamingNote" />
        </Text>
        <Link
          className="link-learn-more"
          color={currentColorScheme.main.accent}
          target="_blank"
          isHovered
          href={renamingSettingsUrl}
        >
          {t("Common:LearnMore")}
        </Link>
      </div>
      {settingsBlock}
      <SaveCancelButtons
        tabIndex={11}
        id="buttonsPortalRenaming"
        className="save-cancel-buttons"
        onSaveClick={onOpenModal}
        onCancelClick={onCancelPortalName}
        saveButtonLabel={t("Common:SaveButton")}
        cancelButtonLabel={t("Common:CancelButton")}
        showReminder={showReminder}
        reminderTest={t("YouHaveUnsavedChanges")}
        displaySettings={true}
        hasScroll={hasScroll}
        additionalClassSaveButton="portal-renaming-save"
        additionalClassCancelButton="portal-renaming-cancel"
      />
      <PortalRenamingDialog
        visible={isShowModal}
        onClose={onCloseModal}
        onSave={onSavePortalRename}
        isSaving={isLoadingPortalNameSave}
      />
    </StyledSettingsComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const {
    theme,
    tenantAlias,
    baseDomain,
    currentColorScheme,
    renamingSettingsUrl,
  } = auth.settingsStore;
  const { setPortalRename, getAllSettings } = setup;
  const { isLoaded, setIsLoadedPortalRenaming, initSettings, setIsLoaded } =
    common;

  return {
    theme,
    setPortalRename,
    isLoaded,
    setIsLoadedPortalRenaming,
    tenantAlias,
    initSettings,
    setIsLoaded,
    getAllSettings,
    domain: baseDomain,
    currentColorScheme,
    renamingSettingsUrl,
  };
})(
  withLoading(withTranslation(["Settings", "Common"])(observer(PortalRenaming)))
);
