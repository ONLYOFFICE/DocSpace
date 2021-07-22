import React from "react";
import Text from "@appserver/components/text";

const DateCell = ({ create, updatedDate, createdDate, sideColor, item }) => {
  const { fileExst, contentLength, providerKey } = item;

  const date = create ? createdDate : updatedDate;

  return (
    <Text
      title={date}
      fontSize="12px"
      fontWeight={400}
      color={sideColor}
      className="row_update-text"
    >
      {(fileExst || contentLength || !providerKey) && date && date}
    </Text>
  );
};

export default DateCell;
