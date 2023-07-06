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

const DocumentService = ({}) => {
  const { t } = useTranslation(["Settings", "Common"]);

  const [editingAdress, setEditingAdress] = useState("");
  const onChangeEditingAdress = (e) => setEditingAdress(e.target.value);

  const [fromDocspaceAdress, setFromDocspaceAdress] = useState("");
  const onChangeFromDocspaceAdress = (e) =>
    setFromDocspaceAdress(e.target.value);

  const [fromDocServiceAdress, setFromDocServiceAdress] = useState("");
  const onChangeFromDocServiceAdress = (e) =>
    setFromDocServiceAdress(e.target.value);

  const onSubmit = (e) => {
    e.preventDefault();
  };

  const onReset = () => {};

  return (
    <Styled.Location>
      <Styled.LocationHeader>
        <div className="main">
          <Heading className={"heading"} isInline level={3}>
            {/* Document Service Location */}
            {t("Settings:DocumentServiceLocationHeader")}
          </Heading>
          <div className="help-button-wrapper">
            <HelpButton
              tooltipContent={
                t("Settings:DocumentServiceLocationHeaderHelp")
                // "Document Service is the server service which allows to perform the document editing and allows to convert the document file into the appropriate OfficeOpen XML format."
              }
            />
          </div>
        </div>
        <div className="secondary">
          {
            t("Settings:DocumentServiceLocationHeaderInfo")
            // " Document service location specifies the address of the server with the document services installed. Please change the '<editors-dns-name>' for the server address in the below lines leaving the rest of the line exactly as it is."
          }
        </div>
      </Styled.LocationHeader>

      <Styled.LocationForm onSubmit={onSubmit}>
        <div className="form-inputs">
          <div className="input-wrapper">
            <Label
              htmlFor="editingAdress"
              text={
                // "Document Editing Service Address"
                t("Settings:DocumentServiceLocationUrlApi")
              }
            />
            <InputBlock
              id="editingAdress"
              scale
              iconButtonClassName={"icon-button"}
              value={editingAdress}
              onChange={onChangeEditingAdress}
              placeholder={"http://<editors-dns-name>/"}
            />
          </div>
          <div className="input-wrapper">
            <Label
              htmlFor="fromDocspaceAdress"
              text={
                t("Settings:DocumentServiceLocationUrlInternal")
                // "Document Service address for requests from the DocSpace"
              }
            />
            <InputBlock
              id="fromDocspaceAdress"
              scale
              iconButtonClassName={"icon-button"}
              value={fromDocspaceAdress}
              onChange={onChangeFromDocspaceAdress}
              placeholder={"http://<editors-dns-name>/"}
            />
          </div>
          <div className="input-wrapper">
            <Label
              htmlFor="fromDocServiceAdress"
              text={
                t("Settings:DocumentServiceLocationUrlPortal")
                // "DocSpace address for requests from the Document Service"
              }
            />
            <InputBlock
              id="fromDocServiceAdress"
              scale
              iconButtonClassName={"icon-button"}
              value={fromDocServiceAdress}
              onChange={onChangeFromDocServiceAdress}
              placeholder={"http://<win-nvplrl2avjo/"}
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

export default inject(({}) => {
  return {};
})(observer(DocumentService));
