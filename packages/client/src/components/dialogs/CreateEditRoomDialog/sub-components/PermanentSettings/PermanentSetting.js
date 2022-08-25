import React from "react";
import styled, { css } from "styled-components";
import { ReactSVG } from "react-svg";

import SecondaryInfoButton from "../SecondaryInfoButton";

const StyledPermanentSetting = styled.div`
  box-sizing: border-box;
  display: flex;
  flex-direction: ${(props) => (props.isFull ? "column" : "row")};
  align-items: ${(props) => (props.isFull ? "start" : "center")};
  justify-content: ${(props) => (props.isFull ? "center" : "start")};
  gap: 4px;

  width: 100%;
  max-width: 100%;
  padding: 12px 16px;

  background: #f8f9f9;
  border-radius: 6px;

  user-select: none;

  .permanent_setting-main_info {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: start;
    gap: 8px;

    &-icon {
      width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin: -2px;

      svg {
        max-width: 100%;
        max-height: 100%;

        ${(props) =>
          props.type === "privacy" &&
          css`
            path {
              fill: #35ad17;
            }
          `}
      }
    }

    &-title {
      font-weight: 600;
      font-size: 12px;
      line-height: 16px;
    }
  }

  .permanent_setting-help_button {
    margin-left: auto;
    white-space: pre-line;
  }

  .permanent_setting-secondary-info {
    font-weight: 400;
    font-size: 12px;
    line-height: 16px;
    color: #555f65;
    white-space: pre-line;
  }
`;

const PermanentSetting = ({ isFull, type, icon, title, content }) => {
  return (
    <StyledPermanentSetting
      className="permanent_setting"
      isFull={isFull}
      type={type}
    >
      <div className="permanent_setting-main_info">
        <ReactSVG className="permanent_setting-main_info-icon" src={icon} />
        <div className="permanent_setting-main_info-title">{title}</div>
      </div>

      {isFull ? (
        <div className="permanent_setting-secondary-info">{content}</div>
      ) : (
        <div className="permanent_setting-help_button">
          <SecondaryInfoButton content={content} />
        </div>
      )}
    </StyledPermanentSetting>
  );
};

export default PermanentSetting;
