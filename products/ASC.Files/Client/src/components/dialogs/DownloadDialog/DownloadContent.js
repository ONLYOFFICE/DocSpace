import React from "react";
import {
  Row,
  RowContent,
  RowContainer,
  Text,
  LinkWithDropdown,
} from "asc-web-components";

const DownloadContent = (props) => {
  const {
    t,
    checkedTitle,
    indeterminateTitle,
    items,
    formatKeys,
    onSelectFormat,
    onRowSelect,
    getItemIcon,
    titleFormat,
    getTitleLabel,
    type,
  } = props;

  const getFormats = (item) => {
    const documentFormats = [
      {
        key: "key1",
        label: t("OriginalFormat"),
        onClick: onSelectFormat.bind(
          this,
          formatKeys.OriginalFormat,
          item,
          "document"
        ),
      },
      {
        key: "key2",
        label: ".txt",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.TxtFormat,
          item,
          "document"
        ),
      },
      {
        key: "key3",
        label: ".docx",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.DocxFormat,
          item,
          "document"
        ),
      },
      {
        key: "key4",
        label: ".odt",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.OdtFormat,
          item,
          "document"
        ),
      },
      {
        key: "key5",
        label: ".pdf",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.PdfFormat,
          item,
          "document"
        ),
      },
      {
        key: "key6",
        label: ".rtf",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.RtfFormat,
          item,
          "document"
        ),
      },
      {
        key: "key7",
        label: t("CustomFormat"),
        onClick: onSelectFormat.bind(
          this,
          formatKeys.CustomFormat,
          item,
          "document"
        ),
      },
    ];

    const presentationFormats = [
      {
        key: "key1",
        label: t("OriginalFormat"),
        onClick: onSelectFormat.bind(
          this,
          formatKeys.OriginalFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key2",
        label: ".odp",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.OdpFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key3",
        label: ".pdf",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.PdfFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key4",
        label: ".pptx",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.PptxFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key5",
        label: t("CustomFormat"),
        onClick: onSelectFormat.bind(
          this,
          formatKeys.CustomFormat,
          item,
          "presentation"
        ),
      },
    ];

    const spreadsheetFormats = [
      {
        key: "key1",
        label: t("OriginalFormat"),
        onClick: onSelectFormat.bind(
          this,
          formatKeys.OriginalFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key2",
        label: ".odp",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.OdsFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key3",
        label: ".pdf",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.PdfFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key4",
        label: ".xlsx",
        onClick: onSelectFormat.bind(
          this,
          formatKeys.XlsxFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key5",
        label: t("CustomFormat"),
        onClick: onSelectFormat.bind(
          this,
          formatKeys.CustomFormat,
          item,
          "spreadsheet"
        ),
      },
    ];

    switch (type) {
      case "document":
        return documentFormats;
      case "spreadsheet":
        return spreadsheetFormats;
      case "presentation":
        return presentationFormats;
      default:
        return [];
    }
  };

  const getTitle = () => {
    switch (type) {
      case "document":
        return t("Documents");
      case "spreadsheet":
        return t("Spreadsheets");
      case "presentation":
        return t("Presentations");
      default:
        return "";
    }
  };

  const title = getTitle();

  const length = items.length;
  const minHeight = length > 2 ? 110 : length * 50;
  const showTitle = length > 1;
  const formats = getFormats();
  const documentsTitle = getTitleLabel(titleFormat);

  return (
    <>
      {showTitle && (
        <Row
          key={"title"}
          onSelect={onRowSelect.bind(this, "All", type)}
          checked={checkedTitle}
          indeterminate={indeterminateTitle}
        >
          <RowContent>
            <Text truncate type="page" title={title} fontSize="14px">
              {title}
            </Text>
            <></>
            <Text fontSize="12px" containerWidth="auto">
              {t("ConvertInto")}
            </Text>
            <LinkWithDropdown
              containerWidth="auto"
              data={formats}
              directionX="right"
              directionY="bottom"
              dropdownType="appearDashedAfterHover"
              fontSize="12px"
            >
              {documentsTitle}
            </LinkWithDropdown>
          </RowContent>
        </Row>
      )}

      <RowContainer
        useReactWindow={length > 2}
        style={{ minHeight: minHeight, padding: "8px 0" }}
        itemHeight={50}
      >
        {items.map((file) => {
          const element = getItemIcon(file);
          let dropdownItems = getFormats(file);
          const format = getTitleLabel(file.format);
          dropdownItems = dropdownItems.slice(1, -1);
          dropdownItems = dropdownItems.filter(
            (x) => x.label !== file.fileExst
          );
          return (
            <Row
              key={file.id}
              onSelect={onRowSelect.bind(this, file, type)}
              checked={file.checked}
              element={element}
            >
              <RowContent>
                <Text truncate type="page" title={file.title} fontSize="14px">
                  {file.title}
                </Text>
                <></>
                <Text fontSize="12px" containerWidth="auto">
                  {file.checked && t("ConvertInto")}
                </Text>
                <LinkWithDropdown
                  dropdownType="appearDashedAfterHover"
                  containerWidth="auto"
                  data={dropdownItems}
                  directionX="right"
                  directionY="bottom"
                  fontSize="12px"
                >
                  {format}
                </LinkWithDropdown>
              </RowContent>
            </Row>
          );
        })}
      </RowContainer>
    </>
  );
};

export default DownloadContent;
