import React from "react";
import Loaders from "../../Loaders";
import StyledDialogAsideLoader from "./StyledDialogAsideLoader";
import Aside from "@appserver/components/aside";
import Backdrop from "@appserver/components/backdrop";

const DialogAsideLoader = ({ isPanel, withoutAside }) => {
  const zIndex = 310;

  const renderClearDialogAsideLoader = () => {
    return (
      <StyledDialogAsideLoader visible isPanel={isPanel}>
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

  return withoutAside ? (
    renderClearDialogAsideLoader()
  ) : (
    <>
      <Backdrop visible isAside />
      <StyledDialogAsideLoader visible isPanel={isPanel}>
        <Aside className="dialog-aside-loader" visible zIndex={zIndex}>
          {renderClearDialogAsideLoader()}
        </Aside>
      </StyledDialogAsideLoader>
    </>
  );
};

export default DialogAsideLoader;
