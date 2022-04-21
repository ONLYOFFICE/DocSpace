import React from "react";
import { StyledDualRing } from "../styled-loader";
// eslint-disable-next-line react/prop-types
export const DualRing = ({ size, color, label, theme }) => (
  <StyledDualRing
    width={size}
    height={size}
    viewBox="0 0 100 100"
    xmlns="http://www.w3.org/2000/svg"
    color={color}
    aria-label={label}
  >
    <circle
      cx="50"
      cy="50"
      ng-attr-r="{{config.radius}}"
      ng-attr-stroke-width="{{config.width}}"
      ng-attr-stroke="{{config.c1}}"
      ng-attr-stroke-dasharray="{{config.dasharray}}"
      fill="none"
      strokeLinecap="round"
      r="40"
      strokeWidth="8"
      stroke={color}
      strokeDasharray="62.83185307179586 62.83185307179586"
      transform="rotate(32.3864 50 50)"
    >
      <animateTransform
        attributeName="transform"
        type="rotate"
        calcMode="linear"
        values="0 50 50;360 50 50"
        keyTimes="0;1"
        dur="1.1s"
        begin="0s"
        repeatCount="indefinite"
      ></animateTransform>
    </circle>
    <circle
      cx="50"
      cy="50"
      ng-attr-r="{{config.radius2}}"
      ng-attr-stroke-width="{{config.width}}"
      ng-attr-stroke="{{config.c2}}"
      ng-attr-stroke-dasharray="{{config.dasharray2}}"
      ng-attr-stroke-dashoffset="{{config.dashoffset2}}"
      fill="none"
      strokeLinecap="round"
      r="20"
      strokeWidth="4"
      stroke={color}
      strokeDasharray="29.845130209103033 29.845130209103033"
      strokeDashoffset="29.845130209103033"
      transform="rotate(-360 -8.10878e-8 -8.10878e-8)"
    >
      <animateTransform
        attributeName="transform"
        type="rotate"
        calcMode="linear"
        values="0 50 50;-360 50 50"
        keyTimes="0;1"
        dur="1.1s"
        begin="0s"
        repeatCount="indefinite"
      ></animateTransform>
    </circle>
  </StyledDualRing>
);
