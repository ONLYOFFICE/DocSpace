import React from "react";

export const DualRing = ({ width, height, color, label }) => (
  <svg
    width={width}
    height={height}
    viewBox="0 0 100 100"
    xmlns="http://www.w3.org/2000/svg"
    stroke={color}
    aria-label={label}
  >
    <circle cx="50" cy="50" 
      ng-attr-r="{{config.radius}}" 
      ng-attr-stroke-width="{{config.width}}" 
      ng-attr-stroke="{{config.c1}}" 
      ng-attr-stroke-dasharray="{{config.dasharray}}" 
      fill="none" 
      stroke-linecap="round" 
      r="40" 
      stroke-width="8" 
      stroke={color} 
      stroke-dasharray="62.83185307179586 62.83185307179586" 
      transform="rotate(32.3864 50 50)">
        <animateTransform 
          attributeName="transform" 
          type="rotate" 
          calcMode="linear" 
          values="0 50 50;360 50 50" 
          keyTimes="0;1" 
          dur="1.1s" 
          begin="0s"  
          repeatCount="indefinite">
        </animateTransform>
    </circle>
    <circle cx="50" cy="50" 
      ng-attr-r="{{config.radius2}}" 
      ng-attr-stroke-width="{{config.width}}" 
      ng-attr-stroke="{{config.c2}}" 
      ng-attr-stroke-dasharray="{{config.dasharray2}}" 
      ng-attr-stroke-dashoffset="{{config.dashoffset2}}" 
      fill="none" 
      stroke-linecap="round" 
      r="20" 
      stroke-width="4" 
      stroke={color}
      stroke-dasharray="29.845130209103033 29.845130209103033" 
      stroke-dashoffset="29.845130209103033" 
      transform="rotate(-360 -8.10878e-8 -8.10878e-8)">
        <animateTransform 
          attributeName="transform" 
          type="rotate" 
          calcMode="linear" 
          values="0 50 50;-360 50 50" 
          keyTimes="0;1" 
          dur="1.1s" 
          begin="0s" 
          repeatCount="indefinite">
        </animateTransform>
    </circle>
  </svg>
);