import React from "react";
import Text from "@appserver/components/text";
import globalColors from "@appserver/components/utils/globalColors";

const sideColor = globalColors.gray;

const SizeCell = (props) => {
  const { t, item } = props;
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
