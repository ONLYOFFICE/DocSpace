import React, { useRef } from "react";
import TableContainer from "@appserver/components/table-container";
import { inject, observer } from "mobx-react";
import Column from "@appserver/components/table-container/Column";
import TableCell from "./TableCell";

const Table = ({ filesList }) => {
  const columns = [
    {
      key: -1,
      title: "Checkbox",
      includes: ["checked"],
      resizable: false,
    },
    {
      key: 0,
      title: "Name",
      includes: ["title"],
      resizable: true,
    },
    {
      key: 1,
      title: "Author",
      includes: ["createdBy", "displayName"],
      resizable: true,
    },
    {
      key: 2,
      title: "Created",
      includes: ["created"],
      resizable: true,
    },
    // {
    //   key: 3,
    //   title: "Type",
    //   includes: [""],
    //   resizable: true,
    // },
    {
      key: 4,
      title: "Size",
      includes: ["contentLength"],
      resizable: true,
    },
    {
      key: 5,
      title: "Settings$#",
      includes: [""],
      resizable: false,
    },
  ];

  const ref = useRef(null);

  return (
    <TableContainer forwardedRef={ref}>
      {columns.map((column, index) => {
        const { key, title, resizable } = column;

        return (
          <Column
            id={`column_${index}`}
            index={index}
            key={key}
            title={title}
            resizable={resizable}
            containerRef={ref}
          >
            {filesList.map((item) => (
              <TableCell column={column} key={item.id} item={item} />
            ))}
          </Column>
        );
      })}
    </TableContainer>
  );
};

export default inject(({ filesStore }) => {
  const { filesList } = filesStore;
  return { filesList };
})(observer(Table));
