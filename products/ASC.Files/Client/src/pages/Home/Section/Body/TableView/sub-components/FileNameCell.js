import React from "react";
import Link from "@appserver/components/link";
import Checkbox from "@appserver/components/checkbox";
import TableCell from "@appserver/components/table-container/TableCell";
import Loader from "@appserver/components/loader";

const FileNameCell = ({
  item,
  titleWithoutExt,
  linkStyles,
  element,
  onContentSelect,
  checked,
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
          <div className="table-container_element">{element}</div>
          <Checkbox
            className="table-container_row-checkbox"
            onChange={onChange}
            isChecked={checked}
            title={t("Common:TitleSelectFile")}
          />
        </TableCell>
      )}

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
