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

export const getSizeInPxPrivate = (size: "small" | "medium" | "large") => {
  switch (size) {
    case "small":
      return "17px";
    case "medium":
      return "34px";
    case "large":
      return "102px";
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
        top: -15px;
        left: 22px;
      `;
    case "large":
      return css`
        height: 45px;
        width: 39px;
        top: -39px;
        left: 66px;
      `;
  }
};
