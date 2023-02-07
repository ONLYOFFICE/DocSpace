import * as React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { isMobileOnly } from "react-device-detect";
import { ReactSVG } from "react-svg";

const StyledMediaError = styled.div`
  background: rgba(0, 0, 0, 0.7);
  width: ${(props) => props.width + "px"};
  height: 56px;
  position: absolute;
  opacity: 1;
  z-index: 1006;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 20px;
`;

const StyledErrorToolbar = styled.div`
  padding: 10px 24px;
  bottom: 24px;
  z-index: 1006;
  position: fixed;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 18px;

  background: rgba(0, 0, 0, 0.4);
  &:hover {
    background: rgba(0, 0, 0, 0.8);
  }

  .toolbar-item {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 48px;
    width: 48px;
    &:hover {
      cursor: pointer;
    }
  }
`;

export const MediaError = ({
  width,
  height,
  onMaskClick,
  model,
  errorTitle,
}) => {
  let errorLeft = (window.innerWidth - width) / 2 + "px";
  let errorTop = (window.innerHeight - height) / 2 + "px";

  const items = !isMobileOnly
    ? model.filter((el) => el.key !== "rename")
    : model.filter((el) => el.key === "delete" || el.key === "download");

  return (
    <>
      <StyledMediaError
        width={width}
        height={height}
        style={{
          left: `${errorLeft}`,
          top: `${errorTop}`,
        }}
      >
        <Text
          fontSize="15px"
          color={"#fff"}
          textAlign="center"
          className="title"
        >
          {errorTitle}
        </Text>
      </StyledMediaError>

      <StyledErrorToolbar>
        {items.map((item) => {
          if (item.disabled) return;

          const onClick = () => {
            onMaskClick();
            item.onClick && item.onClick();
          };
          return (
            <div className="toolbar-item" key={item.key} onClick={onClick}>
              <ReactSVG src={item.icon} />
            </div>
          );
        })}
      </StyledErrorToolbar>
    </>
  );
};
