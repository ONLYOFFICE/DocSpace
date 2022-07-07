import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";

const DownloadRow = (props) => {
  const {
    t,
    file,
    onRowSelect,
    type,
    dropdownItems,
    getIcon,
    getFolderIcon,
    isOther,
    isChecked,
  } = props;

  //console.log("DownloadRow render");

  const [dropDownIsOpen, setDropDownIsOpen] = useState(false);

  const getItemIcon = (item) => {
    const extension = item.fileExst;
    const icon = extension
      ? getIcon(32, extension)
      : getFolderIcon(item.providerKey, 32);

    return (
      <ReactSVG
        beforeInjection={(svg) => {
          svg.setAttribute("style", "margin-top: 4px; margin-right: 12px;");
        }}
        src={icon}
        loading={() => <div style={{ width: "96px" }} />}
      />
    );
  };

  const element = getItemIcon(file);

  return (
    <div className="download-dialog_row">
      <Checkbox
        className="download-dialog-checkbox"
        data-item-id={file.id}
        data-type={type}
        onChange={onRowSelect}
        isChecked={isChecked}
      />
      {element}
      <Text
        className="download-dialog_row-text"
        truncate
        type="page"
        title={file.title}
        fontSize="14px"
        fontWeight={600}
        noSelect
      >
        {file.title}
      </Text>
      {file.checked && !isOther && (
        <LinkWithDropdown
          className="download-dialog-link"
          isOpen={dropDownIsOpen}
          dropdownType="alwaysDashed"
          containerMinWidth="fit-content"
          data={dropdownItems}
          directionX="left"
          directionY="bottom"
          fontSize="13px"
          fontWeight={600}
          hasScroll={true}
        >
          {file.format || t("OriginalFormat")}
        </LinkWithDropdown>
      )}
      {isOther && file.fileExst && (
        <Text
          className="download-dialog-other-text"
          truncate
          type="page"
          title={file.title}
          fontSize="13px"
          fontWeight={600}
          noSelect
        >
          {t("OriginalFormat")}
        </Text>
      )}
    </div>
  );
};

export default inject(({ settingsStore }) => {
  const { getIcon, getFolderIcon } = settingsStore;

  return {
    getIcon,
    getFolderIcon,
  };
})(observer(DownloadRow));
