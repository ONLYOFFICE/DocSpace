import React from "react";
import Loaders from "../../Loaders";
import StyledDialogAsideLoader from "./StyledDialogAsideLoader";
import Aside from "@docspace/components/aside";
import Backdrop from "@docspace/components/backdrop";

const DialogAsideLoader = ({
  isPanel,
  withoutAside,
  withFooterBorder = false,
}) => {
  const zIndex = 310;

  const renderClearDialogAsideLoader = () => {
    return (
      <StyledDialogAsideLoader withFooterBorder={withFooterBorder} visible>
        <div className="dialog-loader-header">
          <Loaders.Rectangle height="29px" />
        </div>
        <div className="dialog-loader-body">
          <Loaders.Rectangle height="200px" />
        </div>

        <div className="dialog-loader-footer">
          <Loaders.Rectangle height="40px" />
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
