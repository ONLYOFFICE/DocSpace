import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import Heading from "@docspace/components/heading";
import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";
import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast";

import {
  StyledEditLinkPanel,
  StyledScrollbar,
  StyledButtons,
} from "./StyledEditLinkPanel";

import LinkBlock from "./LinkBlock";
import ToggleBlock from "./ToggleBlock";
import PasswordAccessBlock from "./PasswordAccessBlock";
import LimitTimeBlock from "./LimitTimeBlock";

const EditLinkPanel = ({ t, visible, setIsVisible, isEdit }) => {
  const [passwordAccessIsChecked, setPasswordAccessIsChecked] = useState(true);
  const [limitByTimeIsChecked, setLimitByTimeIsChecked] = useState(false);
  const [denyDownload, setDenyDownload] = useState(false);

  const onPasswordAccessChange = () =>
    setPasswordAccessIsChecked(!passwordAccessIsChecked);

  const onLimitByTimeChange = () =>
    setLimitByTimeIsChecked(!limitByTimeIsChecked);

  const onDenyDownloadChange = () => setDenyDownload(!denyDownload);

  const onClose = () => setIsVisible(false);
  const onSave = () => {
    toastr(t("LinkEditedSuccessfully"));
    onClose();
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
            <LinkBlock t={t} />
            <PasswordAccessBlock
              t={t}
              headerText={t("Files:PasswordAccess")}
              bodyText={t("Files:PasswordLink")}
              isChecked={passwordAccessIsChecked}
              onChange={onPasswordAccessChange}
            />
            <LimitTimeBlock
              headerText={t("Files:LimitByTimePeriod")}
              bodyText={t("Files:ChooseExpirationDate")}
              isChecked={limitByTimeIsChecked}
              onChange={onLimitByTimeChange}
            />
            <ToggleBlock
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
            onClick={onSave}
          />
          <Button
            scale
            size="normal"
            label={t("Common:CancelButton")}
            onClick={onClose}
          />
        </StyledButtons>
      </Aside>
    </StyledEditLinkPanel>
  );
};

export default inject(({ dialogsStore }) => {
  const { editLinkPanelIsVisible, setEditLinkPanelIsVisible, linkIsEdit } =
    dialogsStore;

  return {
    visible: editLinkPanelIsVisible,
    setIsVisible: setEditLinkPanelIsVisible,
    isEdit: linkIsEdit,
  };
})(
  withTranslation(["SharingPanel", "Common", "Files"])(observer(EditLinkPanel))
);
