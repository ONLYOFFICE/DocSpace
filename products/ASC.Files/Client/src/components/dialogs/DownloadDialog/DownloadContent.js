import React from "react";
import Row from "@appserver/components/row";
import RowContent from "@appserver/components/row-content";
import RowContainer from "@appserver/components/row-container";
import Text from "@appserver/components/text";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import styled from "styled-components";
import { FilesFormats } from "@appserver/common/constants";

const StyledDownloadContent = styled.div`
  .row_content,
  .row-content_tablet-side-info {
    overflow: unset;
  }
`;

const DownloadContent = (props) => {
  const {
    t,
    checkedTitle,
    indeterminateTitle,
    items,
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
          FilesFormats.OriginalFormat,
          item,
          "document"
        ),
      },
      {
        key: "key2",
        label: ".txt",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.TxtFormat,
          item,
          "document"
        ),
      },
      {
        key: "key3",
        label: ".docx",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.DocxFormat,
          item,
          "document"
        ),
      },
      {
        key: "key4",
        label: ".odt",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.OdtFormat,
          item,
          "document"
        ),
      },
      {
        key: "key5",
        label: ".pdf",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.PdfFormat,
          item,
          "document"
        ),
      },
      {
        key: "key6",
        label: ".rtf",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.RtfFormat,
          item,
          "document"
        ),
      },
      {
        key: "key7",
        label: t("CustomFormat"),
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.CustomFormat,
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
          FilesFormats.OriginalFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key2",
        label: ".odp",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.OdpFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key3",
        label: ".pdf",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.PdfFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key4",
        label: ".pptx",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.PptxFormat,
          item,
          "presentation"
        ),
      },
      {
        key: "key5",
        label: t("CustomFormat"),
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.CustomFormat,
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
          FilesFormats.OriginalFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key2",
        label: ".odp",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.OdsFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key3",
        label: ".pdf",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.PdfFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key4",
        label: ".xlsx",
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.XlsxFormat,
          item,
          "spreadsheet"
        ),
      },
      {
        key: "key5",
        label: t("CustomFormat"),
        onClick: onSelectFormat.bind(
          this,
          FilesFormats.CustomFormat,
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
        return t("Common:Documents");
      case "spreadsheet":
        return t("Translations:Spreadsheets");
      case "presentation":
        return t("Translations:Presentations");
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
    <StyledDownloadContent>
      {showTitle && (
        <Row
          key={"title"}
          onSelect={onRowSelect.bind(this, "All", type)}
          checked={checkedTitle}
          indeterminate={indeterminateTitle}
        >
          <RowContent convertSideInfo={false}>
            <Text truncate type="page" title={title} fontSize="14px">
              {title}
            </Text>
            <></>
            <Text fontSize="12px" containerWidth="auto">
              {(checkedTitle || indeterminateTitle) && t("ConvertInto")}
            </Text>
            {checkedTitle || indeterminateTitle ? (
              <LinkWithDropdown
                containerWidth="auto"
                data={formats}
                directionX="left"
                directionY="bottom"
                dropdownType="appearDashedAfterHover"
                fontSize="12px"
              >
                {documentsTitle}
              </LinkWithDropdown>
            ) : (
              <></>
            )}
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
              <RowContent convertSideInfo={false}>
                <Text
                  truncate
                  type="page"
                  title={file.title}
                  fontSize="14px"
                  noSelect
                >
                  {file.title}
                </Text>
                <></>
                {file.checked && (
                  <Text fontSize="12px" containerWidth="auto" noSelect>
                    {t("ConvertInto")}
                  </Text>
                )}

                {file.checked ? (
                  <LinkWithDropdown
                    dropdownType="appearDashedAfterHover"
                    containerWidth="auto"
                    data={dropdownItems}
                    directionX="left"
                    directionY="bottom"
                    fontSize="12px"
                  >
                    {format}
                  </LinkWithDropdown>
                ) : (
                  <></>
                )}
              </RowContent>
            </Row>
          );
        })}
      </RowContainer>
    </StyledDownloadContent>
  );
};

export default DownloadContent;
