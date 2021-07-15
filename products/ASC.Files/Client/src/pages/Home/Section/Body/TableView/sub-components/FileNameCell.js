import React from "react";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

const FileNameCell = ({ item, titleWithoutExt, linkStyles }) => {
  const { fileExst } = item;
  return (
    <Link
      type="page"
      title={titleWithoutExt}
      fontWeight="600"
      fontSize="15px"
      {...linkStyles}
      color="#333"
      isTextOverflow
    >
      {titleWithoutExt}
      {fileExst ? (
        <Text
          className="badge-ext"
          as="span"
          color="#A3A9AE"
          fontSize="15px"
          fontWeight={600}
          title={fileExst}
          truncate={true}
        >
          {fileExst}
        </Text>
      ) : null}
    </Link>
  );
};

export default FileNameCell;
