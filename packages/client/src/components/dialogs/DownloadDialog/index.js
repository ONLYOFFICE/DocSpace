import React from "react";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { StyledDownloadDialog } from "./StyledDownloadDialog";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";

import DownloadContent from "./DownloadContent";

class DownloadDialogComponent extends React.Component {
  constructor(props) {
    super(props);
    const { documents, spreadsheets, presentations, masterForms, other } =
      this.props.sortedFiles;

    this.state = {
      documents: {
        isChecked: true,
        isIndeterminate: false,
        format: null,
        files: documents,
      },
      spreadsheets: {
        isChecked: true,
        isIndeterminate: false,
        format: null,
        files: spreadsheets,
      },
      presentations: {
        isChecked: true,
        isIndeterminate: false,
        format: null,
        files: presentations,
      },
      masterForms: {
        isChecked: true,
        isIndeterminate: false,
        format: null,
        files: masterForms,
      },
      other: {
        isChecked: true,
        isIndeterminate: false,
        files: other,
      },
      modalDialogToggle: "modalDialogToggle",
    };
  }

  onClose = () => this.props.setDownloadDialogVisible(false);

  getDownloadItems = () => {
    const itemList = [
      ...this.state.documents.files,
      ...this.state.spreadsheets.files,
      ...this.state.presentations.files,
      ...this.state.masterForms.files,
      ...this.state.other.files,
    ];

    const files = [];
    const folders = [];
    let singleFileUrl = null;

    for (let item of itemList) {
      if (item.checked) {
        if (!!item.fileExst || item.contentLength) {
          const format =
            !item.format || item.format === this.props.t("OriginalFormat")
              ? item.fileExst
              : item.format;
          if (!singleFileUrl) {
            singleFileUrl = item.viewUrl;
          }
          files.push({ key: item.id, value: format });
        } else {
          folders.push(item.id);
        }
      }
    }

    return [files, folders, singleFileUrl];
  };

  onDownload = () => {
    const { t, downloadFiles } = this.props;
    const [fileConvertIds, folderIds, singleFileUrl] = this.getDownloadItems();
    if (fileConvertIds.length === 1 && folderIds.length === 0) {
      // Single file download as
      const file = fileConvertIds[0];
      if (file.value && singleFileUrl) {
        const viewUrl = `${singleFileUrl}&outputtype=${file.value}`;
        window.open(viewUrl, "_self");
      }
      this.onClose();
    } else if (fileConvertIds.length || folderIds.length) {
      this.onClose();
      downloadFiles(fileConvertIds, folderIds, {
        label: t("Translations:ArchivingData"),
        error: t("Common:ErrorInternalServer"),
      });
    }
  };

  getNewArrayFiles = (fileId, array, format) => {
    //Set all documents format
    if (!fileId) {
      for (let file of array) {
        file.format =
          format === this.props.t("CustomFormat") || file.fileExst === format
            ? this.props.t("OriginalFormat")
            : format;
      }

      return array;
    } else {
      //Set single document format
      const newDoc = array.find((x) => x.id == fileId);
      if (newDoc.format !== format) {
        newDoc.format = format;
      }
      return array;
    }
  };

  onSelectFormat = (e) => {
    const { format, type, fileId } = e.target.dataset;
    const files = this.state[type].files;

    this.setState((prevState) => {
      const newState = { ...prevState };
      newState[type].files = this.getNewArrayFiles(fileId, files, format);
      newState[type].format = !fileId ? format : this.props.t("CustomFormat");

      const index = newState[type].files.findIndex(
        (f) => f.format && f.format !== this.props.t("OriginalFormat")
      );

      if (index === -1) {
        newState[type].format = this.props.t("OriginalFormat");
      }

      return { ...prevState, ...newState };
    });
  };

  updateDocsState = (fieldStateName, itemId) => {
    const { isChecked, isIndeterminate, files } = this.state[fieldStateName];

    if (itemId === "All") {
      const checked = isIndeterminate ? false : !isChecked;
      for (let file of files) {
        file.checked = checked;
      }

      this.setState((prevState) => {
        const newState = { ...prevState };

        newState[fieldStateName].files = files;
        newState[fieldStateName].isIndeterminate = false;
        newState[fieldStateName].isChecked = checked;

        return { ...prevState, ...newState };
      });
    } else {
      const file = files.find((x) => x.id == itemId);
      file.checked = !file.checked;

      const disableFiles = files.find((x) => x.checked === false);
      const activeFiles = files.find((x) => x.checked === true);
      const isIndeterminate = !activeFiles ? false : !!disableFiles;
      const isChecked = disableFiles ? false : true;

      this.setState((prevState) => {
        const newState = { ...prevState };

        newState[fieldStateName].files = files;
        newState[fieldStateName].isIndeterminate = isIndeterminate;
        newState[fieldStateName].isChecked = isChecked;

        return { ...prevState, ...newState };
      });
    }
  };

  onRowSelect = (e) => {
    const { itemId, type } = e.currentTarget.dataset;

    switch (type) {
      case "documents":
        this.updateDocsState("documents", itemId);

        break;
      case "spreadsheets":
        this.updateDocsState("spreadsheets", itemId);
        break;
      case "presentations":
        this.updateDocsState("presentations", itemId);
        break;
      case "masterForms":
        this.updateDocsState("masterForms", itemId);
        break;
      case "other":
        this.updateDocsState("other", itemId);
        break;

      default:
        break;
    }
  };

