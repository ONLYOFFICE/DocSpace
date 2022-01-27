import React from "react";
import Link from "@appserver/components/link";
import Checkbox from "@appserver/components/checkbox";
import TableCell from "@appserver/components/table-container/TableCell";

const FileNameCell = ({
  item,
  titleWithoutExt,
  linkStyles,
  element,
  onContentSelect,
  checked,
  selectionProp,
  t,
}) => {
  const { title } = item;

  const onChange = (e) => {
    onContentSelect && onContentSelect(e.target.checked, item);
  };

  return (
    <>
      <TableCell
        hasAccess={true}
        checked={checked}
        {...selectionProp}
        className={`${selectionProp?.className} table-container_row-checkbox-wrapper`}
      >
        <div className="table-container_element">{element}</div>
        <Checkbox
          className="table-container_row-checkbox"
          onChange={onChange}
          isChecked={checked}
          title={t("Common:TitleSelectFile")}
        />
      </TableCell>

      <Link
        type="page"
        title={title}
        fontWeight="600"
        fontSize="13px"
        {...linkStyles}
        color="#333"
        isTextOverflow
        className="item-file-name"
      >
        {titleWithoutExt}
      </Link>
    </>
  );
};

export default FileNameCell;
