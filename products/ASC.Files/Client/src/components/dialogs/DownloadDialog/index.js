import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import ModalDialogContainer from "../ModalDialogContainer";
import {
  ModalDialog,
  Button,
  Text,
  Row,
  RowContent,
  RowContainer,
} from "asc-web-components";
import { ReactSVG } from "react-svg";
import { withTranslation } from "react-i18next";
import { utils, api, toastr } from "asc-web-common";
import {
  getFileIcon,
  getFolderIcon,
  isSpreadsheet,
  isPresentation,
  isDocument,
  getSelection,
} from "../../../store/files/selectors";
import {
  setProgressBarData,
  clearProgressData,
} from "../../../store/files/actions";
import DownloadContent from "./DownloadContent";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "DownloadDialog",
  localesPath: "dialogs/DownloadDialog",
});

const { changeLanguage } = utils;

const formatKeys = Object.freeze({
  OriginalFormat: 0,
  TxtFormat: 1,
  DocxFormat: 2,
  OdtFormat: 3,
  OdsFormat: 4,
  OdpFormat: 5,
  PdfFormat: 6,
  RtfFormat: 7,
  XlsxFormat: 8,
  PptxFormat: 9,
  CustomFormat: 10,
});

class DownloadDialogComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);

    const documents = [];
    const spreadsheets = [];
    const presentations = [];
    const other = [];

    for (let item of props.items) {
      item.checked = true;
      item.format = formatKeys.OriginalFormat;

      if (item.fileExst) {
        if (isSpreadsheet(item.fileExst)) {
          spreadsheets.push(item);
        } else if (isPresentation(item.fileExst)) {
          presentations.push(item);
        } else if (item.fileExst !== ".pdf" && isDocument(item.fileExst)) {
          documents.push(item);
        } else {
          other.push(item);
        }
      } else {
        other.push(item);
      }
    }

    this.state = {
      documents,
      spreadsheets,
      presentations,
      other,

      documentsTitleFormat: formatKeys.OriginalFormat,
      spreadsheetsTitleFormat: formatKeys.OriginalFormat,
      presentationsTitleFormat: formatKeys.OriginalFormat,

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

  getTitleLabel = (format) => {
    switch (format) {
      case 0:
        return this.props.t("OriginalFormat");
      case 1:
        return ".txt";
      case 2:
        return ".docx";
      case 3:
        return ".odt";
      case 4:
        return ".ods";
      case 5:
        return ".odp";
      case 6:
        return ".pdf";
      case 7:
        return ".rtf";
      case 8:
        return ".xlsx";
      case 9:
        return ".pptx";
      case 10:
        return this.props.t("CustomFormat");
      default:
        return "";
    }
  };

  getDownloadItems = () => {
    const { documents, spreadsheets, presentations, other } = this.state;
    const items = [];
    const folders = [];

    for (let item of documents) {
      if (item.checked) {
        const format =
          item.format === 0 ? item.fileExst : this.getTitleLabel(item.format);
        items.push({ key: item.id, value: format });
      }
    }

    for (let item of spreadsheets) {
      if (item.checked) {
        const format =
          item.format === 0 ? item.fileExst : this.getTitleLabel(item.format);
        items.push({ key: item.id, value: format });
      }
    }

    for (let item of presentations) {
      if (item.checked) {
        const format =
          item.format === 0 ? item.fileExst : this.getTitleLabel(item.format);
        items.push({ key: item.id, value: format });
      }
    }

    for (let item of other) {
      if (item.checked) {
        if (item.fileExst) {
          const format =
            item.format === 0 ? item.fileExst : this.getTitleLabel(item.format);
          items.push({ key: item.id, value: format });
        } else {
          folders.push(item.id);
        }
      }
    }

    return [items, folders];
  };

  onDownload = () => {
    const {
      onDownloadProgress,
      onClose,
      t,
      setProgressBarData,
      clearProgressData,
    } = this.props;

    const downloadItems = this.getDownloadItems();
    const fileConvertIds = downloadItems[0];
    const folderIds = downloadItems[1];

    if (fileConvertIds.length || folderIds.length) {
      setProgressBarData({
        visible: true,
        percent: 0,
        label: t("ArchivingData"),
      });
      api.files
        .downloadFormatFiles(fileConvertIds, folderIds)
        .then(() => {
          onClose();
          onDownloadProgress(false);
        })
        .catch((err) => {
          toastr.error(err);
          clearProgressData();
        });
    }
  };

  getItemIcon = (item) => {
    const extension = item.fileExst;
    const icon = extension
      ? getFileIcon(extension, 24)
      : getFolderIcon(item.providerKey, 24);

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

  onSelectFormat = (format, file, type) => {
    const { documents, spreadsheets, presentations } = this.state;

    const newDocuments = documents;
    const newSpreadsheets = spreadsheets;
    const newPresentations = presentations;

    if (type === "document") {
      //Set all documents format
      if (!file) {
        for (let file of newDocuments) {
          file.format =
            format === formatKeys.CustomFormat || file.fileExst === format
              ? formatKeys.OriginalFormat
              : format;
        }
        this.setState({
          documents: newDocuments,
          documentsTitleFormat: format,
        });
      } else {
        //Set single document format
        const newDoc = newDocuments.find((x) => x.id === file.id);
        if (newDoc.format !== format) {
          newDoc.format = format;
          this.setState({
            documents: newDocuments,
            documentsTitleFormat: formatKeys.CustomFormat,
          });
        }
      }
    } else if (type === "spreadsheet") {
      //Set all spreadsheets format
      if (!file) {
        for (let file of newSpreadsheets) {
          file.format =
            format === formatKeys.CustomFormat || file.fileExst === format
              ? formatKeys.OriginalFormat
              : format;
        }
        this.setState({
          spreadsheets: newSpreadsheets,
          spreadsheetsTitleFormat: format,
        });
      } else {
        //Set single spreadsheet format
        const newSpreadsheet = newSpreadsheets.find((x) => x.id === file.id);
        if (newSpreadsheet.format !== format) {
          newSpreadsheet.format = format;
          this.setState({
            spreadsheets: newSpreadsheets,
            spreadsheetsTitleFormat: formatKeys.CustomFormat,
          });
        }
      }
    } else if (type === "presentation") {
      //Set all presentations format
      if (!file) {
        for (let file of newPresentations) {
          file.format =
            format === formatKeys.CustomFormat || file.fileExst === format
              ? formatKeys.OriginalFormat
              : format;
        }
        this.setState({
          presentations: newPresentations,
          presentationsTitleFormat: format,
        });
      } else {
        //Set single presentation format
        const newPresentation = newPresentations.find((x) => x.id === file.id);
        if (newPresentation.format !== format) {
          newPresentation.format = format;
          this.setState({
            presentations: newPresentations,
            presentationsTitleFormat: formatKeys.CustomFormat,
          });
        }
      }
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
    const { onClose, visible, t } = this.props;
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

    return (
      <ModalDialogContainer>
        <ModalDialog visible={visible} onClose={onClose}>
          <ModalDialog.Header>{t("DownloadAs")}</ModalDialog.Header>
          <ModalDialog.Body>
            <Text>{t("ChooseFormatText")}</Text>
            {documents.length > 0 && (
              <DownloadContent
                t={t}
                checkedTitle={checkedDocTitle}
                indeterminateTitle={indeterminateDocTitle}
                items={documents}
                formatKeys={formatKeys}
                onSelectFormat={this.onSelectFormat}
                onRowSelect={this.onRowSelect}
                getItemIcon={this.getItemIcon}
                getTitleLabel={this.getTitleLabel}
                titleFormat={documentsTitleFormat}
                type="document"
              />
            )}

            {spreadsheets.length > 0 && (
              <DownloadContent
                t={t}
                checkedTitle={checkedSpreadsheetTitle}
                indeterminateTitle={indeterminateSpreadsheetTitle}
                items={spreadsheets}
                formatKeys={formatKeys}
                onSelectFormat={this.onSelectFormat}
                onRowSelect={this.onRowSelect}
                getItemIcon={this.getItemIcon}
                getTitleLabel={this.getTitleLabel}
                titleFormat={spreadsheetsTitleFormat}
                type="spreadsheet"
              />
            )}

            {presentations.length > 0 && (
              <DownloadContent
                t={t}
                checkedTitle={checkedPresentationTitle}
                indeterminateTitle={indeterminatePresentationTitle}
                items={presentations}
                formatKeys={formatKeys}
                onSelectFormat={this.onSelectFormat}
                onRowSelect={this.onRowSelect}
                getItemIcon={this.getItemIcon}
                getTitleLabel={this.getTitleLabel}
                titleFormat={presentationsTitleFormat}
                type="presentation"
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
                          >
                            {folder.title}
                          </Text>
                          <></>
                          <Text fontSize="12px" containerWidth="auto">
                            {folder.fileExst && t("OriginalFormat")}
                          </Text>
                        </RowContent>
                      </Row>
                    );
                  })}
                </RowContainer>
              </>
            )}

            <Text>{t("ConvertToZip")}</Text>
            <Text>{t("ConvertMessage")}</Text>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              className="button-dialog-accept"
              key="DownloadButton"
              label={t("DownloadButton")}
              size="medium"
              primary
              onClick={this.onDownload}
              //isLoading={isLoading}
            />
            <Button
              className="button-dialog"
              key="CancelButton"
              label={t("CancelButton")}
              size="medium"
              onClick={onClose}
              //isLoading={isLoading}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </ModalDialogContainer>
    );
  }
}

const ModalDialogContainerTranslated = withTranslation()(
  DownloadDialogComponent
);

const DownloadDialog = (props) => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {
    items: getSelection(state),
  };
};

export default connect(mapStateToProps, {
  setProgressBarData,
  clearProgressData,
})(withRouter(DownloadDialog));
