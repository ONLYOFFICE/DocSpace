import React, { useState, useRef } from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

import { StyledDropDown, StyledDropDownWrapper } from "../StyledDropdown";

import { isHugeMobile } from "@docspace/components/utils/device";
import DomHelpers from "@docspace/components/utils/domHelpers";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import DropDownItem from "@docspace/components/drop-down-item";
import Checkbox from "@docspace/components/checkbox";
import { connectedCloudsTypeTitleTranslation } from "@docspace/client/src/helpers/filesUtils";

const StyledStorageLocation = styled.div`
  display: flex;
  flex-direction: column;

  .set_room_params-thirdparty {
    display: flex;
    flex-direction: row;
    gap: 8px;
    &-combobox {
      width: 100%;
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      padding: 5px 7px;
      background: #ffffff;
      border-radius: 3px;
      max-height: 32px;

      border: ${(props) => `1px solid ${props.isOpen ? "#2DA7DB" : "#d0d5da"}`};
      &:hover {
        border: ${(props) =>
          `1px solid ${props.isOpen ? "#2DA7DB" : "#a3a9ae"}`};
      }

      &-text {
        font-weight: 400;
        font-size: 13px;
        line-height: 20px;
      }

      &-expander {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 6.35px;
        svg {
          transform: ${(props) =>
            props.isOpen ? "rotate(180deg)" : "rotate(0)"};
          width: 6.35px;
          height: auto;
          path {
            fill: #a3a9ae;
          }
        }
      }
    }

    &-checkbox {
      margin-top: 8px;
      .checkbox {
        margin-right: 8px;
      }
      .checkbox-text {
        font-weight: 400;
        font-size: 13px;
        line-height: 20px;
      }
    }
  }
`;

const ThirpartyComboBox = ({
  t,
  providers,
  storageLocation,
  setChangeStorageLocation,
  setIsScrollLocked,
}) => {
  const dropdownRef = useRef(null);

  const thirdparties = providers.map((provider, i) => ({
    id: i,
    title: connectedCloudsTypeTitleTranslation(provider.provider_key, t),
  }));

  console.log(thirdparties);

  const [isOpen, setIsOpen] = useState(false);
  const [dropdownDirection, setDropdownDirection] = useState("bottom");

  const toggleIsOpen = () => {
    if (isOpen) setIsScrollLocked(false);
    else {
      setIsScrollLocked(true);
      calculateDropdownDirection();
    }
    setIsOpen(!isOpen);
  };

  const setStorageLocaiton = (thirparty) => {
    setChangeStorageLocation(thirparty);
    setIsOpen(false);
    setIsScrollLocked(false);
  };

  const calculateDropdownDirection = () => {
    const { top: offsetTop } = DomHelpers.getOffset(dropdownRef.current);
    const offsetBottom = window.innerHeight - offsetTop;

    const neededHeightDesktop = Math.min(thirdparties.length * 32 + 16, 404);
    const neededHeightMobile = Math.min(thirdparties.length * 32 + 16, 180);
    const neededheight = isHugeMobile()
      ? neededHeightMobile
      : neededHeightDesktop;

    setDropdownDirection(neededheight > offsetBottom ? "top" : "bottom");
  };

  return (
    <StyledStorageLocation isOpen={isOpen}>
      <div className="set_room_params-thirdparty">
        <div
          className="set_room_params-thirdparty-combobox"
          onClick={toggleIsOpen}
        >
          <Text className="set_room_params-thirdparty-combobox-text" noSelect>
            {storageLocation?.title ||
              t("ThirdPartyStorageComboBoxPlaceholder")}
          </Text>
          <ReactSVG
            className="set_room_params-thirdparty-combobox-expander"
            src={"/static/images/expander-down.react.svg"}
          />
        </div>

        <Button
          className="set_room_params-thirdparty-connect"
          size="small"
          label={t("Common:Connect")}
        />
      </div>

      <StyledDropDownWrapper
        className="dropdown-content-wrapper"
        ref={dropdownRef}
      >
        <StyledDropDown
          className="dropdown-content"
          open={isOpen}
          forwardedRef={dropdownRef}
          clickOutsideAction={toggleIsOpen}
          maxHeight={isHugeMobile() ? 158 : 382}
          directionY={dropdownDirection}
          marginTop={dropdownDirection === "bottom" ? "4px" : "-36px"}
        >
          {thirdparties.map((thirdparty) => (
            <DropDownItem
              className="dropdown-item"
              label={thirdparty.title}
              key={thirdparty.id}
              height={32}
              heightTablet={32}
              onClick={() => setStorageLocaiton(thirdparty)}
            />
          ))}
        </StyledDropDown>
      </StyledDropDownWrapper>
    </StyledStorageLocation>
  );
};

export default ThirpartyComboBox;
