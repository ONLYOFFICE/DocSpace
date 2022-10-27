import React from "react";
import { StyledRow } from "./StyledSelectionPanel";
import Text from "@docspace/components/text";
import RadioButton from "@docspace/components/radio-button";
import { ItemIcon } from "@docspace/components";
const FilesListRow = ({
  displayType,
  index,
  onSelectFile,
  isChecked,
  theme,
  folderSelection,
  icon,
  item,
}) => {
  const { title } = item;
  const element = <ItemIcon item={item} />;

  const onFileClick = () => {
    onSelectFile && onSelectFile(item, index);
  };
  return (
    <StyledRow
      displayType={displayType}
      theme={theme}
      isChecked={isChecked}
      folderSelection={folderSelection}
      onClick={onFileClick}
    >
      <div className="selection-panel_icon">{element}</div>
      <div className="selection-panel_text">
        <Text fontSize="14px" fontWeight={600}>
          {title}
        </Text>
      </div>
      <div className="selection-panel_checkbox">
        {!folderSelection && (
          <RadioButton
            //theme={theme}
            fontSize="13px"
            fontWeight="400"
            name={`${index}`}
            label=""
            isChecked={isChecked}
            onClick={onFileClick}
            value=""
            className="select-file-dialog_checked"
          />
        )}
      </div>
    </StyledRow>
  );
};

export default FilesListRow;
