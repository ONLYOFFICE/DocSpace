import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
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
}) => {
  const { fileExst, title } = item;

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
        {fileExst ? (
          <Text
            className="badge-ext"
            as="span"
            color="#A3A9AE"
            fontSize="13px"
            fontWeight={600}
            truncate={true}
          >
            {fileExst}
          </Text>
        ) : null}
      </Link>
    </>
  );
};

export default FileNameCell;
