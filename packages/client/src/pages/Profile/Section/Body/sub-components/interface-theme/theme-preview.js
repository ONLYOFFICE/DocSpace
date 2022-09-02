import React from "react";
import { ThemeProvider } from "styled-components";
import RadioButton from "@docspace/components/radio-button";
import Text from "@docspace/components/text";
//import ThemeProvider from "@docspace/components/theme-provider";

import { StyledWrapper, StyledPreview } from "./styled-preview";

const ThemePreview = (props) => {
  const {
    t,
    label,
    isDisabled,
    theme,
    isChecked,
    onChangeTheme,
    value,
  } = props;
  return (
    <StyledWrapper>
      <div className="header">
        <RadioButton
          name={`theme-option-${value}`}
          label={label}
          onClick={onChangeTheme}
          value={value}
          isDisabled={isDisabled}
          isChecked={isChecked}
        />
      </div>
      <ThemeProvider theme={theme}>
        <StyledPreview>
          <div className="article">
            <img
              className="logo"
              src="/static/images/logo.docspace.react.svg"
            />
            <button className="main-button">
              <Text fontSize="16px" fontWeight={700} className="text">
                {t("Common:Actions")}
              </Text>
              <img src="/static/images/triangle-main-button.svg" />
            </button>
          </div>
        </StyledPreview>
      </ThemeProvider>
    </StyledWrapper>
  );
};

export default ThemePreview;
