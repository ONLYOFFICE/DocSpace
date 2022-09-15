import React, { useState, useEffect } from "react";
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

const StyledComponent = styled.div`
  .link {
    font-weight: 600;
    border-bottom: 1px dashed #333333;
    border-color: ${(props) => !props.isPortalPaid && "#A3A9AE"};
  }

  .description,
  .link {
    color: ${(props) => !props.isPortalPaid && "#A3A9AE"};
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
    isPortalPaid,
    getCompanyInfoSettings,
    setCompanyInfoSettings,
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
    tReady,
    setIsLoadedCompanyInfoSettingsData,
    isLoadedCompanyInfoSettingsData,
    buildVersionInfo,
    personal,
  } = props;

  const [companyName, setCompanyName] = useState(
    companyInfoSettingsData.companyName
  );
  const [email, setEmail] = useState(companyInfoSettingsData.email);
  const [phone, setPhone] = useState(companyInfoSettingsData.phone);
  const [site, setSite] = useState(companyInfoSettingsData.site);
  const [address, setAddress] = useState(companyInfoSettingsData.address);

  const [hasErrorSite, setHasErrorSite] = useState(false);
  const [hasErrorEmail, setHasErrorEmail] = useState(false);
  const [hasErrorCompanyName, setHasErrorCompanyName] = useState(false);
  const [hasErrorPhone, setHasErrorPhone] = useState(false);
  const [hasErrorAddress, setHasErrorAddress] = useState(false);

  const [isChangesSettings, setIsChangesSettings] = useState(false);
  const [hasChangesDefaultSettings, setHasChangesDefaultSettings] = useState(
    false
  );

  const [showModal, setShowModal] = useState(false);

  const defaultDataCompanyInfoSettings = JSON.parse(
    localStorage.getItem("defaultDataCompanyInfoSettings")
  );

  const previewData = { companyName, email, phone, site, address };

  const link = "About this program";

  useEffect(() => {
    if (!(companyInfoSettingsData && tReady)) return;

    setIsLoadedCompanyInfoSettingsData(true);
  }, [companyInfoSettingsData, tReady]);

  useEffect(() => {
    setCompanyName(companyInfoSettingsData.companyName);
    setEmail(companyInfoSettingsData.email);
    setPhone(companyInfoSettingsData.phone);
    setSite(companyInfoSettingsData.site);
    setAddress(companyInfoSettingsData.address);
  }, [companyInfoSettingsData]);

  useEffect(() => {
    const settings = {
      address,
      companyName,
      email,
      phone,
      site,
    };

    const dataСompanyInfoSettings = {
      address: companyInfoSettingsData.address,
      companyName: companyInfoSettingsData.companyName,
      email: companyInfoSettingsData.email,
      phone: companyInfoSettingsData.phone,
      site: companyInfoSettingsData.site,
    };

    const hasError =
      hasErrorSite ||
      hasErrorEmail ||
      hasErrorCompanyName ||
      hasErrorPhone ||
      hasErrorAddress;

    const noСhange = isEqual(settings, dataСompanyInfoSettings);

    const hasСhangeDefault = !isEqual(settings, defaultDataCompanyInfoSettings);

    if (hasСhangeDefault) {
      setHasChangesDefaultSettings(true);
    } else {
      setHasChangesDefaultSettings(false);
    }

    if (!(hasError || noСhange)) {
      setIsChangesSettings(true);
    } else {
      setIsChangesSettings(false);
    }
  }, [
    address,
    companyName,
    email,
    phone,
    site,
    hasErrorSite,
    hasErrorEmail,
    hasErrorCompanyName,
    hasErrorPhone,
    hasErrorAddress,
    companyInfoSettingsData,
    defaultDataCompanyInfoSettings,
  ]);

  const validateUrl = (url) => {
    const urlRegex = /^(ftp|http|https):\/\/[^ "]+$/;
    const hasError = !urlRegex.test(url);

    setHasErrorSite(hasError);
  };

  const validateEmail = (email) => {
    const emailRegex = /.+@.+\..+/;
    const hasError = !emailRegex.test(email);

    setHasErrorEmail(hasError);
  };

  const validateEmpty = (value, type) => {
    const hasError = value.trim() === "";

    if (type === "companyName") {
      setHasErrorCompanyName(hasError);
    }

    if (type === "phone") {
      setHasErrorPhone(hasError);
    }

    if (type === "address") {
      setHasErrorAddress(hasError);
    }
  };

  const onChangeSite = (url) => {
    validateUrl(url);
    setSite(url);
  };

  const onChangeEmail = (email) => {
    validateEmail(email);
    setEmail(email);
  };

  const onChangeСompanyName = (companyName) => {
    validateEmpty(companyName, "companyName");
    setCompanyName(companyName);
  };

  const onChangePhone = (phone) => {
    validateEmpty(phone, "phone");
    setPhone(phone);
  };

  const onChangeAddress = (address) => {
    validateEmpty(address, "address");
    setAddress(address);
  };

  const onSave = () => {
    setCompanyInfoSettings(address, companyName, email, phone, site)
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getCompanyInfoSettings();
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  const onRestore = () => {
    restoreCompanyInfoSettings()
      .then(() => {
        toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
        getCompanyInfoSettings();
      })
      .catch((error) => {
        toastr.error(error);
      });
  };

  const onShowExample = () => {
    if (!isPortalPaid) return;

    setShowModal(true);
  };

  const onCloseModal = () => {
    setShowModal(false);
  };

  return !isLoadedCompanyInfoSettingsData ? (
    <LoaderCompanyInfoSettings />
  ) : (
    <>
      <AboutDialog
        visible={showModal}
        onClose={onCloseModal}
        buildVersionInfo={buildVersionInfo}
        personal={personal}
        previewData={previewData}
      />

      <StyledComponent isPortalPaid={isPortalPaid}>
        <div className="header">{t("Settings:CompanyInfoSettings")}</div>
        <div className="description">
          <Trans t={t} i18nKey="CompanyInfoSettingsDescription" ns="Settings">
            "This information will be displayed in the
            {isPortalPaid ? (
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
            className="field-container-width"
            labelText={t("Common:CompanyName")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerCompanyName"
              className="text-input"
              isDisabled={!isPortalPaid}
              scale={true}
              value={companyName}
              hasError={hasErrorCompanyName}
              onChange={(e) => onChangeСompanyName(e.target.value)}
              tabIndex={3}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerEmail"
            isDisabled={!isPortalPaid}
            className="field-container-width"
            labelText={t("Common:Email")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerEmail"
              className="text-input"
              isDisabled={!isPortalPaid}
              scale={true}
              value={email}
              hasError={hasErrorEmail}
              onChange={(e) => onChangeEmail(e.target.value)}
              tabIndex={4}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerPhone"
            className="field-container-width"
            labelText={t("Common:Phone")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerPhone"
              className="text-input"
              isDisabled={!isPortalPaid}
              scale={true}
              value={phone}
              hasError={hasErrorPhone}
              onChange={(e) => onChangePhone(e.target.value)}
              tabIndex={5}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerWebsite"
            className="field-container-width"
            labelText={t("Common:Website")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerWebsite"
              className="text-input"
              isDisabled={!isPortalPaid}
              scale={true}
              value={site}
              hasError={hasErrorSite}
              onChange={(e) => onChangeSite(e.target.value)}
              tabIndex={6}
            />
          </FieldContainer>
          <FieldContainer
            id="fieldContainerAddress"
            className="field-container-width"
            labelText={t("Common:Address")}
            isVertical={true}
          >
            <TextInput
              id="textInputContainerAddress"
              className="text-input"
              isDisabled={!isPortalPaid}
              scale={true}
              value={address}
              hasError={hasErrorAddress}
              onChange={(e) => onChangeAddress(e.target.value)}
              tabIndex={7}
            />
          </FieldContainer>
        </div>
        <SaveCancelButtons
          className="save-cancel-buttons"
          onSaveClick={onSave}
          onCancelClick={onRestore}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          displaySettings={true}
          showReminder={isPortalPaid && isChangesSettings}
          disableRestoreToDefault={!hasChangesDefaultSettings}
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
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
    buildVersionInfo,
    personal,
  } = settingsStore;

  return {
    getCompanyInfoSettings,
    setCompanyInfoSettings,
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
