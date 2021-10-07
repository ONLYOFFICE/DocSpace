import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

const FileNameCell = ({ item, titleWithoutExt, linkStyles }) => {
  const { fileExst, title } = item;
  return (
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
  );
};

export default FileNameCell;
