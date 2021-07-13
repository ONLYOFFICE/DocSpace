import React from "react";
import Text from "@appserver/components/text";

const AuthorCell = ({ updatedDate, sideColor, item }) => {
  const { fileExst, contentLength, providerKey } = item;

  return (
    <Text
      style={{ minWidth: 120, maxWidth: 200 }}
      title={updatedDate}
      fontSize="12px"
      fontWeight={400}
      color={sideColor}
      className="row_update-text"
    >
      {(fileExst || contentLength || !providerKey) &&
        updatedDate &&
        updatedDate}
    </Text>
  );
};

export default AuthorCell;
