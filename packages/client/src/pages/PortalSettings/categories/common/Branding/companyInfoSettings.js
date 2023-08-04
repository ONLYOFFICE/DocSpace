import React, { useState, useEffect, useCallback } from "react";
import { Trans, withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import isEqual from "lodash/isEqual";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";
import Link from "@docspace/components/link";
import LoaderCompanyInfoSettings from "../sub-components/loaderCompanyInfoSettings";
import AboutDialog from "../../../../About/AboutDialog";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";

const StyledComponent = styled.div`
  .link {
    font-weight: 600;
    border-bottom: 1px dashed #333333;
    border-color: ${(props) => !props.isSettingPaid && "#A3A9AE"};
  }

  .description,
  .link {
    color: ${(props) => !props.isSettingPaid && "#A3A9AE"};
  }

  .text-input {
    font-size: 13px;
  }

  .save-cancel-buttons {
    margin-top: 24px;
  }
`;

const CompanyInfoSettings = (props) => {
  const {
    t,
    isSettingPaid,
    getCompanyInfoSettings,
    setCompanyInfoSettings,
    companyInfoSettingsIsDefault,
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
    tReady,
    setIsLoadedCompanyInfoSettingsData,
    isLoadedCompanyInfoSettingsData,
    buildVersionInfo,
    personal,
  } = props;

  const defaultCompanySettingsError = {
    hasErrorAddress: false,
    hasErrorCompanyName: false,
    hasErrorEmail: false,
    hasErrorPhone: false,
    hasErrorSite: false,
  };

  const [companySettings, setCompanySettings] = useState({});
  const [companySettingsError, setCompanySettingsError] = useState(
    defaultCompanySettingsError
  );
  const [showReminder, setShowReminder] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);

  const { address, companyName, email, phone, site } = companySettings;
  const {
    hasErrorAddress,
    hasErrorCompanyName,
    hasErrorEmail,
    hasErrorPhone,
    hasErrorSite,
  } = companySettingsError;

  const link = t("Common:AboutCompanyTitle");

  useEffect(() => {
    if (!(companyInfoSettingsData && tReady)) return;

    setIsLoadedCompanyInfoSettingsData(true);
  }, [companyInfoSettingsData, tReady]);

  const getSettings = () => {
    const companySettings = getFromSessionStorage("companySettings");

    const defaultData = {
      address: companyInfoSettingsData.address,
      companyName: companyInfoSettingsData.companyName,
      email: companyInfoSettingsData.email,
      phone: companyInfoSettingsData.phone,
      site: companyInfoSettingsData.site,
    };

    saveToSessionStorage("defaultCompanySettings", defaultData);

    if (companySettings) {
      setCompanySettings({
        address: companySettings.address,
        companyName: companySettings.companyName,
        email: companySettings.email,
        phone: companySettings.phone,
        site: companySettings.site,
      });
    } else {
      setCompanySettings(defaultData);
    }
  };

  useEffect(() => {
    getSettings();
  }, [isLoading]);

  useEffect(() => {
    const defaultCompanySettings = getFromSessionStorage(
      "defaultCompanySettings"
    );

    const newSettings = {
      address: companySettings.address,
      companyName: companySettings.companyName,
      email: companySettings.email,
      phone: companySettings.phone,
      site: companySettings.site,
    };

    saveToSessionStorage("companySettings", newSettings);

    if (isEqual(defaultCompanySettings, newSettings)) {
      setShowReminder(false);
    } else {
      setShowReminder(true);
    }
  }, [companySettings, companyInfoSettingsData]);

  const validateSite = (site) => {
    const urlRegex = /^(ftp|http|https):\/\/[^ "]+$/;
    const hasErrorSite = !urlRegex.test(site);

    setCompanySettingsError({ ...companySettingsError, hasErrorSite });
  };

  const validateEmail = (email) => {
    const emailRegex = /.+@.+\..+/;
    const hasErrorEmail = !emailRegex.test(email);

    setCompanySettingsError({ ...companySettingsError, hasErrorEmail });
  };

  const validateEmpty = (value, type) => {
    const hasError = value.trim() === "";
    const phoneRegex = /^[\d\(\)\-+]+$/;
    const hasErrorPhone = !phoneRegex.test(value);

    if (type === "companyName") {
      setCompanySettingsError({
        ...companySettingsError,
        hasErrorCompanyName: hasError,
      });
    }

    if (type === "phone") {
      setCompanySettingsError({
        ...companySettingsError,
        hasErrorPhone,
      });
    }

    if (type === "address") {
      setCompanySettingsError({
        ...companySettingsError,
        hasErrorAddress: hasError,
      });
    }
  };

  const onChangeSite = (e) => {
    const site = e.target.value;
    validateSite(site);
    setCompanySettings({ ...companySettings, site });
    saveToSessionStorage("companySettings", { ...companySettings, site });
  };

  const onChangeEmail = (e) => {
    const email = e.target.value;
    validateEmail(email);
    setCompanySettings({ ...companySettings, email });
    saveToSessionStorage("companySettings", { ...companySettings, email });
  };

  const onChangeСompanyName = (e) => {
    const companyName = e.target.value;
    validateEmpty(companyName, "companyName");
    setCompanySettings({ ...companySettings, companyName });
    saveToSessionStorage("companySettings", {
      ...companySettings,
      companyName,
    });
  };

  const onChangePhone = (e) => {
    const phone = e.target.value;
    validateEmpty(phone, "phone");
    setCompanySettings({ ...companySettings, phone });
    saveToSessionStorage("companySettings", { ...companySettings, phone });
  };

  const onChangeAddress = (e) => {
    const address = e.target.value;
    validateEmpty(address, "address");
    setCompanySettings({ ...companySettings, address });
    saveToSessionStorage("companySettings", { ...companySettings, address });
  };

  const onSave = useCallback(async () => {
    setIsLoading(true);

    await setCompanyInfoSettings(address, companyName, email, phone, site)
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
      })
      .catch((error) => {
        toastr.error(error);
      });

    await getCompanyInfoSettings();

    const data = {
      address,
      companyName,
      email,
      phone,
      site,
    };

    saveToSessionStorage("companySettings", data);
    saveToSessionStorage("defaultCompanySettings", data);

    setCompanySettingsError({
      hasErrorAddress: false,
      hasErrorCompanyName: false,
      hasErrorEmail: false,
      hasErrorPhone: false,
      hasErrorSite: false,
    });

    setIsLoading(false);
  }, [
    setIsLoading,
    setCompanyInfoSettings,
    getCompanyInfoSettings,
    companySettings,
  ]);

  const onRestore = useCallback(async () => {
    setIsLoading(true);

    await restoreCompanyInfoSettings()
      .then((res) => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        setCompanySettings(res);
        saveToSessionStorage("companySettings", res);
      })
      .catch((error) => {
        toastr.error(error);
      });

    await getCompanyInfoSettings();

    setCompanySettingsError({
      hasErrorAddress: false,
      hasErrorCompanyName: false,
      hasErrorEmail: false,
      hasErrorPhone: false,
      hasErrorSite: false,
    });

    setIsLoading(false);
  }, [setIsLoading, restoreCompanyInfoSettings, getCompanyInfoSettings]);

  const onShowExample = () => {
    if (!isSettingPaid) return;

    setShowModal(true);
  };

  const onCloseModal = () => {
    setShowModal(false);
  };

  if (!isLoadedCompanyInfoSettingsData) return <LoaderCompanyInfoSettings />;

  return (
    <>
      <AboutDialog
        visible={showModal}
        onClose={onCloseModal}
        buildVersionInfo={buildVersionInfo}
        personal={personal}
        previewData={companySettings}
      />

      <StyledComponent isSettingPaid={isSettingPaid}>
        <div className="header settings_unavailable">
          {t("Settings:CompanyInfoSettings")}
        </div>
        <div className="description settings_unavailable">
          <Trans t={t} i18nKey="CompanyInfoSettingsDescription" ns="Settings">
            "This information will be displayed in the
            {isSettingPaid ? (
              <Link className="link" onClick={onShowExample} noHover={true}>
                {{ link }}
              </Link>
            ) : (
              <span className="link"> {{ link }}</span>
            )}
            window."
          </Trans>
        </div>
        <div className="settings-block">
          <FieldContainer
            id="fieldContainerCompanyName"
            className="field-container-width settings_unavailable"
            labelText={t("Common:CompanyName")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerCompanyName"
              className="text-input"
              isDisabled={!isSettingPaid}
              scale={true}
              value={companyName}
              hasError={hasErrorCompanyName}
              onChange={onChangeСompanyName}
              tabIndex={5}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerEmail"
            isDisabled={!isSettingPaid}
            className="field-container-width settings_unavailable"
            labelText={t("Common:Email")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerEmail"
              className="text-input"
              isDisabled={!isSettingPaid}
              scale={true}
              value={email}
              hasError={hasErrorEmail}
              onChange={onChangeEmail}
              tabIndex={6}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerPhone"
            className="field-container-width settings_unavailable"
            labelText={t("Common:Phone")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerPhone"
              className="text-input"
              isDisabled={!isSettingPaid}
              scale={true}
              value={phone}
              hasError={hasErrorPhone}
              onChange={onChangePhone}
              tabIndex={7}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerWebsite"
            className="field-container-width settings_unavailable"
            labelText={t("Common:Website")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerWebsite"
              className="text-input"
              isDisabled={!isSettingPaid}
              scale={true}
              value={site}
              hasError={hasErrorSite}
              onChange={onChangeSite}
              tabIndex={8}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerAddress"
            className="field-container-width settings_unavailable"
            labelText={t("Common:Address")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerAddress"
              className="text-input"
              isDisabled={!isSettingPaid}
              scale={true}
              value={address}
              hasError={hasErrorAddress}
              onChange={onChangeAddress}
              tabIndex={9}
            />
          </FieldContainer>
        </div>
        <SaveCancelButtons
          tabIndex={10}
          className="save-cancel-buttons"
          onSaveClick={onSave}
          onCancelClick={onRestore}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          reminderTest={t("YouHaveUnsavedChanges")}
          displaySettings={true}
          showReminder={(isSettingPaid && showReminder) || isLoading}
          disableRestoreToDefault={companyInfoSettingsIsDefault || isLoading}
          additionalClassSaveButton="company-info-save"
          additionalClassCancelButton="company-info-cancel"
        />
      </StyledComponent>
    </>
  );
};

export default inject(({ auth, common }) => {
  const { settingsStore } = auth;

  const {
    setIsLoadedCompanyInfoSettingsData,
    isLoadedCompanyInfoSettingsData,
  } = common;

  const {
    getCompanyInfoSettings,
    setCompanyInfoSettings,
    companyInfoSettingsIsDefault,
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
    buildVersionInfo,
    personal,
  } = settingsStore;

  return {
    getCompanyInfoSettings,
    setCompanyInfoSettings,
    companyInfoSettingsIsDefault,
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
    setIsLoadedCompanyInfoSettingsData,
    isLoadedCompanyInfoSettingsData,
    buildVersionInfo,
    personal,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(CompanyInfoSettings))
  )
);
