import React, { useEffect, useState } from "react";
import Checkbox from "@appserver/components/checkbox";
import TableCell from "@appserver/components/table-container/TableCell";

const CheckboxCell = (props) => {
  const { onContentFileSelect, element, checkedProps, item } = props;
  const { checked } = checkedProps;

  const [iconVisible, setIconVisible] = useState(!checked);

  const onMouseEnter = () => {
    if (checked) return;
    setIconVisible(false);
  };

  const onMouseLeave = () => {
    if (checked) return;
    setIconVisible(true);
  };

  useEffect(() => {
    setIconVisible(!checked);
  }, [checked]);

  const onChange = (e) => {
    onContentFileSelect && onContentFileSelect(e.target.checked, item);
  };

  return (
    <TableCell onMouseLeave={onMouseLeave} onMouseEnter={onMouseEnter}>
      {iconVisible ? (
        element
      ) : (
        <Checkbox onChange={onChange} isChecked={checked} />
      )}
    </TableCell>
  );
};

export default CheckboxCell;
