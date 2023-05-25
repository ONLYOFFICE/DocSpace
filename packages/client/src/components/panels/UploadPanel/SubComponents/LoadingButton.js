import React, { useState, useEffect } from "react";
import { inject, observer } from "mobx-react";
import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";
import {
  StyledLoadingButton,
  StyledCircle,
} from "@docspace/components/ColorTheme/styled/sub-components/StyledLoadingButton";

const LoadingButton = (props) => {
  const { id, className, style, percent, onClick, isConversion, inConversion } =
    props;
  const [isAnimation, setIsAnimation] = useState(true);

  const stopAnimation = () => {
    setIsAnimation(false);
  };

  useEffect(() => {
    const timer = setTimeout(stopAnimation, 5000);

    return function cleanup() {
      clearTimeout(timer);
    };
  }, [isAnimation]);

  return (
    <ColorTheme
      {...props}
      id={id}
      className={className}
      style={style}
      onClick={onClick}
      themeId={ThemeType.LoadingButton}
    >
      <StyledCircle
        percent={percent}
        inConversion={inConversion}
        isAnimation={isAnimation}
      >
        <div className="circle__mask circle__full">
          <div className="circle__fill"></div>
        </div>
        <div className="circle__mask">
          <div className="circle__fill"></div>
        </div>

        <StyledLoadingButton
          className="loading-button"
          isConversion={isConversion}
        >
          {!inConversion && <>&times;</>}
        </StyledLoadingButton>
      </StyledCircle>
    </ColorTheme>
  );
};

export default inject(({ uploadDataStore }, { item }) => {
  const { primaryProgressDataStore, isParallel } = uploadDataStore;
  const { loadingFile: file } = primaryProgressDataStore;

  const loadingFile = !file || !file.uniqueId ? null : file;

  const currentFileUploadProgress =
    file && loadingFile?.uniqueId === item?.uniqueId
      ? loadingFile.percent
      : null;

  return {
    percent: isParallel
      ? item?.percent
        ? item.percent
        : null
      : currentFileUploadProgress,
  };
})(observer(LoadingButton));
