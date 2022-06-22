import React, { useEffect, useState } from "react";
import { isMobile } from "react-device-detect";
import Row from "@appserver/components/row";
import RowContent from "@appserver/components/row-content";
import RowContainer from "@appserver/components/row-container";
import Text from "@appserver/components/text";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import styled, { css } from "styled-components";
import { tablet } from "@appserver/components/utils/device";

const MobileStyles = css`
  .row-content_tablet-side-info {
    display: flex;
    gap: 5px;
  }
  .download-dialog-link {
    text-decoration: underline dashed;
  }
`;

const StyledDownloadContent = styled.div`
  .row_content,
  .row-content_tablet-side-info {
    overflow: unset;
  }

  .download-dialog_row-container {
    display: flex;
  }

  ${isMobile && MobileStyles}

  @media ${tablet} {
    ${MobileStyles}
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
    extsConvertible,
    title,
  } = props;

  const [isScrolling, setIsScrolling] = useState(null);
  const [isOpen, setIsOpen] = useState(null);

  const getTitleExtensions = () => {
    let arr = [];
    for (let item of items) {
      const exst = item.fileExst;

      arr = [...arr, ...extsConvertible[exst]];
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

  useEffect(() => {
    if (isScrolling) {
      setIsOpen(false);
      const id = setTimeout(() => setIsScrolling(false), 500);
      return () => {
        clearTimeout(id);
        setIsOpen(null);
      };
    }
  }, [isScrolling]);

  const onScroll = () => {
    setIsScrolling(true);
  };

  const getFormats = (item) => {
    const arrayFormats = item ? extsConvertible[item.fileExst] : [];
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
    for (let f of arrayFormats) {
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
            <Text fontSize="12px" containerMinWidth="fit-content">
              {(checkedTitle || indeterminateTitle) && t("ConvertInto")}
            </Text>
            {checkedTitle || indeterminateTitle ? (
              <LinkWithDropdown
                className="download-dialog-link"
                containerMinWidth="fit-content"
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
        onScroll={onScroll}
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
                  <Text
                    fontSize="12px"
                    containerMinWidth="fit-content"
                    noSelect
                  >
                    {t("ConvertInto")}
                  </Text>
                )}

                {file.checked ? (
                  <LinkWithDropdown
                    className="download-dialog-link"
                    isOpen={isOpen}
                    dropdownType={
                      isMobile ? "alwaysDashed" : "appearDashedAfterHover"
                    }
                    containerMinWidth="fit-content"
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
