import React, { useState } from "react";
import styled, { css } from "styled-components";
import RoomTypeDropdown from "./RoomTypeDropdown";
import ThirdpartyComboBox from "../../sub-components/ThidpartyComboBox";

import TextInput from "@appserver/components/text-input";
import Label from "@appserver/components/label";
import ToggleButton from "@appserver/components/toggle-button";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";
import AvatarEditor from "@appserver/components/avatar-editor";
import TagInput from "../../sub-components/TagInput";

const StyledSetRoomParams = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 20px;

  .set_room_params-input {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }
`;

const StyledParam = styled.div`
  box-sizing: border-box;
  display: flex;
  width: 100%;

  ${(props) =>
    props.isPrivate
      ? css`
          flex-direction: row;
          justify-content: space-between;
        `
      : props.storageLocation
      ? css`
          flex-direction: column;
          gap: 12px;
        `
      : ""}

  .set_room_params-info {
    display: flex;
    flex-direction: column;
    gap: 4px;

    .set_room_params-info-title {
      display: flex;
      flex-direction: row;
      align-items: center;
      gap: 6px;

      .set_room_params-info-title-text {
        font-weight: 600;
        font-size: 13px;
        line-height: 20px;
      }
      .set_room_params-info-title-help {
        border-radius: 50%;
        background-color: #a3a9ae;
        circle,
        rect {
          fill: #ffffff;
        }
      }
    }
    .set_room_params-info-description {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: #a3a9ae;
    }
  }

  .set_room_params-toggle {
    width: 28px;
    height: 16px;
    margin: 2px 0;
  }
`;

const StyledIconEditorWrapper = styled.div`
  .use_modal-avatar_editor_body {
    margin: 0;
  }

  .use_modal-buttons_wrapper {
    display: none;
  }
`;

const SetRoomParams = ({
  t,
  roomParams,
  setRoomParams,
  setRoomType,
  tagHandler,
  setIsScrollLocked,
}) => {
  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  return (
    <StyledSetRoomParams>
      <RoomTypeDropdown
        t={t}
        currentRoomType={roomParams.type}
        setRoomType={setRoomType}
      />

      <div className="set_room_params-input">
        <Label
          display="display"
          htmlFor="room-name"
          text={`${t("Common:Name")}:`}
          title="Fill the first name field"
        />
        <TextInput
          id="room-name"
          value={roomParams.title}
          onChange={(e) =>
            setRoomParams({ ...roomParams, title: e.target.value })
          }
          scale
          placeholder="New room"
          tabIndex={1}
        />
      </div>

      <TagInput
        t={t}
        tagHandler={tagHandler}
        setIsScrollLocked={setIsScrollLocked}
      />

      <StyledParam isPrivate>
        <div className="set_room_params-info">
          <div className="set_room_params-info-title">
            <Text className="set_room_params-info-title-text">
              {t("MakeRoomPrivateTitle")}
            </Text>
            <HelpButton
              displayType="auto"
              className="set_room_params-info-title-help"
              iconName="/static/images/info.react.svg"
              offsetRight={0}
              tooltipContent={t("MakeRoomPrivateDescription")}
              size={12}
            />
          </div>
          <div className="set_room_params-info-description">
            {t("MakeRoomPrivateDescription")}
          </div>
        </div>
        <ToggleButton
          className="set_room_params-toggle"
          isChecked={roomParams.isPrivate}
          onChange={onChangeIsPrivate}
        />
      </StyledParam>

      <StyledParam storageLocation>
        <div className="set_room_params-info">
          <div className="set_room_params-info-title">
            <Text className="set_room_params-info-title-text">
              {t("StorageLocationTitle")}
            </Text>
            <HelpButton
              displayType="auto"
              className="set_room_params-info-title-help"
              iconName="/static/images/info.react.svg"
              offsetRight={0}
              tooltipContent={t("StorageLocationDescription")}
              size={12}
            />
          </div>
          <div className="set_room_params-info-description">
            {t("StorageLocationDescription")}
          </div>
        </div>

        <ThirdpartyComboBox
          t={t}
          roomParams={roomParams}
          setRoomParams={setRoomParams}
          setIsScrollLocked={setIsScrollLocked}
        />
      </StyledParam>

      {/* <StyledIconEditorWrapper>
        <AvatarEditor useModalDialog={false}></AvatarEditor>
      </StyledIconEditorWrapper> */}
    </StyledSetRoomParams>
  );
};

export default SetRoomParams;
