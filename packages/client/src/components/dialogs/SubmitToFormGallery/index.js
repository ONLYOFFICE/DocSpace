import { Link, ModalDialog } from "@docspace/components";
import { Button } from "@docspace/components";
import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { Trans, withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import styled from "styled-components";
// import SelectFileDialog from "@docspace/client/src/components/panels/SelectFileDialog";
import FilesSelector from "@docspace/client/src/components/FilesSelector";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";
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
  padding: 8px 16px;
  align-items: center;
  justify-content: start;
  gap: 8px;
  border-radius: 6px;
  background: ${(props) => props.theme.infoPanel.history.fileBlockBg};

  .icon {
    margin: 4px;
    width: 24px;
    height: 24px;
    svg {
      width: 24px;
      height: 24px;
    }
  }

  .item-title {
    margin: 8px 0;
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
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
  selectFileDialogVisible,
  setSelectFileDialogVisible,
  formItem,
  setFormItem,
  getIcon,
  currentColorScheme,
  canSubmitToFormGallery,
}) => {
  const [isSelectingForm, setIsSelectingForm] = useState(false);
  const onOpenFormSelector = () => setIsSelectingForm(true);

  if (formItem) {
    const splitted = formItem.title.split(".");
    formItem.title = splitted.slice(0, -1).join(".");
    formItem.exst = splitted.length !== 1 ? `.${splitted.at(-1)}` : null;
  }

  console.log(formItem);

  const onSelectForm = (data) => setFormItem(data);

  //TODO-mushka add final step to form submition
  const onSubmitToGallery = () => onClose();

  const onCloseSelectFormDialog = () => setIsSelectingForm(false);

  const onClose = () => {
    setVisible(false);
    setIsSelectingForm(false);
    formItem && setFormItem(null);
  };

  if (!canSubmitToFormGallery()) return null;

  if (isSelectingForm)
    return (
      <FilesSelector
        key="select-file-dialog"
        filterParam={FilesSelectorFilterTypes.OFORM}
        isPanelVisible={true}
        onSelectFile={onSelectForm}
        onClose={onCloseSelectFormDialog}
      />
    );

  return (
    <StyledModalDialog visible={visible} onClose={onClose} autoMaxHeight>
      <ModalDialog.Header>{t("Common:SubmitToFormGallery")}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>{t("FormGallery:SubmitToGalleryDialogMainInfo")}</div>
        <div>
          {/* TODO-mushka add correct link to guide */}
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
            <ReactSVG className="icon" src={getIcon(24, formItem.exst)} />
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

export default inject(
  ({ auth, accessRightsStore, dialogsStore, settingsStore }) => ({
    visible: dialogsStore.submitToGalleryDialogVisible,
    setVisible: dialogsStore.setSubmitToGalleryDialogVisible,
    selectFileDialogVisible: dialogsStore.selectFileDialogVisible,
    setSelectFileDialogVisible: dialogsStore.setSelectFileDialogVisible,
    formItem: dialogsStore.formItem,
    setFormItem: dialogsStore.setFormItem,
    getIcon: settingsStore.getIcon,
    currentColorScheme: auth.settingsStore.currentColorScheme,
    canSubmitToFormGallery: accessRightsStore.canSubmitToFormGallery,
  })
)(withTranslation("Common", "FormGallery")(observer(SubmitToFormGallery)));
