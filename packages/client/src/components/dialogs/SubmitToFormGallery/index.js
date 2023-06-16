import { Link, ModalDialog } from "@docspace/components";
import { Button } from "@docspace/components";
import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { Trans, withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
import SelectFileDialog from "@docspace/client/src/components/panels/SelectFileDialog";

export const StyledModalDialog = styled(ModalDialog)`
  .modal-body {
    display: flex;
    flex-direction: column;
    align-items: start;
    justify-content: center;
    gap: 16px;

    font-weight: 400;
    line-height: 20px;
  }
`;

export const StyledFormItem = styled.div`
  width: 100%;
  box-sizing: border-box;
  display: flex;
  flex-direction: row;
  padding: 8px 20px;
  align-items: center;
  justify-content: start;
  gap: 12px;
  border-radius: 6px;
  background: ${(props) => props.theme.infoPanel.history.fileBlockBg};

  .icon {
    width: 24px;
    height: 24px;
    svg {
      width: 24px;
      height: 24px;
    }
  }

  .item-title {
    font-weight: 600;
    font-size: 14px;
    display: flex;
    min-width: 0;
    gap: 0;

    .name {
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    }

    .exst {
      flex-shrink: 0;
      color: ${(props) => props.theme.infoPanel.history.fileExstColor};
    }
  }
`;

const SubmitToFormGallery = ({
  t,
  visible,
  setVisible,
  formItem,
  setFormItem,
  currentColorScheme,
}) => {
  const [isSelectingForm, setIsSelectingForm] = useState(false);
  const onOpenFormSelector = () => setIsSelectingForm(true);

  if (formItem) {
    const splitted = formItem.title.split(".");
    formItem.title = splitted.slice(0, -1).join(".");
    formItem.exst = splitted.length !== 1 ? `.${splitted.at(-1)}` : null;
  }

  //TODO-mushka add final step to form submition
  const onSubmitToGallery = () => {};

  const onClose = () => {
    setVisible(false);
    setIsSelectingForm(false);
    formItem && setFormItem(null);
  };

  if (isSelectingForm)
    return (
      <SelectFileDialog
        key="select-form-dialog"
        displayType="aside"
        onSelectFile={onSubmitToGallery}
        isPanelVisible={true}
        onClose={onClose}
        filteredType="exceptPrivacyTrashArchiveFolders"
        ByExtension
        searchParam={".docxf"}
        dialogName={t("Common:SelectAction")}
        filesListTitle={t("Common:SelectDOCXFFormat")}
        creationButtonPrimary
        withSubfolders={false}
        primaryButtonName={t("FormGallery:SubmitToGallery")}
      />
    );

  return (
    <StyledModalDialog visible={visible} onClose={onClose} autoMaxHeight>
      <ModalDialog.Header>{t("Common:SubmitToFormGallery")}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>{t("FormGallery:SubmitToGalleryDialogMainInfo")}</div>
        <div>
          <Trans
            t={t}
            i18nKey="SubmitToGalleryDialogGuideInfo"
            ns="FormGallery"
          >
            Learn how to create perfect forms and increase your chance to get
            approval in our
            <Link
              color={currentColorScheme.main.accent}
              href="#"
              type={"page"}
              isBold
              isHovered
            >
              guide
            </Link>
            .
          </Trans>
        </div>

        {formItem && (
          <StyledFormItem>
            <ReactSVG className="icon" src={formItem.icon} />
            <div className="item-title">
              {formItem.title ? (
                [
                  <span className="name" key="name">
                    {formItem.title}
                  </span>,
                  formItem.exst && (
                    <span className="exst" key="exst">
                      {formItem.exst}
                    </span>
                  ),
                ]
              ) : (
                <span className="name">{formItem.exst}</span>
              )}
            </div>
          </StyledFormItem>
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        {!formItem ? (
          <Button
            primary
            size="normal"
            label={t("FormGallery:SelectForm")}
            onClick={onOpenFormSelector}
            scale
          />
        ) : (
          <Button
            primary
            size="normal"
            label={t("FormGallery:SubmitToGallery")}
            onClick={onSubmitToGallery}
            scale
          />
        )}
        <Button
          size="normal"
          label={t("Common:CancelButton")}
          onClick={onClose}
          scale
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, dialogsStore }) => ({
  visible: dialogsStore.submitToGalleryDialogVisible,
  setVisible: dialogsStore.setSubmitToGalleryDialogVisible,
  formItem: dialogsStore.formItem,
  setFormItem: dialogsStore.setFormItem,
  currentColorScheme: auth.settingsStore.currentColorScheme,
}))(withTranslation("Common", "FormGallery")(observer(SubmitToFormGallery)));
