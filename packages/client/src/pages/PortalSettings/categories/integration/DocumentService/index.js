import React, { useState } from "react";
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

const URL_REGEX = /^https?:\/\/[-a-zA-Z0-9@:%._\+~#=]{1,256}$/;

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
    changeDocumentServiceLocation(apiUrl, internalUrl, portalUrl);
  };

  const onReset = () => {
    setApiUrl("");
    setInternalUrl("");
    setPortalUrl("");
    setApiUrlIsValid(true);
    setInternalUrlIsValid(true);
    setPortalUrlIsValid(true);
  };

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
          />
          <Button
            onClick={onReset}
            className="button"
            size={"small"}
            label={t("Common:ResetButton")}
          />
        </div>
      </Styled.LocationForm>
    </Styled.Location>
  );
};

export default inject(({ settingsStore }) => {
  return {
    changeDocumentServiceLocation: settingsStore.changeDocumentServiceLocation,
  };
})(observer(DocumentService));
