import React from "react";
import Text from "@appserver/components/text";

const AuthorCell = (props) => {
  const { updatedDate, sideColor, item } = props;
  const { fileExst, contentLength, providerKey } = item;

  return (
    <Text
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