  render() {
    const { t, tReady, visible, extsConvertible, theme } = this.props;

    const {
      files: documents,
      isChecked: checkedDocTitle,
      isIndeterminate: indeterminateDocTitle,
      format: documentsTitleFormat,
    } = this.state.documents;

    const {
      files: spreadsheets,
      isChecked: checkedSpreadsheetTitle,
      isIndeterminate: isIndeterminateSpreadsheetTitle,
      format: spreadsheetsTitleFormat,
    } = this.state.spreadsheets;

    const {
      files: presentations,
      isChecked: checkedPresentationTitle,
      isIndeterminate: indeterminatePresentationTitle,
      format: presentationsTitleFormat,
    } = this.state.presentations;

    const {
      files: masterForms,
      isChecked: checkedMasterFormsTitle,
      isIndeterminate: indeterminateMasterFormsTitle,
      format: masterFormsTitleFormat,
    } = this.state.masterForms;

    const {
      files: other,
      isChecked: checkedOtherTitle,
      isIndeterminate: indeterminateOtherTitle,
    } = this.state.other;

    const isCheckedLength =
      documents.filter((f) => f.checked).length +
      spreadsheets.filter((f) => f.checked).length +
      presentations.filter((f) => f.checked).length +
      masterForms.filter((f) => f.checked).length +
      other.filter((f) => f.checked).length;

    const isSingleFile = isCheckedLength <= 1;

    const downloadContentProps = {
      t,
      theme,
      extsConvertible,
      onSelectFormat: this.onSelectFormat,
      onRowSelect: this.onRowSelect,
      getItemIcon: this.getItemIcon,
    };

    const totalItemsNum =
      this.state.documents.files.length +
      this.state.spreadsheets.files.length +
      this.state.presentations.files.length +
      this.state.masterForms.files.length +
      this.state.other.files.length +
      (this.state.documents.files.length > 1 && 1) +
      (this.state.spreadsheets.files.length > 1 && 1) +
      (this.state.presentations.files.length > 1 && 1) +
      (this.state.masterForms.files.length > 1 && 1) +
      (this.state.other.files.length > 1 && 1);

    return (
      <StyledDownloadDialog
        visible={visible}
        displayType="aside"
        onClose={this.onClose}
        autoMaxHeight
        autoMaxWidth
        isLarge
        isLoading={!tReady}
        withBodyScroll={totalItemsNum > 11}
      >
        <ModalDialog.Header>{t("Translations:DownloadAs")}</ModalDialog.Header>

        <ModalDialog.Body className={this.state.modalDialogToggle}>
          <div className="download-dialog-description">
            <Text noSelect>{t("ChooseFormatText")}.</Text>
            {!isSingleFile && (
              <Text noSelect>
                <Trans t={t} i18nKey="ConvertToZip" />
              </Text>
            )}
          </div>
          {documents.length > 0 && (
            <DownloadContent
              {...downloadContentProps}
              isChecked={checkedDocTitle}
              isIndeterminate={indeterminateDocTitle}
              items={documents}
              titleFormat={documentsTitleFormat || t("OriginalFormat")}
              type="documents"
              title={t("Common:Documents")}
            />
          )}

          {spreadsheets.length > 0 && (
            <DownloadContent
              {...downloadContentProps}
              isChecked={checkedSpreadsheetTitle}
              isIndeterminate={isIndeterminateSpreadsheetTitle}
              items={spreadsheets}
              titleFormat={spreadsheetsTitleFormat || t("OriginalFormat")}
              type="spreadsheets"
              title={t("Translations:Spreadsheets")}
            />
          )}

          {presentations.length > 0 && (
            <DownloadContent
              {...downloadContentProps}
              isChecked={checkedPresentationTitle}
              isIndeterminate={indeterminatePresentationTitle}
              items={presentations}
              titleFormat={presentationsTitleFormat || t("OriginalFormat")}
              type="presentations"
              title={t("Translations:Presentations")}
            />
          )}

          {masterForms.length > 0 && (
            <DownloadContent
              {...downloadContentProps}
              isChecked={checkedMasterFormsTitle}
              isIndeterminate={indeterminateMasterFormsTitle}
              items={masterForms}
              titleFormat={masterFormsTitleFormat || t("OriginalFormat")}
              type="masterForms"
              title={t("Translations:FormTemplates")}
            />
          )}

          {other.length > 0 && (
            <DownloadContent
              {...downloadContentProps}
              isChecked={checkedOtherTitle}
              isIndeterminate={indeterminateOtherTitle}
              items={other}
              type="other"
              title={t("Translations:Other")}
            />
          )}

          <div className="download-dialog-convert-message">
            <Text noSelect>{t("ConvertMessage")}</Text>
          </div>
        </ModalDialog.Body>

        <ModalDialog.Footer>
          <Button
            key="DownloadButton"
            className="download-button"
            label={t("Common:Download")}
            size="normal"
            primary
            onClick={this.onDownload}
            isDisabled={isCheckedLength === 0}
            scale
          />
          <Button
            key="CancelButton"
            className="cancel-button"
            label={t("Common:CancelButton")}
            size="normal"
            onClick={this.onClose}
            scale
          />
        </ModalDialog.Footer>
      </StyledDownloadDialog>
    );
  }
}

const DownloadDialog = withTranslation([
  "DownloadDialog",
  "Common",
  "Translations",
])(DownloadDialogComponent);

export default inject(
  ({ auth, filesStore, dialogsStore, filesActionsStore, settingsStore }) => {
    const { sortedFiles } = filesStore;
    const { extsConvertible } = settingsStore;
    const { theme } = auth.settingsStore;

    const { downloadDialogVisible: visible, setDownloadDialogVisible } =
      dialogsStore;

    const { downloadFiles } = filesActionsStore;

    return {
      sortedFiles,
      visible,
      extsConvertible,

      setDownloadDialogVisible,
      downloadFiles,

      theme,
    };
  }
)(observer(DownloadDialog));
