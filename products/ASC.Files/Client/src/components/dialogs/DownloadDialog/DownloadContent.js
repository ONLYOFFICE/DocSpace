import React from "react";
import { isMobile } from "react-device-detect";
import Row from "@appserver/components/row";
import RowContent from "@appserver/components/row-content";
import RowContainer from "@appserver/components/row-container";
import Text from "@appserver/components/text";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import styled from "styled-components";

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
    type,
    filesConverts,
    title,
  } = props;

  const getTitleExtensions = () => {
    let arr = [];
    for (let item of items) {
      const exst = item.fileExst;

      const exstItem = filesConverts.find((f) => f[exst]);
      const arrayExst = exstItem ? exstItem[exst] : [];
      arr = [...arr, ...arrayExst];
    }

    arr = arr.filter((x, pos) => arr.indexOf(x) !== pos);
    arr = arr.filter((x, pos) => arr.indexOf(x) === pos);

    const formats = [
      {
        key: "original",
        label: t("OriginalFormat"),
        onClick: onSelectFormat,
        "data-format": t("OriginalFormat"),
        "data-type": type,
      },
    ];

    for (let f of arr) {
      formats.push({
        key: f,
        label: f,
        onClick: onSelectFormat,
        "data-format": f,
        "data-type": type,
      });
    }

    formats.push({
      key: "custom",
      label: t("CustomFormat"),
      onClick: onSelectFormat,
      "data-format": t("CustomFormat"),
      "data-type": type,
    });

    return formats;
  };

  const getFormats = (item) => {
    const arrayFormats = item && filesConverts.find((f) => f[item.fileExst]);
    const conversionFormats = arrayFormats ? arrayFormats[item.fileExst] : [];

    const formats = [
      {
        key: "original",
        label: t("OriginalFormat"),
        onClick: onSelectFormat,
        "data-format": t("OriginalFormat"),
        "data-type": type,
        "data-file-id": item.id,
      },
    ];
    for (let f of conversionFormats) {
      formats.push({
        key: f,
        label: f,
        onClick: onSelectFormat,
        "data-format": f,
        "data-type": type,
        "data-file-id": item.id,
      });
    }

    switch (type) {
      case "document":
        return formats;
      case "spreadsheet":
        return formats;
      case "presentation":
        return formats;
      default:
        return [];
    }
  };

  const length = items.length;
  const minHeight = length > 2 ? 110 : length * 50;
  const showTitle = length > 1;

  const titleData = getTitleExtensions();

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
                data={titleData}
                directionX="left"
                directionY="bottom"
                dropdownType="appearDashedAfterHover"
                fontSize="12px"
              >
                {titleFormat}
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
                    dropdownType={
                      isMobile ? "alwaysDashed" : "appearDashedAfterHover"
                    }
                    containerWidth="auto"
                    data={dropdownItems}
                    directionX="left"
                    directionY="bottom"
                    fontSize="12px"
                  >
                    {file.format || t("OriginalFormat")}
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
