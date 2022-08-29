import React from "react";
import styled from "styled-components";

import RoomType from "../RoomType";

import { Scrollbar } from "@docspace/components";
import { Base } from "@docspace/components/themes";

const StyledDropdownMobile = styled.div`
  visibility: ${(props) => (props.isOpen ? "visible" : "hidden")};

  & > .dropdown-mobile-backdrop {
    z-index: 999;
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    margin-top: -64px;

    .dropdown-mobile-wrapper {
      border-radius: 6px 6px 0 0;
      padding: 6px 0;
      box-shadow: 0px -4px 60px rgba(4, 15, 27, 0.12);
      position: fixed;
      width: 100%;
      height: calc(100vh - 254px);

      bottom: -100%;
      transition: all 0.2s ease-in-out;
      &-active {
        bottom: 0;
      }
    }

    .dropdown-mobile-scrollbar {
      background: rgba(6, 22, 38, 0.2);
      .scroll-body {
        padding-right: 0 !important;
        * {
          -ms-overflow-style: none;
        }
        ::-webkit-scrollbar {
          display: none;
        }
      }

      .dropdown-mobile-content {
        background: ${(props) =>
          props.theme.createEditRoomDialog.roomTypeDropdown.mobile.background};
        box-sizing: border-box;
        width: 100%;
        display: flex;
        flex-direction: column;
      }
    }
  }
`;

StyledDropdownMobile.defaultProps = { theme: Base };

const DropdownMobile = ({ t, open, onClose, roomTypes, chooseRoomType }) => {
  return (
    <StyledDropdownMobile className="dropdown-mobile" isOpen={open}>
      <div className="dropdown-mobile-backdrop" onClick={onClose}>
        <div
          className={`dropdown-mobile-wrapper ${
            open && "dropdown-mobile-wrapper-active"
          }`}
        >
          <Scrollbar className="dropdown-mobile-scrollbar">
            <div className="dropdown-mobile-content">
              {roomTypes.map((room) => (
                <RoomType
                  t={t}
                  key={room.type}
                  room={room}
                  type="dropdownItem"
                  onClick={() => chooseRoomType(room.type)}
                />
              ))}
            </div>
          </Scrollbar>
        </div>
      </div>
    </StyledDropdownMobile>
  );
};

export default DropdownMobile;
