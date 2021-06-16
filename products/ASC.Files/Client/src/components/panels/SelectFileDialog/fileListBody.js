import React from "react";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { useTranslation } from "react-i18next";
const FileListBody = ({ isLoadingData, filesList, onFileClick }) => {
  const { t } = useTranslation(["SelectFile", "Common"]);

  return (
    <>
      {!isLoadingData ? (
        filesList.length > 0 ? (
          filesList.map((data, index) => (
            <div className="file-name">
              <div
                id={`${index}`}
                key={`${index}`}
                className="entry-title"
                onClick={onFileClick}
              >
                {data.title.substring(0, data.title.indexOf(".gz"))}
              </div>
              <div className="file-exst">{".gz"}</div>
            </div>
          ))
        ) : (
          <Text>{`${t("Home:EmptyFolderHeader")}`} </Text>
        )
      ) : (
        <div key="loader" className="panel-loader-wrapper">
          <Loader type="oval" size="16px" className="panel-loader" />
          <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
            "Common:LoadingDescription"
          )}`}</Text>
        </div>
      )}
    </>
  );
};
export default FileListBody;
