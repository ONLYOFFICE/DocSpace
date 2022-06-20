import React, { useState } from "react";
import Text from "@appserver/components/text";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import Checkbox from "@appserver/components/checkbox";
import ArrowIcon from "../../../../public/images/arrow.react.svg";
import { StyledDownloadContent } from "./StyledDownloadDialog";
import DownloadRow from "./DownloadRow";

const DownloadContent = (props) => {
  const {
    t,
    items,
    onSelectFormat,
    onRowSelect,
    titleFormat,
    type,
    extsConvertible,
    title,
    isChecked,
    isIndeterminate,
    showHeader,
  } = props;

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
      case "documents":
        return formats;
      case "spreadsheets":
        return formats;
      case "presentations":
        return formats;
      default:
        return [];
    }
  };

  const isOther = type === "other";

  const titleData = !isOther && getTitleExtensions();

  const [isOpen, setIsOpen] = useState(false);

  const onOpen = () => {
    setIsOpen(!isOpen);
  };

  return (
    <StyledDownloadContent isOpen={showHeader ? isOpen : true}>
      {showHeader && (
        <div className="download-dialog_content-wrapper">
          <Checkbox
            data-item-id="All"
            data-type={type}
            isChecked={isChecked}
            isIndeterminate={isIndeterminate}
            onChange={onRowSelect}
            className="download-dialog-checkbox"
          />
          <div
            onClick={onOpen}
            className="download-dialog-heading download-dialog_row-text"
          >
            <Text noSelect fontSize="16px" fontWeight={600}>
              {title}
            </Text>
            <ArrowIcon className="download-dialog-icon" />
          </div>

          {(isChecked || isIndeterminate) && !isOther && (
            <LinkWithDropdown
              containerMinWidth="fit-content"
              data={titleData}
              directionX="left"
              directionY="bottom"
              dropdownType="alwaysDashed"
              fontSize="13px"
              fontWeight={600}
            >
              {titleFormat}
            </LinkWithDropdown>
          )}
        </div>
      )}
      <div className="download-dialog_hidden-items">
        {items.map((file) => {
          const dropdownItems =
            !isOther &&
            getFormats(file).filter((x) => x.label !== file.fileExst);

          return (
            <DownloadRow
              t={t}
              key={file.id}
              file={file}
              isChecked={file.checked}
              onRowSelect={onRowSelect}
              type={type}
              isOther={isOther}
              dropdownItems={dropdownItems}
            />
          );
        })}
      </div>
    </StyledDownloadContent>
  );
};

export default DownloadContent;
