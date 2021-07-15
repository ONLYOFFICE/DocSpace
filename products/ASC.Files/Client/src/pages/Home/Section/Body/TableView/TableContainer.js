import React, { useRef } from "react";
import TableContainer from "@appserver/components/table-container";
import { inject, observer } from "mobx-react";
import TableRow from "./TableRow";
import TableHeader from "@appserver/components/table-container/TableHeader";
import TableBody from "@appserver/components/table-container/TableBody";
import Checkbox from "@appserver/components/checkbox";

const Table = ({ filesList, isHeaderVisible, setSelected }) => {
  const onChange = (checked) => {
    setSelected(checked ? "all" : "none");
  };

  const columns = [
    {
      key: -1,
      //title: "Checkbox",
      //title: "",
      title: (
        <Checkbox
          onChange={onChange}
          // checked={isHeaderChecked}
          // isIndeterminate={isHeaderIndeterminate}
        />
      ),
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
      resizable: false,
    },
    {
      key: 5,
      title: "",
      includes: [""],
      resizable: false,
    },
  ];

  const ref = useRef(null);

  return (
    <TableContainer forwardedRef={ref}>
      <TableHeader
        isHeaderVisible={isHeaderVisible}
        containerRef={ref}
        columns={columns}
      />
      <TableBody>
        {filesList.map((item, index) => (
          <TableRow key={item.id} item={item} index={index} />
        ))}
      </TableBody>
    </TableContainer>
  );
};

export default inject(({ filesStore }) => {
  const {
    filesList,
    setSelected,
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
  } = filesStore;

  return {
    filesList,
    setSelected,
    isHeaderVisible,
    isHeaderIndeterminate,
    isHeaderChecked,
  };
})(observer(Table));
