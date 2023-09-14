import { Link, ModalDialog } from "@docspace/components";
import { Button } from "@docspace/components";
import React, { useState } from "react";
import { observer, inject } from "mobx-react";
import { Trans, withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import FilesSelector from "@docspace/client/src/components/FilesSelector";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";

//
import { request } from "@docspace/common/api/client";
import { getFileInfo } from "@docspace/common/api/files";
import { combineUrl, toUrlParams } from "@docspace/common/utils";

import * as Styled from "./index.styled";

const SubmitToFormGallery = ({
  t,
  visible,
  setVisible,
  formItem,
  setFormItem,
  getIcon,
  currentColorScheme,
  canSubmitToFormGallery,
  submitToFormGallery,
}) => {
  const [isSelectingForm, setIsSelectingForm] = useState(false);
  const onOpenFormSelector = () => setIsSelectingForm(true);

  const [isSubmitting, setIsSubmitting] = useState(false);

  if (formItem) {
    const splitted = formItem.title.split(".");
    formItem.title = splitted.slice(0, -1).join(".");
    formItem.exst = splitted.length !== 1 ? `.${splitted.at(-1)}` : null;
  }

  console.log(formItem);

  const onSelectForm = (data) => setFormItem(data);

  //TODO-mushka add final step to form submition
  const onSubmitToGallery = async () => {
    if (!formItem) return;

    setIsSubmitting(true);

    let file = null;
    const src = `${combineUrl(
      window.DocSpaceConfig?.proxy?.url
    )}/filehandler.ashx?action=download&fileid=${formItem.id}`;

    await fetch(src)
      .then((res) => {
        if (!res.ok) throw new Error(res.statusText);
        return res.blob();
      })
      .then((res) => {
        const reader = new FileReader();
        reader.onload = (e) => (file = e.target?.result);
        reader.readAsArrayBuffer(res);
      })
      .catch((event) => console.error(event))
      .finally(() => setIsSubmitting(false));

    // fetch("https://oforms.teamlab.info/api/upload", {
    //   method: "POST",
    //   body: JSON.stringify({
    //     file: file,
    //     fileName: formItem.title,
    //     language: "en",
    //   }),
    //   headers: {
    //     "Content-type": "multipart/form-data",
    //   },
    // });

    await submitToFormGallery(file, formItem.title, "en");

    onClose();
  };

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
    <Styled.SubmitToGalleryDialog
      visible={visible}
      onClose={onClose}
      autoMaxHeight
    >
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
          <Styled.FormItem>
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
          </Styled.FormItem>
        )}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        {!formItem ? (
          <Button
            primary
            size="normal"
            label={t("FormGallery:SelectForm")}
            onClick={onOpenFormSelector}
            isLoading={isSubmitting}
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
    </Styled.SubmitToGalleryDialog>
  );
};

export default inject(
  ({ auth, accessRightsStore, dialogsStore, settingsStore, oformsStore }) => ({
    visible: dialogsStore.submitToGalleryDialogVisible,
    setVisible: dialogsStore.setSubmitToGalleryDialogVisible,
    formItem: dialogsStore.formItem,
    setFormItem: dialogsStore.setFormItem,
    getIcon: settingsStore.getIcon,
    currentColorScheme: auth.settingsStore.currentColorScheme,
    canSubmitToFormGallery: accessRightsStore.canSubmitToFormGallery,
    submitToFormGallery: oformsStore.submitToFormGallery,
  })
)(withTranslation("Common", "FormGallery")(observer(SubmitToFormGallery)));
