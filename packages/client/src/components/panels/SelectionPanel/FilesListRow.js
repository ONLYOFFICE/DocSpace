import React from "react";
import { StyledRow } from "./StyledSelectionPanel";
import Text from "@docspace/components/text";
import RadioButton from "@docspace/components/radio-button";
import ItemIcon from "../../ItemIcon";
const FilesListRow = ({
  displayType,
  index,
  onSelectFile,
  isChecked,
  folderSelection,
  icon,
  item,
}) => {
  const { id, fileExst, title } = item;
  const element = <ItemIcon id={id} icon={icon} fileExst={fileExst} />;

  const onFileClick = () => {
    onSelectFile && onSelectFile(item, index);
  };
  return (
    <StyledRow
      displayType={displayType}
      isChecked={isChecked}
      folderSelection={folderSelection}
      onClick={onFileClick}
    >
      <div className="selection-panel_icon">{element}</div>
      <div className="selection-panel_text">
        <Text fontSize="14px" fontWeight={600} noSelect>
          {title}
        </Text>
      </div>
      <div className="selection-panel_checkbox">
        {!folderSelection && (
          <RadioButton
            fontSize="13px"
            fontWeight="400"
            name={`${index}`}
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
