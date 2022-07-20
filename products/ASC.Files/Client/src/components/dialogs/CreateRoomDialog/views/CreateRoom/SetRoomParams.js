import React, { useState } from "react";
import styled from "styled-components";
import RoomTypeDropdown from "./RoomTypeDropdown";
import TextInput from "@appserver/components/text-input";
import Label from "@appserver/components/label";
import { roomTypes } from "../../roomTypes";
import TagList from "./TagList";
import ToggleButton from "@appserver/components/toggle-button";
import HelpButton from "@appserver/components/help-button";
import Text from "@appserver/components/text";

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
  flex-direction: row;
  justify-content: space-between;
  width: 100%;

  .set_room_params-param-info {
    display: flex;
    flex-direction: column;
    gap: 4px;

    .set_room_params-param-info-title {
      display: flex;
      flex-direction: row;
      align-items: center;
      gap: 6px;

      .set_room_params-param-info-title-text {
        font-weight: 600;
        font-size: 13px;
        line-height: 20px;
      }
      .set_room_params-param-info-title-help {
        border-radius: 50%;
        background-color: #a3a9ae;
        circle,
        rect {
          fill: #ffffff;
        }
      }
    }
    .set_room_params-param-info-description {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: #a3a9ae;
    }
  }
  .set_room_params-param-toggle {
    width: 28px;
    height: 16px;
    margin: 2px 0;
  }
`;

const SetRoomParams = ({
  t,
  roomParams,
  setRoomParams,
  setRoomType,
  tagHandler,
}) => {
  const [tagInput, setTagInput] = useState("");
  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  return (
    <StyledSetRoomParams>
      <RoomTypeDropdown
        t={t}
        currentRoomType={roomParams.type}
        setRoomType={setRoomType}
      />

      <div className="set_room_params-input set_room_params-text_input">
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

      <div className="set_room_params-input set_room_params-tag_input">
        <Label
          display="display"
          htmlFor="tags-input"
          text={`${t("Tags")}:`}
          title="Fill the first name field"
        />
        <TextInput
          id="tag-input"
          value={tagInput}
          onChange={(e) => setTagInput(e.target.value)}
          scale
          placeholder={t("Add a tag")}
          tabIndex={1}
        />

        <TagList t={t} tagHandler={tagHandler} />
      </div>

      <StyledParam>
        <div className="set_room_params-param-info">
          <div className="set_room_params-param-info-title">
            <Text className="set_room_params-param-info-title-text">
              {t("MakeRoomPrivateTitle")}
            </Text>
            <HelpButton
              displayType="auto"
              className="set_room_params-param-info-title-help"
              iconName="/static/images/info.react.svg"
              offsetRight={0}
              tooltipContent={t("MakeRoomPrivateDescription")}
              size={12}
            />
          </div>
          <div className="set_room_params-param-info-description">
            {t("MakeRoomPrivateDescription")}
          </div>
        </div>
        <ToggleButton
          className="set_room_params-param-toggle"
          isChecked={roomParams.isPrivate}
          onChange={onChangeIsPrivate}
        />
      </StyledParam>

      <StyledParam>
        <div className="set_room_params-param-info">
          <div className="set_room_params-param-info-title">
            <Text className="set_room_params-param-info-title-text">
              {t("StorageLocationTitle")}
            </Text>
            <HelpButton
              displayType="auto"
              className="set_room_params-param-info-title-help"
              iconName="/static/images/info.react.svg"
              offsetRight={0}
              tooltipContent={t("StorageLocationDescription")}
              size={12}
            />
          </div>
          <div className="set_room_params-param-info-description">
            {t("StorageLocationDescription")}
          </div>
        </div>
      </StyledParam>
    </StyledSetRoomParams>
  );
};

export default SetRoomParams;
