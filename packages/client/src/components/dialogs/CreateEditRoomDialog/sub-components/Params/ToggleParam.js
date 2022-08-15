import ToggleButton from "@docspace/components/toggle-button";
import React from "react";
import styled from "styled-components";
import { StyledParam } from "./StyledParam";

const StyledToggleParam = styled(StyledParam)`
  flex-direction: row;
  justify-content: space-between;
  gap: 8px;
  box-sizing: border-box;
  max-width: 100%;

  .set_room_params-info-description {
    box-sizing: border-box;
    max-width: 100%;
  }

  .set_room_params-toggle {
    width: 28px;
    min-width: 28px;
  }
`;

const ToggleParam = ({ title, description, isChecked, onCheckedChange }) => {
  return (
    <StyledToggleParam isPrivate>
      <div className="set_room_params-info">
        <div className="set_room_params-info-title">
          <div className="set_room_params-info-title-text">{title}</div>
        </div>
        <div className="set_room_params-info-description">{description}</div>
      </div>
      <ToggleButton
        className="set_room_params-toggle"
        isChecked={isChecked}
        onChange={onCheckedChange}
      />
    </StyledToggleParam>
  );
};

export default ToggleParam;
