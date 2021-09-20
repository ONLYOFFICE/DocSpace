import React from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import Row from "@appserver/components/row";
import RowContent from "@appserver/components/row-content";
import RowContainer from "@appserver/components/row-container";
import { ReactSVG } from "react-svg";
import { withTranslation } from "react-i18next";
import { downloadFormatFiles } from "@appserver/common/api/files";
import { TIMEOUT } from "../../../helpers/constants";
import DownloadContent from "./DownloadContent";
import { inject, observer } from "mobx-react";

class DownloadDialogComponent extends React.Component {
  constructor(props) {
    super(props);
    const { sortedFiles, t } = this.props;

    this.state = {
      documents: sortedFiles.documents,
      spreadsheets: sortedFiles.spreadsheets,
      presentations: sortedFiles.presentations,
      other: sortedFiles.other,

      documentsTitleFormat: null,
      spreadsheetsTitleFormat: null,
      presentationsTitleFormat: null,

      checkedDocTitle: true,
      checkedSpreadsheetTitle: true,
      checkedPresentationTitle: true,
      checkedOtherTitle: true,

      indeterminateDocTitle: false,
      indeterminateSpreadsheetTitle: false,
      indeterminatePresentationTitle: false,
      indeterminateOtherTitle: false,
    };
  }

  onClose = () => this.props.setDownloadDialogVisible(false);

  getDownloadItems = () => {
    const { documents, spreadsheets, presentations, other } = this.state;
    const files = [];
    const folders = [];

    const collectItems = (itemList) => {
      for (let item of itemList) {
        if (item.checked) {
          if (item.fileExst) {
            const format =
              item.format === this.props.t("OriginalFormat")
                ? item.fileExst
                : item.format;
            const viewUrl = item.viewUrl;
            files.push({ key: item.id, value: format, viewUrl });
          } else {
            folders.push(item.id);
          }
        }
      }
    };

    collectItems(documents);
    collectItems(spreadsheets);
    collectItems(presentations);
    collectItems(other);

    return [files, folders];
  };

