import { Link, ModalDialog } from "@docspace/components";
import { Button } from "@docspace/components";
import { useState, useRef } from "react";
import { observer, inject } from "mobx-react";
import { Trans, withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";
import FilesSelector from "@docspace/client/src/components/FilesSelector";
import { FilesSelectorFilterTypes } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";

import { combineUrl } from "@docspace/common/utils";

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
  const [isSubmitting, setIsSubmitting] = useState(false);

  const abortControllerRef = useRef(new AbortController());

  let formItemIsSet = !!formItem;

  const [isSelectingForm, setIsSelectingForm] = useState(false);
  const onOpenFormSelector = () => setIsSelectingForm(true);
  const onCloseFormSelector = () => {
    if (!formItemIsSet) onClose();
    else setIsSelectingForm(false);
  };

  const onSelectForm = (data) => {
    formItemIsSet = true;
    setFormItem(data);
  };

  const onSubmitToGallery = async () => {
    if (!formItem) return;

    setIsSubmitting(true);

    const origin = combineUrl(window.DocSpaceConfig?.proxy?.url);
    const fileSrc = `${origin}/filehandler.ashx?action=download&fileid=${formItem.id}`;

    const file = await fetch(fileSrc)
      .then((res) => {
        if (!res.ok) throw new Error(res.statusText);
        return res.arrayBuffer();
      })
      .then(async (arrayBuffer) => {
        return new File([arrayBuffer], formItem.title, {
          type: "application/octet-stream",
        });
      })
      .catch((err) => onError(err));

    await submitToFormGallery(
      file,
      formItem.title,
      "en",
      abortControllerRef.current?.signal
    )
      .then((res) => {
        if (!res.data) throw new Error(res.statusText);
        window.location.replace(res.data);
      })
      .catch((err) => onError(err))
      .finally(() => onClose());
  };

  const onClose = () => {
    abortControllerRef.current?.abort();
    setIsSubmitting(false);
    setFormItem(null);
    setIsSelectingForm(false);
    setVisible(false);
  };

  const onError = (err) => {
    if (!err.message === "canceled") {
      console.error(err);
      toastr.error(err);
    }
    onClose();
  };

  if (!canSubmitToFormGallery()) return null;

  if (isSelectingForm)
    return (
      <FilesSelector
        key="select-file-dialog"
        filterParam={FilesSelectorFilterTypes.DOCXF}
        descriptionText={t("Common:SelectDOCXFFormat")}
        isPanelVisible={true}
        onSelectFile={onSelectForm}
        onClose={onCloseFormSelector}
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
              {formItem?.title ? (
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
                <span className="name">{"" + formItem.exst}</span>
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
            scale
          />
        ) : (
          <Button
            primary
            size="normal"
            label={t("Common:SubmitToGallery")}
            onClick={onSubmitToGallery}
            isLoading={isSubmitting}
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
