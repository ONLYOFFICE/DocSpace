import { css } from "styled-components";

export const getSizeInPx = (size: "small" | "medium" | "large") => {
  switch (size) {
    case "small":
      return "16px";
    case "medium":
      return "32px";
    case "large":
      return "96px";
  }
};

export const getBorderRadiusInPx = (size: "small" | "medium" | "large") => {
  switch (size) {
    case "small":
      return "3px";
    case "medium":
      return "6px";
    case "large":
      return "16px";
  }
};

export const getSizeInPxPrivate = (
  size: "small" | "medium" | "large",
  isCustom: undefined | boolean = false
) => {
  switch (size) {
    case "small":
      return isCustom ? "17.5px" : "17px";
    case "medium":
      return isCustom ? "35px" : "34px";
    case "large":
      return isCustom ? "105px" : "102px";
  }
};

export const getPrivateIconCss = (size: "small" | "medium" | "large") => {
  switch (size) {
    case "small":
      return css`
        height: 15px;
        width: 13px;
        top: -15px;
        left: 22px;
      `;
    case "medium":
      return css`
        height: 15px;
        width: 13px;
        bottom: 0;
        right: 0;
      `;
    case "large":
      return css`
        height: 45px;
        width: 41px;
        bottom: 0;
        right: 0;
        margin-right: -1px;
      `;
  }
};
