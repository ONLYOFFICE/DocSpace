import React from "react";
import Text from "@appserver/components/text";

const SizeCell = ({ t, item, sideColor }) => {
  const {
    fileExst,
    contentLength,
    providerKey,
    filesCount,
    foldersCount,
  } = item;
  return (
    <Text
      style={{ minWidth: 120, maxWidth: 200 }}
      as="div"
      color={sideColor}
      fontSize="12px"
      fontWeight={400}
      title=""
      truncate={true}
    >
      {fileExst || contentLength
        ? contentLength
        : !providerKey
        ? `${t("TitleDocuments")}: ${filesCount} | ${t(
            "TitleSubfolders"
          )}: ${foldersCount}`
        : ""}
    </Text>
  );
};

export default SizeCell;
