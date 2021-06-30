import React from "react";
import Loaders from "../../Loaders";
import StyledDialogAsideLoader from "./StyledDialogAsideLoader";

const DialogAsideLoader = ({ isPanel }) => {
  return (
    <StyledDialogAsideLoader isPanel={isPanel}>
      <div className="dialog-loader-header">
        <Loaders.Rectangle />
      </div>
      <div className="dialog-loader-body">
        <Loaders.Rectangle height="200px" />
      </div>

      <div className="dialog-loader-footer">
        <Loaders.Rectangle />
      </div>
    </StyledDialogAsideLoader>
  );
};

export default DialogAsideLoader;
