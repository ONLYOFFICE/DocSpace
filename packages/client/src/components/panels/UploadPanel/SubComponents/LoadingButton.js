import React, { useState, useEffect } from "react";
import {
  StyledCircle,
  StyledCircleWrap,
  StyledLoadingButton,
} from "./StyledLoadingButton";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const LoadingButton = (props) => {
  const {
    id,
    className,
    style,
    percent,
    onClick,
    isConversion,
    inConversion,
    ...rest
  } = props;
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
      elementType={ThemeType.LoadingButton}
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

        <StyledLoadingButton isConversion={isConversion}>
          {!inConversion && <>&times;</>}
        </StyledLoadingButton>
      </StyledCircle>
    </ColorTheme>
  );
};

export default LoadingButton;
