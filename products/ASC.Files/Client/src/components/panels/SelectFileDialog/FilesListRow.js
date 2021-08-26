import React from "react";
import { StyledFilesList } from "../StyledPanels";
import { ReactSVG } from "react-svg";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import RadioButton from "@appserver/components/radio-button";
const FilesListRow = ({
  displayType,
  needRowSelection,
  index,
  onSelectFile,
  fileName,
  children,
  fileExst,
  iconSrc,
  isMultiSelect, // it will be needed
  isChecked,
}) => {
  return (
    <StyledFilesList
      displayType={displayType}
      needRowSelection={needRowSelection}
      isChecked={isChecked}
    >
      <div
        data-index={index}
        className="modal-dialog_file-name"
        onClick={onSelectFile}
      >
        {isMultiSelect ? ( //  it will be needed
          <Checkbox
            label=""
            isChecked={isChecked}
            className="select-file-dialog_checked"
          />
        ) : (
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            name={`${index}`}
            label=""
            isChecked={isChecked}
            onClick={onSelectFile}
            value=""
            className="select-file-dialog_checked"
          />
        )}
        <ReactSVG src={iconSrc} className="select-file-dialog_icon" />
        <div data-index={index} className="files-list_full-name">
          <Text data-index={index} className="entry-title">
            {fileName}
          </Text>

          <div data-index={index} className="file-exst">
            {fileExst}
          </div>
        </div>
        <div className="files-list_file-children_wrapper">{children}</div>
      </div>
    </StyledFilesList>
  );
};

FilesListRow.defaultProps = {
  needRowSelection: true,
  isMultiSelect: false,
};

export default inject(({ formatsStore }, { fileExst }) => {
  const { iconFormatsStore } = formatsStore;
  const iconSrc = iconFormatsStore.getIconSrc(fileExst, 24);
  return {
    iconSrc,
  };
})(observer(FilesListRow));
