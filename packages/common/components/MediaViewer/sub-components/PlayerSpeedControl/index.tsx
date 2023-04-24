import React, { memo, useEffect, useRef, useState } from "react";
import { isMobileOnly } from "react-device-detect";

import {
  DropDown,
  DropDownItem,
  SpeedControlWrapper,
  ToastSpeed,
} from "./PlayerSpeedControl.styled";

import { PlayerSpeedControlProps } from "./PlayerSpeedControl.props";

import {
  DefaultIndexSpeed,
  getNextIndexSpeed,
  MillisecondShowSpeedToast,
  speedIcons,
  speedRecord,
  speeds,
} from "./PlayerSpeedControl.helper";

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

  const toggle = () => {
    if (isMobileOnly) {
      const nextIndexSpeed = getNextIndexSpeed(currentIndexSpeed);

      setCurrentIndexSpeed(nextIndexSpeed);

      const newSpeed = speedRecord[speeds[nextIndexSpeed]];

      handleSpeedChange(newSpeed);

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
