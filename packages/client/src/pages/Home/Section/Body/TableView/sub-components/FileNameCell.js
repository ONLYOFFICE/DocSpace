import React from "react";
import Link from "@docspace/components/link";
import Checkbox from "@docspace/components/checkbox";
import TableCell from "@docspace/components/table-container/TableCell";
import Loader from "@docspace/components/loader";

const FileNameCell = ({
  item,
  titleWithoutExt,
  linkStyles,
  element,
  onContentSelect,
  checked,
  theme,
  t,
  inProgress,
}) => {
  const { title } = item;

  const onChange = (e) => {
    onContentSelect && onContentSelect(e.target.checked, item);
  };

  return (
    <>
      {inProgress ? (
        <Loader
          className="table-container_row-loader"
          type="oval"
          size="16px"
        />
      ) : (
        <TableCell
          className="table-container_element-wrapper"
          hasAccess={true}
          checked={checked}
        >
          <div className="table-container_element-container">
            <div className="table-container_element">{element}</div>
            <Checkbox
              className="table-container_row-checkbox"
              onChange={onChange}
              isChecked={checked}
              title={t("Common:TitleSelectFile")}
            />
          </div>
        </TableCell>
      )}

      <Link
        type="page"
        title={title}
        fontWeight="600"
        fontSize="13px"
        {...linkStyles}
        color={theme.filesSection.tableView.fileName.linkColor}
        isTextOverflow
        className="item-file-name"
      >
        {titleWithoutExt}
      </Link>
    </>
  );
};

export default FileNameCell;
