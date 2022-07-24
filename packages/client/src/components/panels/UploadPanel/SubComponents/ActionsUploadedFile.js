import React from "react";
import ShareButton from "./ShareButton";
import IconButton from "@docspace/components/icon-button";
import LoadingButton from "./LoadingButton";
const ActionsUploadedFile = ({ item, isPersonal, onCancelCurrentUpload }) => {
  const onCancelClick = !item.inConversion
    ? { onClick: onCancelCurrentUpload }
    : {};

  return (
    <>
      {item.action === "upload" && !isPersonal && (
        <ShareButton uniqueId={item.uniqueId} />
      )}
      {item.action === "convert" && (
        <div
          className="upload_panel-icon"
          data-id={item.uniqueId}
          data-file-id={item.fileId}
          data-action={item.action}
          {...onCancelClick}
        >
          <LoadingButton
            isConversion
            inConversion={item.inConversion}
            percent={item.convertProgress}
          />
          <IconButton
            iconName="/static/images/refresh.react.svg"
            className="convert_icon"
            size="medium"
            isfill={true}
            color="#A3A9AE"
          />
        </div>
      )}
    </>
  );
};

export default ActionsUploadedFile;
