import React from "react";
import IconButton from "@docspace/components/icon-button";

import ArrowPathReactSvgUrl from "../../../../../public/images/arrow.path.react.svg?url";

const ArrowButton = ({ isRootFolder, onBackToParentFolder }) => {
  return (
    <>
      {!isRootFolder ? (
        <div className="navigation-arrow-container">
          <IconButton
            iconName={ArrowPathReactSvgUrl}
            size="17"
            isFill={true}
            onClick={onBackToParentFolder}
            className="arrow-button"
          />
          <div className="navigation-header-separator" />
        </div>
      ) : (
        <></>
      )}
    </>
  );
};

export default React.memo(ArrowButton);
