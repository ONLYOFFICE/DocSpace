import React, { useState } from "react";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";
import LinkWithDropdown from "@docspace/components/link-with-dropdown";
import Checkbox from "@docspace/components/checkbox";
import ArrowIcon from "@docspace/client/public/images/arrow.react.svg";
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
    theme,
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

  const showHeader = items.length > 1;

  return (
    <StyledDownloadContent isOpen={showHeader ? isOpen : true} theme={theme}>
      {showHeader && (
        <div className="download-dialog_content-wrapper download-dialog-row">
          <div className="download-dialog-main-content">
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
              className="download-dialog-heading download-dialog-title"
            >
              <Text noSelect fontSize="16px" fontWeight={600}>
                {title}
              </Text>
              <ArrowIcon className="download-dialog-icon" />
            </div>
          </div>
          <div className="download-dialog-actions">
            {(isChecked || isIndeterminate) && !isOther && (
              <LinkWithDropdown
                className="download-dialog-link"
                containerMinWidth="fit-content"
                data={titleData}
                directionX="left"
                directionY="bottom"
                dropdownType="alwaysDashed"
                fontSize="13px"
                fontWeight={600}
                withExpander
              >
                {titleFormat}
              </LinkWithDropdown>
            )}
          </div>
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

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { theme } = settingsStore;

  return {
    theme,
  };
})(observer(DownloadContent));
