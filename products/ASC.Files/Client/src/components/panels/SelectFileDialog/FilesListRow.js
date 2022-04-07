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
  noCheckBox,
  theme,
}) => {
  return (
    <StyledFilesList
      displayType={displayType}
      theme={theme}
      needRowSelection={needRowSelection}
      isChecked={isChecked}
      noCheckBox={noCheckBox}
    >
      <div
        data-index={index}
        className="modal-dialog_file-name"
        onClick={onSelectFile}
      >
        {isMultiSelect ? ( //  it will be needed
          <Checkbox
            theme={theme}
            label=""
            isChecked={isChecked}
            className="select-file-dialog_checked"
          />
        ) : (
          <RadioButton
            theme={theme}
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
          <Text theme={theme} data-index={index} className="entry-title">
            {fileName}
            <Text
              theme={theme}
              data-index={index}
              className="file-exst"
              as="span"
            >
              {fileExst}
            </Text>
          </Text>
        </div>
        <div className="files-list_file-children_wrapper">{children}</div>
      </div>
    </StyledFilesList>
  );
};

FilesListRow.defaultProps = {
  isMultiSelect: false,
};

export default inject(({ settingsStore }, { fileExst }) => {
  const iconSrc = settingsStore.getIconSrc(fileExst, 24);
  return {
    iconSrc,
  };
})(observer(FilesListRow));
