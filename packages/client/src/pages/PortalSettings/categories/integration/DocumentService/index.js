import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import * as Styled from "./index.styled";
import {
  Link,
  Button,
  Heading,
  HelpButton,
  InputBlock,
  Label,
  Text,
} from "@docspace/components";
import toastr from "@docspace/components/toast/toastr";
import Loaders from "@docspace/common/components/Loaders";

const URL_REGEX = /^https?:\/\/[-a-zA-Z0-9@:%._\+~#=]{1,256}\/?$/;
const EDITOR_URL_PLACEHOLDER = `${window.location.protocol}//<editors-dns-name>/`;

const DocumentService = ({
  getDocumentServiceLocation,
  changeDocumentServiceLocation,
  currentColorScheme,
  integrationSettingsUrl,
}) => {
  const { t, ready } = useTranslation(["Settings", "Common"]);

  const [isLoading, setIsLoading] = useState(true);
  const [isSaveLoading, setSaveIsLoading] = useState(false);
  const [isResetLoading, setResetIsLoading] = useState(false);

  const [isDefaultSettings, setIsDefaultSettiings] = useState(false);
  const [portalUrl, setPortalUrl] = useState("");
  const [portalUrlIsValid, setPortalUrlIsValid] = useState(true);
  const [docServiceUrl, setDocServiceUrl] = useState("");
  const [docServiceUrlIsValid, setDocServiceUrlIsValid] = useState(true);
  const [internalUrl, setInternalUrl] = useState("");
  const [internalUrlIsValid, setInternalUrlIsValid] = useState(true);

  const [initPortalUrl, setInitPortalUrl] = useState("");
  const [initDocServiceUrl, setInitDocServiceUrl] = useState("");
  const [initInternalUrl, setInitInternalUrl] = useState("");

  useEffect(() => {
    setIsLoading(true);
    getDocumentServiceLocation()
      .then((result) => {
        setIsDefaultSettiings(result?.isDefault || false);

        setPortalUrl(result?.docServicePortalUrl);
        setInternalUrl(result?.docServiceUrlInternal);
        setDocServiceUrl(result?.docServiceUrl);

        setInitPortalUrl(result?.docServicePortalUrl);
        setInitInternalUrl(result?.docServiceUrlInternal);
        setInitDocServiceUrl(result?.docServiceUrl);
      })
      .catch((error) => toastr.error(error))
      .finally(() => setIsLoading(false));
  }, []);

  const onChangeDocServiceUrl = (e) => {
    setDocServiceUrl(e.target.value);
    if (!e.target.value) setDocServiceUrlIsValid(true);
    else setDocServiceUrlIsValid(URL_REGEX.test(e.target.value));
  };

  const onChangeInternalUrl = (e) => {
    setInternalUrl(e.target.value);
    if (!e.target.value) setInternalUrlIsValid(true);
    else setInternalUrlIsValid(URL_REGEX.test(e.target.value));
  };

  const onChangePortalUrl = (e) => {
    setPortalUrl(e.target.value);
    if (!e.target.value) setPortalUrlIsValid(true);
    else setPortalUrlIsValid(URL_REGEX.test(e.target.value));
  };

  const onSubmit = (e) => {
    e.preventDefault();
    setSaveIsLoading(true);
    changeDocumentServiceLocation(docServiceUrl, internalUrl, portalUrl)
      .then((response) => {
        toastr.success(t("Common:ChangesSavedSuccessfully"));

        setDocServiceUrl(response[0]);
        setInternalUrl(response[1]);
        setPortalUrl(response[2]);

        setInitDocServiceUrl(response[0]);
        setInitInternalUrl(response[1]);
        setInitPortalUrl(response[2]);

        setIsDefaultSettiings(false);
      })
      .catch((e) => toastr.error(e))
      .finally(() => setSaveIsLoading(false));
  };

  const onReset = () => {
    setDocServiceUrlIsValid(true);
    setInternalUrlIsValid(true);
    setPortalUrlIsValid(true);

    setResetIsLoading(true);
    changeDocumentServiceLocation(null, null, null)
      .then((response) => {
        toastr.success(t("Common:ChangesSavedSuccessfully"));

        setDocServiceUrl(response[0]);
        setInternalUrl(response[1]);
        setPortalUrl(response[2]);

        setInitDocServiceUrl(response[0]);
        setInitInternalUrl(response[1]);
        setInitPortalUrl(response[2]);

        setIsDefaultSettiings(true);
      })
      .catch((e) => toastr.error(e))
      .finally(() => setResetIsLoading(false));
  };

  const isFormEmpty = !docServiceUrl && !internalUrl && !portalUrl;
  const allInputsValid =
    docServiceUrlIsValid && internalUrlIsValid && portalUrlIsValid;

  const isValuesInit =
    docServiceUrl == initDocServiceUrl &&
    internalUrl == initInternalUrl &&
    portalUrl == initPortalUrl;

  if (isLoading || !ready) return <Loaders.SettingsDSConnect />;

  return (
    <Styled.Location>
      <Styled.LocationHeader>
        <div className="main">
          <Heading className={"heading"} isInline level={3}>
            {t("Settings:DocumentServiceLocationHeader")}
          </Heading>
          <div className="help-button-wrapper">
            <HelpButton
              tooltipContent={t("Settings:DocumentServiceLocationHeaderHelp")}
            />
          </div>
        </div>
        <div className="secondary">
          {t("Settings:DocumentServiceLocationHeaderInfo")}
        </div>
        <div>
          <Link
            className="third-party-link"
            color={currentColorScheme.main.accent}
            isHovered
            target="_blank"
            href={integrationSettingsUrl}
          >
            {t("Common:LearnMore")}
          </Link>
        </div>
      </Styled.LocationHeader>

      <Styled.LocationForm onSubmit={onSubmit}>
        <div className="form-inputs">
          <div className="input-wrapper">
            <Label
              htmlFor="docServiceAdress"
              text={t("Settings:DocumentServiceLocationUrlApi")}
            />
            <InputBlock
              id="docServiceAdress"
              type="text"
              autoComplete="off"
              tabIndex={1}
              scale
              iconButtonClassName={"icon-button"}
              value={docServiceUrl}
              onChange={onChangeDocServiceUrl}
              placeholder={EDITOR_URL_PLACEHOLDER}
              hasError={!docServiceUrlIsValid}
              isDisabled={isSaveLoading || isResetLoading}
            />
            <Text className="subtitle">
              {t("Common:Example", {
                example: EDITOR_URL_PLACEHOLDER,
              })}
            </Text>
          </div>
          <div className="input-wrapper">
            <Label
              htmlFor="internalAdress"
              text={t("Settings:DocumentServiceLocationUrlInternal")}
            />
            <InputBlock
              id="internalAdress"
              type="text"
              autoComplete="off"
              tabIndex={2}
              scale
              iconButtonClassName={"icon-button"}
              value={internalUrl}
              onChange={onChangeInternalUrl}
              placeholder={EDITOR_URL_PLACEHOLDER}
              hasError={!internalUrlIsValid}
              isDisabled={isSaveLoading || isResetLoading}
            />
            <Text className="subtitle">
              {t("Common:Example", {
                example: EDITOR_URL_PLACEHOLDER,
              })}
            </Text>
          </div>
          <div className="input-wrapper">
            <Label
              htmlFor="portalAdress"
              text={t("Settings:DocumentServiceLocationUrlPortal")}
            />
            <InputBlock
              id="portalAdress"
              type="text"
              autoComplete="off"
              tabIndex={3}
              scale
              iconButtonClassName={"icon-button"}
              value={portalUrl}
              onChange={onChangePortalUrl}
              placeholder={"http://<docspace-dns-name>/"}
              hasError={!portalUrlIsValid}
              isDisabled={isSaveLoading || isResetLoading}
            />
            <Text className="subtitle">
              {t("Common:Example", {
                example: `${window.location.origin}`,
              })}
            </Text>
          </div>
        </div>
        <div className="form-buttons">
          <Button
            onClick={onSubmit}
            className="button"
            primary
            size={"small"}
            label={t("Common:SaveButton")}
            isDisabled={
              isFormEmpty ||
              isValuesInit ||
              !allInputsValid ||
              isSaveLoading ||
              isResetLoading
            }
            isLoading={isSaveLoading}
          />
          <Button
            onClick={onReset}
            className="button"
            size={"small"}
            label={t("Settings:RestoreDefaultButton")}
            isDisabled={isDefaultSettings || isSaveLoading || isResetLoading}
            isLoading={isResetLoading}
          />
        </div>
      </Styled.LocationForm>
    </Styled.Location>
  );
};

export default inject(({ auth, settingsStore }) => {
  const { currentColorScheme, integrationSettingsUrl } = auth.settingsStore;
  const { getDocumentServiceLocation, changeDocumentServiceLocation } =
    settingsStore;
  return {
    getDocumentServiceLocation,
    changeDocumentServiceLocation,
    currentColorScheme,
    integrationSettingsUrl,
  };
})(observer(DocumentService));
