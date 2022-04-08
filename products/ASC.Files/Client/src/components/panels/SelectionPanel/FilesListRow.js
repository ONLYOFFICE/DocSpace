import React from "react";
import { StyledRow } from "./StyledSelectionPanel";
import { ReactSVG } from "react-svg";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import RadioButton from "@appserver/components/radio-button";
import ItemIcon from "../../ItemIcon";
const FilesListRow = ({
  displayType,
  index,
  onFileClick,
  isChecked,
  theme,
  folderSelection,
  icon,
  item,
}) => {
  console.log("item", item);
  const { id, fileExst, title } = item;
  const element = <ItemIcon id={id} icon={icon} fileExst={fileExst} />;
  return (
    <StyledRow displayType={displayType} theme={theme} isChecked={isChecked}>
      {folderSelection && (
        <div
          //onClick={onFolderRowClick}
          className="selection-panel_clicked-area"
        ></div>
      )}
      <div className="selection-panel_icon">{element}</div>
      <div className="selection-panel_text">
        <Text fontSize="14px" fontWeight={600}>
          {title}
        </Text>
      </div>
      <div className="selection-panel_checkbox">
        {folderSelection && (
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
