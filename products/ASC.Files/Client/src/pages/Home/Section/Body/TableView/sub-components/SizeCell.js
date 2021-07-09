import React from "react";
import Text from "@appserver/components/text";

const SizeCell = (props) => {
  const { t, item, sideColor } = props;
  const {
    fileExst,
    contentLength,
    providerKey,
    filesCount,
    foldersCount,
  } = item;
  return (
    <Text
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
