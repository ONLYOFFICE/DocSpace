import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import FieldContainer from "@docspace/components/field-container";
import TextInput from "@docspace/components/text-input";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import { inject, observer } from "mobx-react";
import isEqual from "lodash/isEqual";
import withLoading from "SRC_DIR/HOCs/withLoading";
import styled from "styled-components";

const StyledComponent = styled.div`
  .save-cancel-buttons {
    margin-top: 8px;
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
  const [isFirstCompanyInfoSettings, setIsFirstCompanyInfoSettings] = useState(
    localStorage.getItem("isFirstCompanyInfoSettings")
  );

  useEffect(() => {
    const settings = {
      IsLicensor: true,
      address,
      companyName,
      email,
      phone,
      site,
    };

    const hasError =
      hasErrorSite ||
      hasErrorEmail ||
      hasErrorCompanyName ||
      hasErrorPhone ||
      hasErrorAddress;

    const noСhange = _.isEqual(settings, companyInfoSettingsData);

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

        if (!localStorage.getItem("isFirstCompanyInfoSettings")) {
          localStorage.setItem("isFirstCompanyInfoSettings", true);

          setIsFirstCompanyInfoSettings("true");
        }
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

  return (
    <StyledComponent>
      <div className="header">Company info settings</div>
      <div className="description">
        This information will be displayed in the About this program window.
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
            isDisabled={!isPortalPaid}
            scale={true}
            value={companyName}
            hasError={hasErrorCompanyName}
            onChange={(e) => onChangeСompanyName(e.target.value)}
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
            isDisabled={!isPortalPaid}
            scale={true}
            value={email}
            hasError={hasErrorEmail}
            onChange={(e) => onChangeEmail(e.target.value)}
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
            isDisabled={!isPortalPaid}
            scale={true}
            value={phone}
            hasError={hasErrorPhone}
            onChange={(e) => onChangePhone(e.target.value)}
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
            isDisabled={!isPortalPaid}
            scale={true}
            value={site}
            hasError={hasErrorSite}
            onChange={(e) => onChangeSite(e.target.value)}
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
            isDisabled={!isPortalPaid}
            scale={true}
            value={address}
            hasError={hasErrorAddress}
            onChange={(e) => onChangeAddress(e.target.value)}
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
        showReminder={isChangesSettings}
        isFirstRestoreToDefault={isFirstCompanyInfoSettings}
      />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {
  const { settingsStore } = auth;
  const {
    getCompanyInfoSettings,
    setCompanyInfoSettings,
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
  } = settingsStore;
  return {
    getCompanyInfoSettings,
    setCompanyInfoSettings,
    restoreCompanyInfoSettings,
    companyInfoSettingsData,
  };
})(
  withLoading(
    withTranslation(["Settings", "Common"])(observer(CompanyInfoSettings))
  )
);
