import React, { useState, useEffect, useRef } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import copy from "copy-to-clipboard";
import isEqual from "lodash/isEqual";
// import { createPasswordHash } from "@docspace/common/utils";

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
    roomId,
    linkId,
    isEdit,
    visible,
    password,
    setIsVisible,
    editExternalLink,
    setExternalLinks,
    shareLink,
    unsavedChangesDialogVisible,
    setUnsavedChangesDialog,
    // hashSettings,
    isLocked,
    disabled,
    isDenyDownload,
    link,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  const [linkNameValue, setLinkNameValue] = useState("");
  const [passwordValue, setPasswordValue] = useState(password);
  const [expirationDate, setExpirationDate] = useState("");

  const [isPasswordValid, setIsPasswordValid] = useState(true);
  const [isLinkNameValid, setIsLinkNameValid] = useState(true);

  const [linkValue, setLinkValue] = useState(shareLink);
  const [hasChanges, setHasChanges] = useState(false);

  const [passwordAccessIsChecked, setPasswordAccessIsChecked] =
    useState(isLocked);
  const [limitByTimeIsChecked, setLimitByTimeIsChecked] = useState(false);
  const [denyDownload, setDenyDownload] = useState(isDenyDownload);

  const onPasswordAccessChange = () =>
    setPasswordAccessIsChecked(!passwordAccessIsChecked);

  const onLimitByTimeChange = () =>
    setLimitByTimeIsChecked(!limitByTimeIsChecked);

  const onDenyDownloadChange = () => setDenyDownload(!denyDownload);

  const onClosePanel = () => {
    hasChanges ? setUnsavedChangesDialog(true) : onClose();
  };

  const onClose = () => setIsVisible(false);
  const onSave = () => {
    let err = false;

    const isPasswordValid = !!passwordValue.trim();

    if (!isPasswordValid && passwordAccessIsChecked) {
      setIsPasswordValid(false);
      err = true;
    }

    const isLinkNameValid = !!linkNameValue.trim();

    if (!isLinkNameValid) {
      setIsLinkNameValid(false);
      err = true;
    }

    if (err) return;

    // const passwordHash = passwordAccessIsChecked
    //   ? createPasswordHash(passwordValue, hashSettings)
    //   : null;

    link.sharedTo.title = linkNameValue;
    link.sharedTo.password = passwordAccessIsChecked ? passwordValue : null;
    link.sharedTo.denyDownload = denyDownload;
    // link.sharedTo.expirationDate=expirationDate;

    setIsLoading(true);
    editExternalLink(roomId, link)
      .then((res) => {
        setExternalLinks(res);

        const link = res.find((l) => l?.sharedTo?.id === linkId);

        if (isEdit) {
          copy(linkValue);
          toastr.success(t("Files:LinkEditedSuccessfully"));
        } else {
          copy(link?.sharedTo?.shareLink);

          toastr.success(t("Files:LinkAddedSuccessfully"));
        }
      })
      .catch((err) => toastr.error(err?.message))
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  };

  const initState = {
    passwordValue: password,
    //expirationDate
    passwordAccessIsChecked: isLocked,
    //limitByTimeIsChecked,
    //denyDownload
  };

  useEffect(() => {
    const data = {
      passwordValue,
      //expirationDate,
      passwordAccessIsChecked,
      //limitByTimeIsChecked,
      //denyDownload,
    };

    if (!isEqual(data, initState)) {
      setHasChanges(true);
    } else setHasChanges(false);
  });

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      !unsavedChangesDialogVisible && onSave();
    }
  };

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, [unsavedChangesDialogVisible]);

  return (
    <StyledEditLinkPanel>
      <Backdrop
        onClick={onClosePanel}
        visible={visible}
        isAside={true}
        zIndex={210}
      />
      <Aside
        className="edit-link-panel"
        visible={visible}
        onClose={onClosePanel}
      >
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
              shareLink={shareLink}
              linkNameValue={linkNameValue}
              setLinkNameValue={setLinkNameValue}
              linkValue={linkValue}
              setLinkValue={setLinkValue}
              isLinkNameValid={isLinkNameValid}
              setIsLinkNameValid={setIsLinkNameValid}
            />
            <PasswordAccessBlock
              t={t}
              isLoading={isLoading}
              headerText={t("Files:PasswordAccess")}
              bodyText={t("Files:PasswordLink")}
              isChecked={passwordAccessIsChecked}
              isPasswordValid={isPasswordValid}
              passwordValue={passwordValue}
              setPasswordValue={setPasswordValue}
              setIsPasswordValid={setIsPasswordValid}
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
  // const { hashSettings } = auth.settingsStore;
  const {
    editLinkPanelIsVisible,
    setEditLinkPanelIsVisible,
    unsavedChangesDialogVisible,
    setUnsavedChangesDialog,
    linkParams,
  } = dialogsStore;
  const { externalLinks, editExternalLink, setExternalLinks } = publicRoomStore;
  const { isEdit } = linkParams;

  const linkId = linkParams?.link?.sharedTo?.id;
  const link = externalLinks.find((l) => l?.sharedTo?.id === linkId);
  const template = externalLinks.find((t) => t?.sharedTo?.isTemplate);
  const shareLink = link?.sharedTo?.shareLink ?? template?.sharedTo?.shareLink;

  return {
    visible: editLinkPanelIsVisible,
    setIsVisible: setEditLinkPanelIsVisible,
    isEdit,
    linkId: link?.sharedTo?.id ?? template?.sharedTo?.id,

    disabled: link?.sharedTo?.disabled,
    editExternalLink,
    roomId: selectionParentRoom.id,
    setExternalLinks,
    isLocked: !!link?.sharedTo?.password,
    password: link?.sharedTo?.password ?? "",
    isDenyDownload: link?.sharedTo?.denyDownload,
    shareLink,
    externalLinks,
    unsavedChangesDialogVisible,
    setUnsavedChangesDialog,
    // hashSettings,
    link: link ?? template,
  };
})(
  withTranslation(["SharingPanel", "Common", "Files"])(observer(EditLinkPanel))
);
