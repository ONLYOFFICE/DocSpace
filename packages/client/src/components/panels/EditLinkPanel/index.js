import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import Heading from "@docspace/components/heading";
import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import {
  StyledEditLinkPanel,
  StyledScrollbar,
  StyledButtons,
} from "./StyledEditLinkPanel";

import LinkBlock from "./LinkBlock";
import ToggleBlock from "./ToggleBlock";
import PasswordAccessBlock from "./PasswordAccessBlock";
import LimitTimeBlock from "./LimitTimeBlock";

const EditLinkPanel = (props) => {
  const {
    t,
    visible,
    roomId,
    setIsVisible,
    isEdit,
    editExternalLink,
    linkId,
    title,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  const [linkNameValue, setLinkNameValue] = useState(title); //t("ExternalLink")
  const [passwordValue, setPasswordValue] = useState("");
  const [expirationDate, setExpirationDate] = useState("");

  const [passwordAccessIsChecked, setPasswordAccessIsChecked] = useState(false);
  const [limitByTimeIsChecked, setLimitByTimeIsChecked] = useState(false);
  const [denyDownload, setDenyDownload] = useState(false);

  const onPasswordAccessChange = () =>
    setPasswordAccessIsChecked(!passwordAccessIsChecked);

  const onLimitByTimeChange = () =>
    setLimitByTimeIsChecked(!limitByTimeIsChecked);

  const onDenyDownloadChange = () => setDenyDownload(!denyDownload);

  const onClose = () => setIsVisible(false);
  const onSave = () => {
    setIsLoading(true);

    const options = {
      linkId,
      roomId,
      title: linkNameValue,
      expirationDate,
      password: passwordValue,
      denyDownload,
    };

    editExternalLink(options)
      .then(() => toastr.success(t("LinkEditedSuccessfully")))
      .catch((err) => toastr.error(err?.message))
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  };

  return (
    <StyledEditLinkPanel>
      <Backdrop
        onClick={onClose}
        visible={visible}
        isAside={true}
        zIndex={210}
      />
      <Aside className="edit-link-panel" visible={visible} onClose={onClose}>
        <div className="edit-link_header">
          <Heading className="edit-link_heading">
            {isEdit ? t("Files:EditLink") : t("Files:AddNewLink")}
          </Heading>
        </div>
        <StyledScrollbar stype="mediumBlack">
          <div className="edit-link_body">
            <LinkBlock
              t={t}
              isLoading={isLoading}
              linkNameValue={linkNameValue}
              setLinkNameValue={setLinkNameValue}
            />
            <PasswordAccessBlock
              t={t}
              isLoading={isLoading}
              headerText={t("Files:PasswordAccess")}
              bodyText={t("Files:PasswordLink")}
              isChecked={passwordAccessIsChecked}
              passwordValue={passwordValue}
              setPasswordValue={setPasswordValue}
              onChange={onPasswordAccessChange}
            />
            <LimitTimeBlock
              isLoading={isLoading}
              headerText={t("Files:LimitByTimePeriod")}
              bodyText={t("Files:ChooseExpirationDate")}
              isChecked={limitByTimeIsChecked}
              onChange={onLimitByTimeChange}
            />
            <ToggleBlock
              isLoading={isLoading}
              headerText={t("Files:DenyDownload")}
              bodyText={t("Files:PreventDownloadFilesAndFolders")}
              isChecked={denyDownload}
              onChange={onDenyDownloadChange}
            />
          </div>
        </StyledScrollbar>

        <StyledButtons>
          <Button
            scale
            primary
            size="normal"
            label={t("Common:SaveButton")}
            isDisabled={isLoading}
            onClick={onSave}
          />
          <Button
            scale
            size="normal"
            label={t("Common:CancelButton")}
            isDisabled={isLoading}
            onClick={onClose}
          />
        </StyledButtons>
      </Aside>
    </StyledEditLinkPanel>
  );
};

export default inject(({ auth, dialogsStore, publicRoomStore }) => {
  const { selectionParentRoom } = auth.infoPanelStore;
  const { editLinkPanelIsVisible, setEditLinkPanelIsVisible, linkParams } =
    dialogsStore;
  const { externalLinks, editExternalLink } = publicRoomStore;
  const { isEdit, linkId } = linkParams;

  const link = externalLinks.find((l) => l?.sharedTo?.id === linkId);

  const template = externalLinks.find((t) => t?.sharedTo?.isTemplate);

  return {
    visible: editLinkPanelIsVisible,
    setIsVisible: setEditLinkPanelIsVisible,
    isEdit,
    linkId: link?.sharedTo?.id ?? template?.sharedTo?.id,
    title: link?.sharedTo?.title,
    editExternalLink,
    roomId: selectionParentRoom.id,
  };
})(
  withTranslation(["SharingPanel", "Common", "Files"])(observer(EditLinkPanel))
);
