import React from "react";
import { ThemeProvider } from "styled-components";
import RadioButton from "@docspace/components/radio-button";
import Text from "@docspace/components/text";

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
            <Text
              fontSize="11px"
              fontWeight={600}
              noSelect
              className="catalog-header"
            >
              {t("Common:Rooms")}
            </Text>
            <div className="item">
              <img src="/static/images/catalog.user.react.svg" />
              <Text className="label" fontWeight={600} noSelect>
                Personal
              </Text>
            </div>
            <div className="item item-active">
              <img src="/static/images/catalog.folder.react.svg" />
              <Text className="label" fontWeight={600} noSelect>
                Shared
              </Text>
            </div>
            <div className="item">
              <img src="/static/images/catalog.archive.react.svg" />
              <Text className="label" fontWeight={600} noSelect>
                Archived
              </Text>
            </div>
          </div>
          <div className="body">
            <Text fontSize="18px" fontWeight={700} noSelect>
              Shared
            </Text>
            <div className="search-input">
              <Text className="placeholder" fontWeight={600} noSelect>
                Find
              </Text>
            </div>
            <div className="row-header">
              <Text fontSize="12px" fontWeight={600} noSelect>
                Name
              </Text>
              <img src="/static/images/sort.desc.react.svg" />
            </div>

            <div className="row">
              <img className="icon" src="images/icons/32/room/review.svg" />
              <Text fontWeight={600} noSelect>
                Room
              </Text>
            </div>
            <div className="row">
              <img className="icon" src="images/icons/32/room/custom.svg" />
              <Text fontWeight={600} noSelect>
                Room
              </Text>
            </div>
          </div>
        </StyledPreview>
      </ThemeProvider>
    </StyledWrapper>
  );
};

export default ThemePreview;
