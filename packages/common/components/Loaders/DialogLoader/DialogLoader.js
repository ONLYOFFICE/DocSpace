import React from "react";
import Loaders from "../../Loaders";
import StyledDialogLoader from "./StyledDialogLoader";

const DialogLoader = ({ isLarge, withFooterBorder }) => {
  return (
    <StyledDialogLoader withFooterBorder={withFooterBorder} isLarge={isLarge}>
      <div className="dialog-loader-header">
        <Loaders.Rectangle height="29px" />
      </div>
      <div className="dialog-loader-body">
        <Loaders.Rectangle height={isLarge ? "175px" : "73px"} />
      </div>
      <div className="dialog-loader-footer">
        <Loaders.Rectangle height="40px" />
        <Loaders.Rectangle height="40px" />
      </div>
    </StyledDialogLoader>
  );
};

export default DialogLoader;
