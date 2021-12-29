import React from "react";
import Loaders from "../../Loaders";
import StyledDialogLoader from "./StyledDialogLoader";

const DialogLoader = ({ bodyHeight = "150px" }) => {
  return (
    <StyledDialogLoader>
      <div className="dialog-loader-header">
        <Loaders.Rectangle height="28px" width="470px" />
        <Loaders.Rectangle
          className="dialog-loader-icon"
          height="28px"
          width="28px"
        />
      </div>
      <div className="dialog-loader-body">
        <Loaders.Rectangle height={bodyHeight} />
      </div>

      <div className="dialog-loader-footer">
        <Loaders.Rectangle height="30px" width="120px" />
        <Loaders.Rectangle
          className="dialog-loader-icon"
          height="30px"
          width="100px"
        />
      </div>
    </StyledDialogLoader>
  );
};

export default DialogLoader;