  //TODO: move to actions?
  onDownload = () => {
    const {
      //onDownloadProgress,
      t,
      getDownloadProgress,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.props;

    const [fileConvertIds, folderIds] = this.getDownloadItems();

    if (fileConvertIds.length === 1 && folderIds.length === 0) {
      // Single file download as
      const file = fileConvertIds[0];
      let viewUrl = file.viewUrl;
      if (file.value) {
        viewUrl = `${viewUrl}&outputtype=${file.value}`;
      }
      window.open(viewUrl, "_self");
      this.onClose();
    } else if (fileConvertIds.length || folderIds.length) {
      setSecondaryProgressBarData({
        icon: "file",
        visible: true,
        percent: 0,
        label: t("Translations:ArchivingData"),
        alert: false,
      });
      downloadFormatFiles(fileConvertIds, folderIds)
        .then((res) => {
          this.onClose();
          getDownloadProgress(
            res[0],
            t("Translations:ArchivingData")
          ).catch((err) => toastr.error(err));
        })
        .catch((err) => {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
          //toastr.error(err);
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        });
    }
  };

  getItemIcon = (item) => {
    const extension = item.fileExst;
    const icon = extension
      ? this.props.getIcon(24, extension)
      : this.props.getFolderIcon(item.providerKey, 24);

    return (
      <ReactSVG
        beforeInjection={(svg) => {
          svg.setAttribute("style", "margin-top: 4px");
        }}
        src={icon}
        loading={this.svgLoader}
      />
    );
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
    const { documents, spreadsheets, presentations } = this.state;
    const { t } = this.props;

    const newDocuments = documents.slice();
    const newSpreadsheets = spreadsheets.slice();
    const newPresentations = presentations.slice();

    if (type === "document") {
      const documents = this.getNewArrayFiles(fileId, newDocuments, format);
      this.setState({
        documents,
        documentsTitleFormat: !fileId ? format : t("CustomFormat"),
      });
    } else if (type === "spreadsheet") {
      const spreadsheets = this.getNewArrayFiles(
        fileId,
        newSpreadsheets,
        format
      );
      this.setState({
        spreadsheets,
        spreadsheetsTitleFormat: !fileId ? format : t("CustomFormat"),
      });
    } else if (type === "presentation") {
      const presentations = this.getNewArrayFiles(
        fileId,
        newPresentations,
        format
      );
      this.setState({
        presentations,
        presentationsTitleFormat: !fileId ? format : t("CustomFormat"),
      });
    }
  };

  onRowSelect = (item, type) => {
    const {
      documents,
      spreadsheets,
      presentations,
      other,
      checkedDocTitle,
      checkedSpreadsheetTitle,
      checkedPresentationTitle,
      checkedOtherTitle,
      indeterminateDocTitle,
      indeterminateSpreadsheetTitle,
      indeterminatePresentationTitle,
      indeterminateOtherTitle,
    } = this.state;

    const newDocuments = documents;
    const newSpreadsheets = spreadsheets;
    const newPresentations = presentations;
    const newOthers = other;

    if (type === "document") {
      //Select all documents
      if (item === "All") {
        const checked = indeterminateDocTitle ? false : !checkedDocTitle;
        for (let file of newDocuments) {
          file.checked = checked;
        }
        this.setState({
          documents: newDocuments,
          indeterminateDocTitle: false,
          checkedDocTitle: checked,
        });
      } else {
        //Select single document
        const newDoc = newDocuments.find((x) => x.id === item.id);
        newDoc.checked = !newDoc.checked;

        const disableFiles = newDocuments.find((x) => x.checked === false);
        const activeFiles = newDocuments.find((x) => x.checked === true);
        const indeterminate = !activeFiles ? false : !!disableFiles;
        const title = disableFiles ? false : true;
        this.setState({
          documents: newDocuments,
          indeterminateDocTitle: indeterminate,
          checkedDocTitle: title,
        });
      }
    } else if (type === "spreadsheet") {
      if (item === "All") {
        //Select all spreadsheets
        const checked = indeterminateSpreadsheetTitle
          ? false
          : !checkedSpreadsheetTitle;
        for (let spreadsheet of newSpreadsheets) {
          spreadsheet.checked = checked;
        }
        this.setState({
          spreadsheets: newSpreadsheets,
          indeterminateSpreadsheetTitle: false,
          checkedSpreadsheetTitle: checked,
        });
      } else {
        //Select single spreadsheet
        const newSpreadsheet = newSpreadsheets.find((x) => x.id === item.id);
        newSpreadsheet.checked = !newSpreadsheet.checked;

        const disableSpreadsheet = newSpreadsheets.find(
          (x) => x.checked === false
        );
        const activeSpreadsheet = newSpreadsheets.find(
          (x) => x.checked === true
        );
        const indeterminate = !activeSpreadsheet ? false : !!disableSpreadsheet;
        const title = disableSpreadsheet ? false : true;
        this.setState({
          spreadsheets: newSpreadsheets,
          indeterminateSpreadsheetTitle: indeterminate,
          checkedSpreadsheetTitle: title,
        });
      }
    } else if (type === "presentation") {
      if (item === "All") {
        //Select all presentations
        const checked = indeterminatePresentationTitle
          ? false
          : !checkedPresentationTitle;
        for (let presentation of newPresentations) {
          presentation.checked = checked;
        }
        this.setState({
          presentations: newPresentations,
          indeterminatePresentationTitle: false,
          checkedPresentationTitle: checked,
        });
      } else {
        //Select single presentation
        const newPresentation = newPresentations.find((x) => x.id === item.id);
        newPresentation.checked = !newPresentation.checked;

        const disablePresentation = newPresentations.find(
          (x) => x.checked === false
        );
        const activePresentation = newPresentations.find(
          (x) => x.checked === true
        );
        const indeterminate = !activePresentation
          ? false
          : !!disablePresentation;
        const title = disablePresentation ? false : true;
        this.setState({
          presentations: newPresentations,
          indeterminatePresentationTitle: indeterminate,
          checkedPresentationTitle: title,
        });
      }
    } else {
      if (item === "All") {
        const checked = indeterminateOtherTitle ? false : !checkedOtherTitle;
        for (let folder of newOthers) {
          folder.checked = checked;
        }
        this.setState({
          other: newOthers,
          indeterminateOtherTitle: false,
          checkedOtherTitle: checked,
        });
      } else {
        const newOther = newOthers.find((x) => x.id === item.id);
        newOther.checked = !newOther.checked;

        const disableFolders = newOthers.find((x) => x.checked === false);
        const activeFolders = newOthers.find((x) => x.checked === true);

        const indeterminate = !activeFolders ? false : !!disableFolders;
        const title = disableFolders ? false : true;
        this.setState({
          other: newOthers,
          indeterminateOtherTitle: indeterminate,
          checkedOtherTitle: title,
        });
      }
    }
  };

  render() {
    const { visible, t, tReady, filesConverts } = this.props;
    const {
      documentsTitleFormat,
      spreadsheetsTitleFormat,
      presentationsTitleFormat,
      documents,
      other,
      spreadsheets,
      presentations,
      checkedDocTitle,
      checkedSpreadsheetTitle,
      checkedPresentationTitle,
      checkedOtherTitle,
      indeterminateDocTitle,
      indeterminateSpreadsheetTitle,
      indeterminatePresentationTitle,
      indeterminateOtherTitle,
    } = this.state;

    const otherLength = other.length;
    const showOther = otherLength > 1;
    const minHeight = otherLength > 2 ? 110 : otherLength * 50;

    const isSingleFile =
      documents.filter((f) => f.checked).length +
        spreadsheets.filter((f) => f.checked).length +
        presentations.filter((f) => f.checked).length +
        other.filter((f) => f.checked).length <=
      1;

    return (
      <ModalDialogContainer
        isLoading={!tReady}
        visible={visible}
        onClose={this.onClose}
      >
        <ModalDialog.Header>{t("Translations:DownloadAs")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text noSelect>{t("ChooseFormatText")}</Text>
          {documents.length > 0 && (
            <DownloadContent
              t={t}
              filesConverts={filesConverts}
              checkedTitle={checkedDocTitle}
              indeterminateTitle={indeterminateDocTitle}
              items={documents}
              onSelectFormat={this.onSelectFormat}
              onRowSelect={this.onRowSelect}
              getItemIcon={this.getItemIcon}
              titleFormat={documentsTitleFormat || t("OriginalFormat")}
              type="document"
              title={t("Common:Documents")}
            />
          )}

          {spreadsheets.length > 0 && (
            <DownloadContent
              t={t}
              filesConverts={filesConverts}
              checkedTitle={checkedSpreadsheetTitle}
              indeterminateTitle={indeterminateSpreadsheetTitle}
              items={spreadsheets}
              onSelectFormat={this.onSelectFormat}
              onRowSelect={this.onRowSelect}
              getItemIcon={this.getItemIcon}
              titleFormat={spreadsheetsTitleFormat || t("OriginalFormat")}
              type="spreadsheet"
              title={t("Translations:Spreadsheets")}
            />
          )}

          {presentations.length > 0 && (
            <DownloadContent
              t={t}
              filesConverts={filesConverts}
              checkedTitle={checkedPresentationTitle}
              indeterminateTitle={indeterminatePresentationTitle}
              items={presentations}
              onSelectFormat={this.onSelectFormat}
              onRowSelect={this.onRowSelect}
              getItemIcon={this.getItemIcon}
              titleFormat={presentationsTitleFormat || t("OriginalFormat")}
              type="presentation"
              title={t("Translations:Presentations")}
            />
          )}

          {otherLength > 0 && (
            <>
              {showOther && (
                <Row
                  key="title2"
                  onSelect={this.onRowSelect.bind(this, "All", "other")}
                  checked={checkedOtherTitle}
                  indeterminate={indeterminateOtherTitle}
                >
                  <RowContent>
                    <Text
                      truncate
                      type="page"
                      title={"Other"}
                      fontSize="14px"
                      noSelect
                    >
                      {t("Other")}
                    </Text>
                    <></>
                  </RowContent>
                </Row>
              )}

              <RowContainer
                useReactWindow
                style={{ minHeight: minHeight, padding: "8px 0" }}
                itemHeight={50}
              >
                {other.map((folder) => {
                  const element = this.getItemIcon(folder);
                  return (
                    <Row
                      key={folder.id}
                      onSelect={this.onRowSelect.bind(this, folder, "other")}
                      checked={folder.checked}
                      element={element}
                    >
                      <RowContent>
                        <Text
                          truncate
                          type="page"
                          title={folder.title}
                          fontSize="14px"
                          noSelect
                        >
                          {folder.title}
                        </Text>
                        <></>
                        <Text fontSize="12px" containerWidth="auto" noSelect>
                          {folder.fileExst && t("OriginalFormat")}
                        </Text>
                      </RowContent>
                    </Row>
                  );
                })}
              </RowContainer>
            </>
          )}

          {!isSingleFile && <Text>{t("ConvertToZip")}</Text>}
          <Text noSelect>{t("ConvertMessage")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
          <Button
            className="button-dialog-accept"
            key="DownloadButton"
            label={t("Common:Download")}
            size="medium"
            primary
            onClick={this.onDownload}
          />
          <Button
            className="button-dialog"
            key="CancelButton"
            label={t("Common:CancelButton")}
            size="medium"
            onClick={this.onClose}
          />
        </ModalDialog.Footer>
      </ModalDialogContainer>
    );
  }
}

const DownloadDialog = withTranslation([
  "DownloadDialog",
  "Common",
  "Translations",
])(DownloadDialogComponent);

export default inject(
  ({
    filesStore,
    uploadDataStore,
    formatsStore,
    dialogsStore,
    filesActionsStore,
  }) => {
    const { secondaryProgressDataStore } = uploadDataStore;
    const { sortedFiles } = filesStore;
    const { getIcon, getFolderIcon } = formatsStore.iconFormatsStore;
    const { filesConverts } = formatsStore.docserviceStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const {
      downloadDialogVisible: visible,
      setDownloadDialogVisible,
    } = dialogsStore;

    const { getDownloadProgress } = filesActionsStore;

    return {
      sortedFiles,
      visible,
      filesConverts,

      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      getIcon,
      getFolderIcon,
      setDownloadDialogVisible,
      getDownloadProgress,
    };
  }
)(withRouter(observer(DownloadDialog)));
