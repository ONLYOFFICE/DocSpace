import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import * as Styled from "./index.styled";
import {
  Button,
  Heading,
  HelpButton,
  InputBlock,
  Label,
} from "@docspace/components";
import toastr from "@docspace/components/toast/toastr";

const URL_REGEX = /^https?:\/\/[-a-zA-Z0-9@:%._\+~#=]{1,256}\/?$/;

const DocumentService = ({ changeDocumentServiceLocation }) => {
  const { t } = useTranslation(["Settings", "Common"]);

  const [apiUrl, setApiUrl] = useState("");
  const [apiUrlIsValid, setApiUrlIsValid] = useState(true);
  const onChangeApiUrl = (e) => {
    setApiUrl(e.target.value);
    if (!e.target.value) setApiUrlIsValid(true);
    else setApiUrlIsValid(URL_REGEX.test(e.target.value));
  };

  const [internalUrl, setInternalUrl] = useState("");
  const [internalUrlIsValid, setInternalUrlIsValid] = useState(true);
  const onChangeInternalUrl = (e) => {
    setInternalUrl(e.target.value);
    if (!e.target.value) setInternalUrlIsValid(true);
    else setInternalUrlIsValid(URL_REGEX.test(e.target.value));
  };

  const [portalUrl, setPortalUrl] = useState("");
  const [portalUrlIsValid, setPortalUrlIsValid] = useState(true);
  const onChangePortalUrl = (e) => {
    setPortalUrl(e.target.value);
    if (!e.target.value) setPortalUrlIsValid(true);
    else setPortalUrlIsValid(URL_REGEX.test(e.target.value));
  };

  const onSubmit = (e) => {
    e.preventDefault();
    changeDocumentServiceLocation(apiUrl, internalUrl, portalUrl)
      .then(() => toastr.success("Success"))
      .catch((e) =>
        toastr.error(`${e.message}:\n${e.response.data.error.message}`)
      );
  };

  const onReset = () => {
    setApiUrl("");
    setInternalUrl("");
    setPortalUrl("");
    setApiUrlIsValid(true);
    setInternalUrlIsValid(true);
    setPortalUrlIsValid(true);
  };

  const isFormEmpty = !apiUrl && !internalUrl && !portalUrl;
  const allInputsValid =
    apiUrlIsValid && internalUrlIsValid && portalUrlIsValid;

  const anyInputFilled = apiUrl || internalUrl || portalUrl;

  useEffect(() => {
    const fetchDocumentServiceLocation = async () => {
      const result = await changeDocumentServiceLocation();
      console.log(result);
    };
    fetchDocumentServiceLocation();
  }, []);

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
      </Styled.LocationHeader>

      <Styled.LocationForm onSubmit={onSubmit}>
        <div className="form-inputs">
          <div className="input-wrapper">
            <Label
              htmlFor="editingAdress"
              text={t("Settings:DocumentServiceLocationUrlApi")}
            />
            <InputBlock
              id="editingAdress"
              type="text"
              autocomplete="off"
              tabindex={1}
              scale
              iconButtonClassName={"icon-button"}
              value={apiUrl}
              onChange={onChangeApiUrl}
              placeholder={"http://<editors-dns-name>/"}
              hasError={!apiUrlIsValid}
            />
          </div>
          <div className="input-wrapper">
            <Label
              htmlFor="fromDocspaceAdress"
              text={t("Settings:DocumentServiceLocationUrlInternal")}
            />
            <InputBlock
              id="fromDocspaceAdress"
              type="text"
              autocomplete="off"
              tabindex={2}
              scale
              iconButtonClassName={"icon-button"}
              value={internalUrl}
              onChange={onChangeInternalUrl}
              placeholder={"http://<editors-dns-name>/"}
              hasError={!internalUrlIsValid}
            />
          </div>
          <div className="input-wrapper">
            <Label
              htmlFor="fromDocServiceAdress"
              text={t("Settings:DocumentServiceLocationUrlPortal")}
            />
            <InputBlock
              id="fromDocServiceAdress"
              type="text"
              autocomplete="off"
              tabindex={3}
              scale
              iconButtonClassName={"icon-button"}
              value={portalUrl}
              onChange={onChangePortalUrl}
              placeholder={"http://<win-nvplrl2avjo/"}
              hasError={!portalUrlIsValid}
            />
          </div>
        </div>
        <div className="form-buttons">
          <Button
            onClick={onSubmit}
            className="button"
            primary
            size={"small"}
            label={t("Common:SaveButton")}
            // isDisabled={isFormEmpty || !allInputsValid}
          />
          <Button
            onClick={onReset}
            className="button"
            size={"small"}
            label={t("Common:ResetButton")}
            isDisabled={!anyInputFilled}
          />
        </div>
      </Styled.LocationForm>
    </Styled.Location>
  );
};

export default inject(({ auth, settingsStore }) => {
  return {
    changeDocumentServiceLocation: settingsStore.changeDocumentServiceLocation,
  };
})(observer(DocumentService));
