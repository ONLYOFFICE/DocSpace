import React, { memo, useEffect, useRef, useState } from "react";
import { isMobileOnly } from "react-device-detect";

import {
  DropDown,
  DropDownItem,
  SpeedControlWrapper,
  ToastSpeed,
} from "./PlayerSpeedControl.styled";

import {
  PlayerSpeedControlProps,
  SpeedRecord,
  SpeedType,
} from "./PlayerSpeedControl.props";

import Icon05x from "PUBLIC_DIR/images/media.viewer05x.react.svg";
import Icon1x from "PUBLIC_DIR/images/media.viewer1x.react.svg";
import Icon15x from "PUBLIC_DIR/images/media.viewer15x.react.svg";
import Icon2x from "PUBLIC_DIR/images/media.viewer2x.react.svg";

const speedIcons = [<Icon05x />, <Icon1x />, <Icon15x />, <Icon2x />];

const speeds: SpeedType = ["X0.5", "X1", "X1.5", "X2"];

const speedRecord: SpeedRecord<SpeedType> = {
  "X0.5": 0.5,
  X1: 1,
  "X1.5": 1.5,
  X2: 2,
};

const DefaultIndexSpeed = 1;
const MillisecondShowSpeedToast = 2000;

function PlayerSpeedControl({
  handleSpeedChange,
  onMouseLeave,
  src,
}: PlayerSpeedControlProps) {
  const ref = useRef<HTMLDivElement>(null);

  const timerRef = useRef<NodeJS.Timeout>();

  const [currentIndexSpeed, setCurrentIndexSpeed] = useState<number>(
    DefaultIndexSpeed
  );
  const [isOpenSpeedContextMenu, setIsOpenSpeedContextMenu] = useState<boolean>(
    false
  );
  const [speedToastVisible, setSpeedToastVisible] = useState<boolean>(false);

  useEffect(() => {
    setCurrentIndexSpeed(DefaultIndexSpeed);
  }, [src]);

  useEffect(() => {
    const listener = (event: MouseEvent | TouchEvent) => {
      if (!ref.current || ref.current.contains(event.target as Node)) {
        return;
      }

      setIsOpenSpeedContextMenu(false);
    };
    document.addEventListener("mousedown", listener);
    return () => {
      document.removeEventListener("mousedown", listener);
      clearTimeout(timerRef.current);
    };
  }, []);

  const getNextIndexSpeed = (speed: number) => {
    switch (speed) {
      case 0:
        return 2;
      case 1:
        return 0;
      case 2:
        return 3;
      case 3:
        return 1;
      default:
        return DefaultIndexSpeed;
    }
  };

  const toggle = () => {
    if (isMobileOnly) {
      const nextIndexSpeed = getNextIndexSpeed(currentIndexSpeed);

      setCurrentIndexSpeed(nextIndexSpeed);

      const speed = speedRecord[speeds[nextIndexSpeed]];

      handleSpeedChange(speed);

      setSpeedToastVisible(true);
      clearTimeout(timerRef.current);

      timerRef.current = setTimeout(() => {
        setSpeedToastVisible(false);
      }, MillisecondShowSpeedToast);
    } else {
      setIsOpenSpeedContextMenu((prev) => !prev);
    }
  };

  return (
    <>
      {speedToastVisible && (
        <ToastSpeed>{speedIcons[currentIndexSpeed]}</ToastSpeed>
      )}
      <SpeedControlWrapper ref={ref} onClick={toggle}>
        {speedIcons[currentIndexSpeed]}

        {isOpenSpeedContextMenu && (
          <DropDown onMouseLeave={onMouseLeave}>
            {speeds.map((speed, index) => (
              <DropDownItem
                key={speed}
                onClick={() => {
                  setCurrentIndexSpeed(index);
                  handleSpeedChange(speedRecord[speed]);
                  onMouseLeave();
                }}
              >
                {speed}
              </DropDownItem>
            ))}
          </DropDown>
        )}
      </SpeedControlWrapper>
    </>
  );
}

export default memo(PlayerSpeedControl);
