import React from "react";
import Text from "@appserver/components/text";

const AuthorCell = ({ fileOwner, sideColor }) => {
  return (
    <Text
      as="div"
      color={sideColor}
      fontSize="12px"
      fontWeight={400}
      title={fileOwner}
      truncate={true}
    >
      {fileOwner}
    </Text>
  );
};

export default AuthorCell;
