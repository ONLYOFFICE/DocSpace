import React from "react";
const Track = () => {
  return (
    <svg className="loader_track" viewBox="-10 -10 220 220">
      <defs>
        <linearGradient
          id="spinner-color-1"
          gradientUnits="objectBoundingBox"
          x1="0"
          y1="0"
          x2="1"
          y2="1"
        >
          <stop offset="0%" stopColor="currentColor" stopOpacity="0" />
          <stop offset="100%" stopColor="currentColor" stopOpacity=".2" />
        </linearGradient>
        <linearGradient
          id="spinner-color-2"
          gradientUnits="objectBoundingBox"
          x1="0"
          y1="0"
          x2="0"
          y2="1"
        >
          <stop offset="0%" stopColor="currentColor" stopOpacity=".2" />
          <stop offset="100%" stopColor="currentColor" stopOpacity=".4" />
        </linearGradient>
        <linearGradient
          id="spinner-color-3"
          gradientUnits="objectBoundingBox"
          x1="1"
          y1="0"
          x2="0"
          y2="1"
        >
          <stop offset="0%" stopColor="currentColor" stopOpacity=".4" />
          <stop offset="100%" stopColor="currentColor" stopOpacity=".6" />
        </linearGradient>
        <linearGradient
          id="spinner-color-4"
          gradientUnits="objectBoundingBox"
          x1="1"
          y1="1"
          x2="0"
          y2="0"
        >
          <stop offset="0%" stopColor="currentColor" stopOpacity=".6" />
          <stop offset="100%" stopColor="currentColor" stopOpacity=".8" />
        </linearGradient>
        <linearGradient
          id="spinner-color-5"
          gradientUnits="objectBoundingBox"
          x1="0"
          y1="1"
          x2="0"
          y2="0"
        >
          <stop offset="0%" stopColor="currentColor" stopOpacity=".8" />
          <stop offset="100%" stopColor="currentColor" stopOpacity="1" />
        </linearGradient>
        <linearGradient
          id="spinner-color-6"
          gradientUnits="objectBoundingBox"
          x1="0"
          y1="1"
          x2="1"
          y2="0"
        >
          <stop offset="0%" stopColor="currentColor" stopOpacity="1" />
          <stop offset="100%" stopColor="currentColor" stopOpacity="1" />
        </linearGradient>
      </defs>
      <g
        fill="none"
        strokeWidth="60"
        transform="translate(100,100) scale(0.85)"
      >
        <path
          d="M 0,-100 A 100,100 0 0,1 86.6,-50"
          stroke="url(#spinner-color-1)"
        />
        <path
          d="M 86.6,-50 A 100,100 0 0,1 86.6,50"
          stroke="url(#spinner-color-2)"
        />
        <path
          d="M 86.6,50 A 100,100 0 0,1 0,100"
          stroke="url(#spinner-color-3)"
        />
        <path
          d="M 0,100 A 100,100 0 0,1 -86.6,50"
          stroke="url(#spinner-color-4)"
        />
        <path
          d="M -86.6,50 A 100,100 0 0,1 -86.6,-50"
          stroke="url(#spinner-color-5)"
        />
        <path
          d="M -86.6,-50 A 100,100 0 0,1 0,-100"
          stroke="url(#spinner-color-6)"
        />
      </g>

      <animateTransform
        from="0 0 0"
        to="360 0 0"
        attributeName="transform"
        type="rotate"
        repeatCount="indefinite"
        dur="1300ms"
      />
    </svg>
  );
};

export { Track };
