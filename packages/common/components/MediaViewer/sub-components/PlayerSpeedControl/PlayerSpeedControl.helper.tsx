import React from "react";

import Icon05x from "PUBLIC_DIR/images/media.viewer05x.react.svg";
import Icon1x from "PUBLIC_DIR/images/media.viewer1x.react.svg";
import Icon15x from "PUBLIC_DIR/images/media.viewer15x.react.svg";
import Icon2x from "PUBLIC_DIR/images/media.viewer2x.react.svg";

import { SpeedRecord, SpeedType } from "./PlayerSpeedControl.props";

export enum SpeedIndex {
  Speed_X05 = 0,
  Speed_X10 = 1,
  Speed_X15 = 2,
  Speed_X20 = 3,
}

export const speedIcons = [<Icon05x />, <Icon1x />, <Icon15x />, <Icon2x />];

export const speeds: SpeedType = ["X0.5", "X1", "X1.5", "X2"];

export const speedRecord: SpeedRecord<SpeedType> = {
  "X0.5": 0.5,
  X1: 1,
  "X1.5": 1.5,
  X2: 2,
};

export const DefaultIndexSpeed = SpeedIndex.Speed_X10;
export const MillisecondShowSpeedToast = 2000;

/**
 *The function returns the following index based on the logic from the layout
 *https://www.figma.com/file/T49yt13Eiu7nzvj4ymfssV/DocSpace-1.0.0?node-id=34536-418523&t=Yv2Rp3stGISIQNcm-0
 */
export const getNextIndexSpeed = (currentSpeedIndex: number) => {
  switch (currentSpeedIndex) {
    case SpeedIndex.Speed_X10:
      return SpeedIndex.Speed_X05;
    case SpeedIndex.Speed_X05:
      return SpeedIndex.Speed_X15;
    case SpeedIndex.Speed_X15:
      return SpeedIndex.Speed_X20;
    case SpeedIndex.Speed_X20:
      return SpeedIndex.Speed_X10;
    default:
      return DefaultIndexSpeed;
  }
};
