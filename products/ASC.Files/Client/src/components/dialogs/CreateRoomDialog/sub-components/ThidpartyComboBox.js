import React, { useState } from "react";
import styled, { css } from "styled-components";

import ComboBox from "@appserver/components/combobox";
import Button from "@appserver/components/button";
import { isSmallTablet } from "@appserver/components/utils/device";

const StyledThirpartyComboBox = styled.div`
  display: flex;
  flex-direction: row;
  gap: 8px;
  .set_room_params-thirdparty-combo_box {
    .combo-button-label {
      font-weight: 400;
      font-size: 13px;
      line-height: 20px;
      color: ${(props) => (props.isGrayLabel ? "#a3a9ae" : "#333333")};
    }
    svg {
      height: 4.8px;
    }
  }
`;

const ThirdpartyComboBox = ({ t, roomParams, setRoomParams }) => {
  const [isGrayLabel, setIsGrayLabel] = useState(true);

  const setNewThirdpartyDropdownWidth = () => {
    let dropdownContainer = document.getElementById("set_room_params-dropdown");
    const dropdownWidth = isSmallTablet() ? "calc(100vw - 32px)" : "448px";
    dropdownContainer.style.width = dropdownWidth;
    dropdownContainer.style.marginTop = "5px";
  };

  const onSetStorageLocation = (e) => {
    console.log(e);
    setRoomParams({ ...roomParams, storageLocation: e });
    setIsGrayLabel(false);
  };

  return (
    <StyledThirpartyComboBox
      isGrayLabel={isGrayLabel}
      className="set_room_params-thirdparty"
      onClick={setNewThirdpartyDropdownWidth}
    >
      <ComboBox
        dropDownId="set_room_params-dropdown"
        className="set_room_params-thirdparty-combo_box"
        scaled
        scaledOptions
        onSelect={onSetStorageLocation}
        selectedOption={
          roomParams.storageLocation || {
            key: 0,
            label: "Select",
          }
        }
        options={[
          {
            key: 1,
            label: "Onlyoffice DocSpace",
          },
          {
            key: 2,
            label: "Google Drive",
          },
        ]}
      />
      <Button
        className="set_room_params-thirdparty-connect"
        size="small"
        label={t("Common:Connect")}
      />
    </StyledThirpartyComboBox>
  );
};

export default ThirdpartyComboBox;
